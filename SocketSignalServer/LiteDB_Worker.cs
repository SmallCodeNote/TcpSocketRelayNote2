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

            _LiteDBconnectionString = new ConnectionString();
            _LiteDBconnectionString.Filename = dbFilePath;
            _LiteDBconnectionString.Connection = ConnectionType.Shared;

            if (!Directory.Exists(BackupDirTopPath)) { BackupDirTopPath = Path.Combine(Path.GetDirectoryName(dbFilePath), "_backup"); }

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
            Stopwatch sw = new Stopwatch();

            while (!token.IsCancellationRequested)
            {
                sw.Reset(); sw.Start();
                List<string> DebugOutLines = new List<string>();

                try
                {
                    //QueueLoad
                    if (!saveQue.IsEmpty)
                    {
                        List<SocketMessage> upsertMessages = new List<SocketMessage>();
                        while (saveQue.TryDequeue(out SocketMessage socketMessage))
                        {
                            upsertMessages.Add(socketMessage);
                        }

                        int insertCount = 0, updateCount = 0;
                        long startMillisec = sw.ElapsedMilliseconds;

                        List<SocketMessage> insertMessages = new List<SocketMessage>();
                        List<SocketMessage> updateMessages = new List<SocketMessage>();
                        List<string> updateKeys = new List<string>();

                        //ListUpdate
                        foreach (var upsertMessage in upsertMessages)
                        {
                            if (AllListInFile.Any(x => x.Key == upsertMessage.Key))
                            {
                                updateCount++;
                                AllListInFile.First(x => x.Key == upsertMessage.Key).Update(upsertMessage);
                                updateMessages.Add(upsertMessage);
                                updateKeys.Add(upsertMessage.Key);
                            }
                            else
                            {
                                insertCount++;
                                insertMessages.Add(upsertMessage);
                            }
                        }
                        
                        AllListInFile.AddRange(insertMessages.ToList());

                        int ListUpdateTimeInMilisec = (int)(sw.ElapsedMilliseconds - startMillisec);
                        DebugOutLines.Add("insertCount: " + insertCount.ToString() + " updateCount: " + updateCount.ToString() + " Item/Sec: " + ((insertCount + updateCount) * 1000 / (ListUpdateTimeInMilisec)).ToString());
                        Debug.WriteLine(string.Join("\r\n", DebugOutLines)); DebugOutLines.Clear();


                        //DBfileUpdate
                        if (upsertMessages.Count > 0)
                        {
                            using (LiteDatabase litedb = new LiteDatabase(_LiteDBconnectionString))
                            {
                                ILiteCollection<SocketMessage> liteCollection = litedb.GetCollection<SocketMessage>(TableName);
                                List<SocketMessage> liteCollectionList = liteCollection.Query().ToList();

                                DebugOutLines.Add(DateTime.Now.ToString("HH:mm:ss.fff") + " insertBulkList");
                                if (insertCount > 0) liteCollection.InsertBulk(insertMessages);

                                DebugOutLines.Add(DateTime.Now.ToString("HH:mm:ss.fff") + " updateMany");
                                if (updateCount > 0)
                                {
                                    liteCollection.DeleteMany(x => updateKeys.Contains(x.Key));
                                    liteCollection.InsertBulk(updateMessages);
                                }

                                DebugOutLines.Add(DateTime.Now.ToString("HH:mm:ss.fff") + " updateFinish");
                                LastUpdateTime = DateTime.Now;

                                _RefreshFlag = false;
                            }
                        }
                        if (DebugOutLines.Count > 0) Debug.WriteLine(string.Join("\r\n", DebugOutLines)); DebugOutLines.Clear();
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
                    List<SocketMessage> RemoveDataList = new List<SocketMessage>();
                    if ((DateTime.Now - LastBackupTime).TotalSeconds >= BackupIntervalSecond)
                    {
                        RemoveDataList.AddRange( BreakupLightDB_byMonthFile());
                        if (RemoveDataList.Count > 0)
                        {
                            using (LiteDatabase litedb = new LiteDatabase(_LiteDBconnectionString))
                            {
                                ILiteCollection<SocketMessage> liteCollection = litedb.GetCollection<SocketMessage>(TableName);

                                List<string> deleteKeys = RemoveDataList.Select(x => x.Key).ToList();
                                liteCollection.DeleteMany(x => deleteKeys.Contains(x.Key));

                                litedb.Rebuild();
                                LastBackupTime = DateTime.Now;
                            }
                            string backupBuildFilename = Path.Combine(Path.GetDirectoryName(_LiteDBconnectionString.Filename), Path.GetFileNameWithoutExtension(_LiteDBconnectionString.Filename) + "-backup" + Path.GetExtension(_LiteDBconnectionString.Filename));
                            if (File.Exists(backupBuildFilename)) { File.Delete(backupBuildFilename); };
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
                catch (Exception ex)
                {
                    if (DebugOutLines.Count > 0) Debug.WriteLine(string.Join("\r\n", DebugOutLines)); DebugOutLines.Clear();
                    Debug.WriteLine(GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ex.ToString());
                }
            }

            return;
        }

        public int BreakupLightDB_byMonthFile(LiteDatabase litedb)
        {
            Stopwatch sw = new Stopwatch(); sw.Start();
            int processCount = 0;
            TimeSpan timeSpan = new TimeSpan(0, StoreTimeRangeMinute, 0);

            var target = AllListInFile
                .Where(x => x.connectTime < (DateTime)(DateTime.Now - timeSpan))
                .OrderBy(x => x.connectTime)
                ;

            List<SocketMessage> targetQueryList = target.ToList();
            if (targetQueryList.Count < 1) { return processCount; };

            DateTime targetFirstTime = targetQueryList.First().connectTime;
            DateTime targetLastTime = targetQueryList.Last().connectTime;

            TimeSpan fileTimeSpan = new TimeSpan(31, 0, 0, 0);
            DateTime fileStartTime = DateTime.Parse(targetFirstTime.ToString("yyyy/MM/01"));
            DateTime fileEndTime = DateTime.Parse((fileStartTime + fileTimeSpan).ToString("yyyy/MM/01"));

            ILiteCollection<SocketMessage> liteCollection = litedb.GetCollection<SocketMessage>(TableName);

            do
            {
                ConnectionString backupFileString = new ConnectionString();
                backupFileString.Connection = ConnectionType.Direct;//.Shared;//
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
                        Debug.WriteLine("OpenLiteDB\t" + GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + " Filename: " + Path.GetFileName(backupFileString.Filename));
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
                                Debug.WriteLine("RollbackLiteDB\t" + GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name);
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

            sw.Stop();
            Debug.WriteLine("litedb Update ... " + GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + " sw = " + sw.ElapsedMilliseconds.ToString());

            return processCount;
        }

        public List<SocketMessage> BreakupLightDB_byMonthFile()
        {
            List<SocketMessage> RemoveDataList = new List<SocketMessage>();
            Stopwatch sw = new Stopwatch(); sw.Start();

            TimeSpan timeSpan = new TimeSpan(0, StoreTimeRangeMinute, 0);

            List<SocketMessage> breakupTargetList = AllListInFile
                .Where(x => x.connectTime < (DateTime)(DateTime.Now - timeSpan))
                .OrderBy(x => x.connectTime).ToList();

            if (breakupTargetList.Count < 1) { return RemoveDataList; };


            DateTime targetFirstTime = breakupTargetList.First().connectTime;
            DateTime targetLastTime = breakupTargetList.Last().connectTime;

            TimeSpan fileTimeSpan = new TimeSpan(31, 0, 0, 0);
            DateTime fileStartTime = DateTime.Parse(targetFirstTime.ToString("yyyy/MM/01"));
            DateTime fileEndTime = DateTime.Parse((fileStartTime + fileTimeSpan).ToString("yyyy/MM/01"));

            do
            {
                ConnectionString backupFileString = new ConnectionString();
                backupFileString.Connection = ConnectionType.Direct;//.Shared;//
                backupFileString.Filename = Path.Combine(BackupDirTopPath, fileStartTime.ToString("yyyy"), fileStartTime.ToString("yyyyMM")) + ".db";

                string backupFileDir = Path.GetDirectoryName(backupFileString.Filename);
                if (!Directory.Exists(backupFileDir)) { Directory.CreateDirectory(backupFileDir); }

                List<SocketMessage> filteredBreakupTargetList = breakupTargetList.Where(x => x.connectTime >= fileStartTime && x.connectTime < fileEndTime).ToList();
                List<SocketMessage> storedMonthDataList = new List<SocketMessage>();
                List<SocketMessage> monthInsertList = new List<SocketMessage>();
                List<SocketMessage> monthUpdateList = new List<SocketMessage>();

                try
                {
                    using (LiteDatabase litedbBackup = new LiteDatabase(backupFileString))
                    {
                        var colbk = litedbBackup.GetCollection<SocketMessage>(TableName);
                        storedMonthDataList = colbk.Query().ToList();

                        Debug.WriteLine("OpenLiteDB\t" + GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + " Filename: " + Path.GetFileName(backupFileString.Filename) +" sw: "+sw.ElapsedMilliseconds.ToString());

                        try
                        {
                            foreach (SocketMessage skm in filteredBreakupTargetList)
                            {
                                if (storedMonthDataList.Any(x => x.Key == skm.Key))
                                {
                                    monthInsertList.Add(skm);
                                }
                                else
                                {
                                    monthUpdateList.Add(skm);
                                }

                                RemoveDataList.Add(skm);
                            }

                            if (monthInsertList.Count > 0) { colbk.InsertBulk(monthInsertList); }
                            if (monthUpdateList.Count > 0)
                            {
                                List<string> deleteKeys = monthUpdateList.Select(x => x.Key).ToList();
                                colbk.DeleteMany(x => deleteKeys.Contains(x.Key));
                                colbk.InsertBulk(monthUpdateList);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("litedbBackup file operation exception ... " + GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ex.ToString());
                        }
                    }

                    foreach (var item in filteredBreakupTargetList) { AllListInFile.RemoveAll(x => x.Key == item.Key); }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("litedbBackup file open exception ... " + GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ex.ToString());
                }

                fileStartTime = fileEndTime;
                fileEndTime = DateTime.Parse((fileStartTime + fileTimeSpan).ToString("yyyy/MM/01"));

            } while (fileStartTime < targetLastTime);

            sw.Stop();
            Debug.WriteLine("litedb Update ... " + GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + " sw = " + sw.ElapsedMilliseconds.ToString());

            return RemoveDataList;
        }
    }

}
