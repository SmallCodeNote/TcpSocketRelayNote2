using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using LiteDB;

namespace SocketSignalServer
{
    public class LiteDB_Worker : IDisposable
    {
        private ConnectionString _LiteDBconnectionString;
        public string TableName = "table_Message";

        private List<SocketMessage> AllListInFile;

        private ConcurrentQueue<SocketMessage> saveQue;
        private ConcurrentQueue<Task<List<SocketMessage>>> loadQue;

        private Task worker;
        private CancellationTokenSource tokenSource;

        public string BackupDirTopPath = "";
        public int StoreTimeRangeMinute = 1440;
        public int BackupIntervalSecond = 60;
        public float BackupIntervalMinute { get { return BackupIntervalSecond / 60f; } set { BackupIntervalSecond = (int)(value * 60); } }
        public DateTime LastBackupTime;
        public DateTime LastUpdateTime;

        private bool _RefreshFlag = false;

        /// <summary> CheckInterval(ms) </summary>
        public int Interval = 200;

        public LiteDB_Worker(string dbFilePath, string backupDirTopPath = "")
        {
            tokenSource = new CancellationTokenSource();

            //_LiteDBconnectionString = new ConnectionString();
            //_LiteDBconnectionString.Filename = dbFilePath;
            //_LiteDBconnectionString.Connection = ConnectionType.Shared;

            _LiteDBconnectionString = new ConnectionString(dbFilePath)
            {
                Connection = ConnectionType.Shared
            };

            if (!Directory.Exists(BackupDirTopPath))
            {
                BackupDirTopPath = Path.Combine(Path.GetDirectoryName(dbFilePath), "_backup");
            }

            saveQue = new ConcurrentQueue<SocketMessage>();
            loadQue = new ConcurrentQueue<Task<List<SocketMessage>>>();

            if (System.IO.Directory.Exists(Path.GetDirectoryName(dbFilePath)))
            {
                using (LiteDatabase litedb = new LiteDatabase(_LiteDBconnectionString))
                {
                    ILiteCollection<SocketMessage> liteCollection = litedb.GetCollection<SocketMessage>(TableName);
                    AllListInFile = liteCollection.Query().ToList();
                    LastUpdateTime = DateTime.Now;
                }
                worker = Task.Run(() => Worker(tokenSource.Token));
            }

            LastBackupTime = DateTime.Now - TimeSpan.FromSeconds(BackupIntervalSecond);
        }

        public void Dispose()
        {
            if (tokenSource != null)
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
            }
        }

        public void SaveData(SocketMessage message)
        {
            saveQue.Enqueue(message);
        }

        public List<SocketMessage> LoadData()
        {
            Task<List<SocketMessage>> task = new Task<List<SocketMessage>>(() => new List<SocketMessage>(AllListInFile));
            loadQue.Enqueue(task);
            return task.Result;
        }

        public void Refresh()
        {
            DateTime lastUpdateTimeStore = LastUpdateTime;
            _RefreshFlag = true;

            while (lastUpdateTimeStore == LastUpdateTime) { Thread.Sleep(100); }
        }

        public void Start()
        {
            if (worker == null || worker.IsCompleted)
            {
                var token = tokenSource.Token;
                worker = Task.Run(() => { try { Worker(token); } catch { } }, token);
            }
        }

        public void Stop()
        {
            tokenSource.Cancel();
            worker.Wait();
        }

        private void Worker(CancellationToken token)
        {
            int processCount = 0;
            Stopwatch sw = new Stopwatch();

            while (!token.IsCancellationRequested)
            {
                sw.Reset(); sw.Start();

                //QueueLoad
                if (!saveQue.IsEmpty)
                {
                    List<SocketMessage> newMessages = new List<SocketMessage>();
                    while (saveQue.TryDequeue(out SocketMessage socketMessage))
                    {
                        newMessages.Add(socketMessage);
                    }

                    //ListUpdate
                    foreach (var newMessage in newMessages)
                    {
                        var items = AllListInFile.Where(x => x.Key == newMessage.Key);

                        if (items.Count() == 0)
                        {
                            AllListInFile.Add(newMessage);
                            Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " saveList");
                        }
                        else
                        {
                            foreach (var item in items) { item.Update(newMessage); }
                            Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " updateList " + newMessage.ToString());
                        }
                    }

                    //DBfileUpdate
                    if (newMessages.Count > 0)
                    {
                        using (LiteDatabase litedb = new LiteDatabase(_LiteDBconnectionString))
                        {
                            ILiteCollection<SocketMessage> liteCollection = litedb.GetCollection<SocketMessage>(TableName);

                            foreach (var socketMessage in newMessages)
                            {
                                if (liteCollection.FindById(socketMessage.Key) == null)
                                {
                                    liteCollection.Insert(socketMessage.Key, socketMessage);
                                    Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " saveDBfile " + socketMessage.ToString());
                                }
                                else
                                {
                                    liteCollection.Update(socketMessage.Key, socketMessage);
                                    Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " updateDBfile " + socketMessage.ToString());
                                }
                            }

                            LastUpdateTime = DateTime.Now;

                            _RefreshFlag = false;
                        }
                    }
                }

                //RefleshList
                if (_RefreshFlag)
                {
                    using (LiteDatabase litedb = new LiteDatabase(_LiteDBconnectionString))
                    {
                        ILiteCollection<SocketMessage> liteCollection = litedb.GetCollection<SocketMessage>(TableName);
                        AllListInFile = liteCollection.Query().ToList();
                    }

                    LastUpdateTime = DateTime.Now;
                    _RefreshFlag = false;
                }

                //MoveOldDataToBackup
                if ((DateTime.Now - LastBackupTime).TotalSeconds >= BackupIntervalSecond)
                {
                    using (LiteDatabase litedb = new LiteDatabase(_LiteDBconnectionString))
                    {
                        processCount += BreakupLightDB_byMonthFile(litedb);
                        LastBackupTime = DateTime.Now;

                        if (processCount > 1000)
                        {
                            litedb.Rebuild();
                            string backupBuildFilename = Path.Combine(Path.GetDirectoryName(_LiteDBconnectionString.Filename), Path.GetFileNameWithoutExtension(_LiteDBconnectionString.Filename) + "-backup" + Path.GetExtension(_LiteDBconnectionString.Filename));

                            if (File.Exists(backupBuildFilename))
                            {
                                File.Delete(backupBuildFilename);
                            };

                            processCount = 0;
                        }
                    }
                }

                //LoadData
                List<Task<List<SocketMessage>>> taskList = new List<Task<List<SocketMessage>>>();
                while (!loadQue.IsEmpty)
                {
                    while (loadQue.TryDequeue(out Task<List<SocketMessage>> task))
                    {
                        taskList.Add(task);
                        task.Start();
                    }
                    Task<List<SocketMessage>>.WaitAll(taskList.ToArray());
                }

                //Interval
                sw.Stop();
                int remainingTime = Interval - (int)sw.ElapsedMilliseconds;
                if (remainingTime > 0)
                {
                    Task.Delay(TimeSpan.FromMilliseconds(remainingTime), token).Wait();
                }
                else
                {
                    Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " workerIntervalOver " + remainingTime.ToString() + "ms");
                }
            }

            return;
        }

        public int BreakupLightDB_byMonthFile(LiteDatabase litedb)
        {
            int processCount = 0;
            TimeSpan timeSpan = new TimeSpan(0, StoreTimeRangeMinute, 0);

            ILiteCollection<SocketMessage> liteCollection = litedb.GetCollection<SocketMessage>(TableName);
            var target = AllListInFile
                .Where(x => x.connectTime < (DateTime)(DateTime.Now - timeSpan))
                .OrderBy(x => x.connectTime)
                ;

            List<SocketMessage> targetQueryList = target.ToList();
            if (targetQueryList.Count < 1) { return processCount; };

            DateTime targetFirstTime = target.First().connectTime;
            DateTime targetLastTime = target.Last().connectTime;

            TimeSpan fileTimeSpan = new TimeSpan(31, 0, 0, 0);
            DateTime fileStartTime = DateTime.Parse(targetFirstTime.ToString("yyyy/MM/01"));
            DateTime fileEndTime = DateTime.Parse((fileStartTime + fileTimeSpan).ToString("yyyy/MM/01"));

            do
            {
                ConnectionString backupFileString = new ConnectionString();
                backupFileString.Connection = ConnectionType.Shared;
                backupFileString.Filename = Path.Combine(BackupDirTopPath, fileStartTime.ToString("yyyy"), fileStartTime.ToString("yyyyMM")) + ".db";

                string backupFileDir = Path.GetDirectoryName(backupFileString.Filename);
                if (!Directory.Exists(backupFileDir))
                {
                    Directory.CreateDirectory(backupFileDir);
                }

                var backupQueryList = targetQueryList.Where(x => x.connectTime < fileEndTime && x.connectTime >= fileStartTime).ToList();

                try
                {
                    using (LiteDatabase litedbBackup = new LiteDatabase(backupFileString))
                    {
                        Debug.WriteLine("OpenLiteDB\t" + GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name);
                        var colbk = litedbBackup.GetCollection<SocketMessage>(TableName);

                        try
                        {
                            foreach (SocketMessage skm in backupQueryList)
                            {
                                if (colbk.FindById(skm.Key) == null)
                                {
                                    colbk.Insert(skm.Key, skm);
                                }
                                liteCollection.Delete(skm.Key);
                                processCount++;
                            }

                            if (!litedb.Commit())
                            {
                                litedb.Rollback();
                            }
                        }
                        catch (Exception ex)
                        {
                            litedb.Rollback();
                            Debug.WriteLine("litedbBackup file operation exception ... " + GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ex.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("litedbBackup file open exception ... " + GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ex.ToString());
                }

                fileStartTime = fileEndTime;
                fileEndTime = DateTime.Parse((fileStartTime + fileTimeSpan).ToString("yyyy/MM/01"));

            } while (fileStartTime < targetLastTime);

            AllListInFile = liteCollection.Query().ToList();

            return processCount;
        }
    }
}