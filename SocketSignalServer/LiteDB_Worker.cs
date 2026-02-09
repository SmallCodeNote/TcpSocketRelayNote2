using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;

namespace SocketSignalServer
{

    public class LiteDB_Worker : IDisposable
    {
        private readonly ConnectionString _liteDbConnectionString;
        public string TableName = "table_Message";

        // Main data : List + Dictionary
        private readonly List<SocketMessage> _allListInFile;
        private readonly Dictionary<string, SocketMessage> _allDictInFile;
        private readonly ReaderWriterLockSlim _dataLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        private readonly ConcurrentQueue<SocketMessage> _saveQueue;

        private Task _worker;
        private CancellationTokenSource _tokenSource;
        private readonly object _workerLock = new object();

        public string BackupDirTopPath = "";
        public int StoreTimeRangeMinute = 1440;
        public int BackupIntervalSecond = 60;
        public float BackupIntervalMinute
        {
            get { return BackupIntervalSecond / 60f; }
            set { BackupIntervalSecond = (int)Math.Round(value * 60); }
        }

        public DateTime LastBackupTime;
        public DateTime LastUpdateTime;

        private volatile bool _refreshFlag = false;
        private readonly ManualResetEventSlim _refreshCompletedEvent = new ManualResetEventSlim(true);

        /// <summary> CheckInterval(ms) </summary>
        public int Interval { get; set; } = 200;

        private volatile bool _initialized = false;

        public LiteDB_Worker(string dbFilePath, string backupDirTopPath = "")
        {
            _tokenSource = new CancellationTokenSource();

            _liteDbConnectionString = new ConnectionString
            {
                Filename = dbFilePath,
                Connection = ConnectionType.Shared
            };

            // BackupDirTopPath initialize
            if (!string.IsNullOrWhiteSpace(backupDirTopPath))
            {
                BackupDirTopPath = backupDirTopPath;
            }
            else
            {
                var baseDir = Path.GetDirectoryName(dbFilePath);
                if (!string.IsNullOrEmpty(baseDir))
                {
                    BackupDirTopPath = Path.Combine(baseDir, "_backup");
                }
            }

            _saveQueue = new ConcurrentQueue<SocketMessage>();
            _allListInFile = new List<SocketMessage>();
            _allDictInFile = new Dictionary<string, SocketMessage>();

            try
            {
                var dir = Path.GetDirectoryName(dbFilePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                using (var litedb = new LiteDatabase(_liteDbConnectionString))
                {
                    var liteCollection = litedb.GetCollection<SocketMessage>(TableName);
                    var list = liteCollection.Query().ToList();

                    _dataLock.EnterWriteLock();
                    try
                    {
                        _allListInFile.Clear();
                        _allListInFile.AddRange(list);

                        _allDictInFile.Clear();
                        foreach (var m in list)
                        {
                            if (!string.IsNullOrEmpty(m.Key))
                            {
                                _allDictInFile[m.Key] = m;
                            }
                        }

                        LastUpdateTime = DateTime.Now;
                    }
                    finally
                    {
                        _dataLock.ExitWriteLock();
                    }
                }

                _initialized = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{GetType().Name}::Ctor LiteDB init exception: {ex}");
                _initialized = false;
            }

            LastBackupTime = DateTime.Now;
            if (_initialized)
            {
                Start();
            }
        }

        public void Dispose()
        {
            try
            {
                Stop();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{GetType().Name}::Dispose Stop exception: {ex}");
            }

            _dataLock?.Dispose();
            _refreshCompletedEvent?.Dispose();
        }

        public void SaveData(SocketMessage message)
        {
            if (message == null) return;
            if (!_initialized)
            {
                Debug.WriteLine($"{GetType().Name}::SaveData called while not initialized.");
                return;
            }

            _saveQueue.Enqueue(message);
        }

        public List<SocketMessage> LoadData()
        {
            _dataLock.EnterReadLock();
            try
            {
                return new List<SocketMessage>(_allListInFile);
            }
            finally
            {
                _dataLock.ExitReadLock();
            }
        }

        public void Refresh()
        {
            if (!_initialized)
            {
                Debug.WriteLine($"{GetType().Name}::Refresh called while not initialized.");
                return;
            }

            _refreshCompletedEvent.Reset();
            _refreshFlag = true;

            if (!_refreshCompletedEvent.Wait(TimeSpan.FromSeconds(5)))
            {
                Debug.WriteLine($"{GetType().Name}::Refresh timeout.");
            }
        }

        public void Start()
        {
            if (!_initialized)
            {
                Debug.WriteLine($"{GetType().Name}::Start called while not initialized.");
                return;
            }

            lock (_workerLock)
            {
                if (_worker != null && !_worker.IsCompleted)
                {
                    return;
                }

                if (_tokenSource == null || _tokenSource.IsCancellationRequested)
                {
                    _tokenSource?.Dispose();
                    _tokenSource = new CancellationTokenSource();
                }

                var token = _tokenSource.Token;
                _worker = Task.Factory.StartNew(
                    () =>
                    {
                        try
                        {
                            Worker(token);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"{GetType().Name}::Start Worker exception: {ex}");
                        }
                    },
                    token,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default);
            }
        }

        public void Stop()
        {
            lock (_workerLock)
            {
                if (_tokenSource == null)
                {
                    return;
                }

                try
                {
                    _tokenSource.Cancel();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{GetType().Name}::Stop Cancel exception: {ex}");
                }

                try
                {
                    if (_worker != null && !_worker.IsCompleted)
                    {
                        _worker.Wait(2000);
                    }
                }
                catch (AggregateException aex)
                {
                    foreach (var ex in aex.Flatten().InnerExceptions)
                    {
                        Debug.WriteLine($"{GetType().Name}::Stop Worker wait inner exception: {ex}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{GetType().Name}::Stop Worker wait exception: {ex}");
                }

                _tokenSource.Dispose();
                _tokenSource = null;
                _worker = null;
            }
        }

        private void Worker(CancellationToken token)
        {
            var sw = new Stopwatch();

            using (var litedb = new LiteDatabase(_liteDbConnectionString))
            {
                var liteCollection = litedb.GetCollection<SocketMessage>(TableName);

                while (!token.IsCancellationRequested)
                {
                    sw.Reset();
                    sw.Start();
                    var debugLines = new List<string>();

                    try
                    {
                        // Queue Save
                        if (!_saveQueue.IsEmpty)
                        {
                            var upsertMessages = new List<SocketMessage>();
                            SocketMessage socketMessage;
                            while (_saveQueue.TryDequeue(out socketMessage))
                            {
                                if (socketMessage != null)
                                    upsertMessages.Add(socketMessage);
                            }

                            if (upsertMessages.Count > 0)
                            {
                                var startMillisec = sw.ElapsedMilliseconds;

                                //[1] Update List and Dictionary in Memory
                                _dataLock.EnterWriteLock();
                                try
                                {
                                    foreach (var upsert in upsertMessages)
                                    {
                                        if (string.IsNullOrEmpty(upsert.Key)) continue;

                                        SocketMessage existing;
                                        if (!_allDictInFile.TryGetValue(upsert.Key, out existing))
                                        {
                                            // New
                                            _allListInFile.Add(upsert);
                                            _allDictInFile[upsert.Key] = upsert;
                                        }
                                        else
                                        {
                                            // Update
                                            existing.Update(upsert);
                                        }
                                    }

                                    LastUpdateTime = DateTime.Now;
                                }
                                finally
                                {
                                    _dataLock.ExitWriteLock();
                                }

                                var listUpdateTimeMs = (int)(sw.ElapsedMilliseconds - startMillisec);
                                if (listUpdateTimeMs > 0)
                                {
                                    var total = upsertMessages.Count;
                                    var ips = total * 1000 / listUpdateTimeMs;
                                    debugLines.Add($"upsertCount: {total} Item/Sec: {ips} ({listUpdateTimeMs}ms)");
                                }

                                //[2] Update DB (Upsert / DeleteMany + InsertBulk avoid)
                                try
                                {
                                    debugLines.Add($"{DateTime.Now:HH:mm:ss.fff} upsertBulk");
                                    liteCollection.Upsert(upsertMessages);
                                    debugLines.Add($"{DateTime.Now:HH:mm:ss.fff} upsertFinish");
                                    LastUpdateTime = DateTime.Now;
                                }
                                catch (Exception exDb)
                                {
                                    debugLines.Add($"{GetType().Name}::Worker DB upsert exception: {exDb}");
                                }

                                if (debugLines.Count > 0)
                                {
                                    Debug.WriteLine(string.Join(Environment.NewLine, debugLines));
                                    debugLines.Clear();
                                }
                            }
                        }

                        // Refresh List from DB
                        if (_refreshFlag)
                        {
                            try
                            {
                                var list = liteCollection.Query().ToList();

                                _dataLock.EnterWriteLock();
                                try
                                {
                                    _allListInFile.Clear();
                                    _allListInFile.AddRange(list);

                                    _allDictInFile.Clear();
                                    foreach (var m in list)
                                    {
                                        if (!string.IsNullOrEmpty(m.Key))
                                        {
                                            _allDictInFile[m.Key] = m;
                                        }
                                    }

                                    LastUpdateTime = DateTime.Now;
                                }
                                finally
                                {
                                    _dataLock.ExitWriteLock();
                                }
                            }
                            catch (Exception exRef)
                            {
                                Debug.WriteLine($"{GetType().Name}::Worker Refresh exception: {exRef}");
                            }
                            finally
                            {
                                _refreshFlag = false;
                                _refreshCompletedEvent.Set();
                            }
                        }

                        // Move old data to backup
                        if ((DateTime.Now - LastBackupTime).TotalSeconds >= BackupIntervalSecond)
                        {
                            List<SocketMessage> removeDataList = null;
                            try
                            {
                                removeDataList = BreakupLightDB_byMonthFile();
                            }
                            catch (Exception exBk)
                            {
                                Debug.WriteLine($"{GetType().Name}::Worker Breakup exception: {exBk}");
                            }

                            if (removeDataList != null && removeDataList.Count > 0)
                            {
                                try
                                {
                                    var deleteKeys = new HashSet<string>(removeDataList.Select(x => x.Key));
                                    liteCollection.DeleteMany(x => deleteKeys.Contains(x.Key));

                                    litedb.Rebuild();
                                    LastBackupTime = DateTime.Now;
                                }
                                catch (Exception exDbBk)
                                {
                                    Debug.WriteLine($"{GetType().Name}::Worker Backup DB exception: {exDbBk}");
                                }

                                try
                                {
                                    var backupBuildFilename = Path.Combine(
                                        Path.GetDirectoryName(_liteDbConnectionString.Filename),
                                        Path.GetFileNameWithoutExtension(_liteDbConnectionString.Filename) + "-backup" +
                                        Path.GetExtension(_liteDbConnectionString.Filename));

                                    if (!string.IsNullOrEmpty(backupBuildFilename) && File.Exists(backupBuildFilename))
                                    {
                                        File.Delete(backupBuildFilename);
                                    }
                                }
                                catch (Exception exFile)
                                {
                                    Debug.WriteLine($"{GetType().Name}::Worker Backup file delete exception: {exFile}");
                                }
                            }
                        }

                        // Interval wait
                        sw.Stop();
                        var remainingTime = Interval - (int)sw.ElapsedMilliseconds;
                        if (remainingTime > 0)
                        {
                            try
                            {
                                Task.Delay(remainingTime, token).GetAwaiter().GetResult();
                            }
                            catch (OperationCanceledException)
                            {
                                // Cancel ... process finish
                            }
                        }
                        else
                        {
                            Debug.WriteLine($"{DateTime.Now:HH:mm:ss.fff} workerIntervalOver {remainingTime}ms");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (debugLines.Count > 0)
                        {
                            Debug.WriteLine(string.Join(Environment.NewLine, debugLines));
                            debugLines.Clear();
                        }
                        Debug.WriteLine($"{GetType().Name}::Worker exception: {ex}");
                    }
                }
            }
        }

        public List<SocketMessage> BreakupLightDB_byMonthFile()
        {
            var removeDataList = new List<SocketMessage>();
            var sw = new Stopwatch();
            sw.Start();

            var timeSpan = TimeSpan.FromMinutes(StoreTimeRangeMinute);

            List<SocketMessage> breakupTargetList;
            _dataLock.EnterReadLock();
            try
            {
                var now = DateTime.Now;
                breakupTargetList = _allListInFile
                    .Where(x => x.connectTime < now - timeSpan)
                    .OrderBy(x => x.connectTime)
                    .ToList();
            }
            finally
            {
                _dataLock.ExitReadLock();
            }

            if (breakupTargetList.Count < 1)
            {
                return removeDataList;
            }

            var targetFirstTime = breakupTargetList.First().connectTime;
            var targetLastTime = breakupTargetList.Last().connectTime;

            var fileStartTime = new DateTime(targetFirstTime.Year, targetFirstTime.Month, 1);
            var fileEndTime = fileStartTime.AddMonths(1);

            do
            {
                var backupFileString = new ConnectionString
                {
                    Connection = ConnectionType.Direct,
                    Filename = Path.Combine(
                        BackupDirTopPath,
                        fileStartTime.ToString("yyyy"),
                        fileStartTime.ToString("yyyyMM") + ".db")
                };

                var backupFileDir = Path.GetDirectoryName(backupFileString.Filename);
                if (!string.IsNullOrEmpty(backupFileDir) && !Directory.Exists(backupFileDir))
                {
                    Directory.CreateDirectory(backupFileDir);
                }

                var filteredBreakupTargetList = breakupTargetList
                    .Where(x => x.connectTime >= fileStartTime && x.connectTime < fileEndTime)
                    .ToList();

                if (filteredBreakupTargetList.Count == 0)
                {
                    fileStartTime = fileEndTime;
                    fileEndTime = fileStartTime.AddMonths(1);
                    continue;
                }

                try
                {
                    using (var litedbBackup = new LiteDatabase(backupFileString))
                    {
                        var colbk = litedbBackup.GetCollection<SocketMessage>(TableName);
                        var storedMonthDataList = colbk.Query().ToList();

                        Debug.WriteLine($"OpenLiteDB\t{GetType().Name}::{System.Reflection.MethodBase.GetCurrentMethod().Name} Filename: {Path.GetFileName(backupFileString.Filename)} sw: {sw.ElapsedMilliseconds}");

                        try
                        {
                            var storedKeySet = new HashSet<string>(storedMonthDataList.Select(x => x.Key));

                            var monthInsertList = new List<SocketMessage>();
                            var monthUpdateList = new List<SocketMessage>();

                            foreach (var skm in filteredBreakupTargetList)
                            {
                                if (string.IsNullOrEmpty(skm.Key)) continue;

                                if (!storedKeySet.Contains(skm.Key))
                                {
                                    monthInsertList.Add(skm);
                                }
                                else
                                {
                                    monthUpdateList.Add(skm);
                                }
                            }

                            if (monthInsertList.Count > 0)
                            {
                                colbk.InsertBulk(monthInsertList);
                            }

                            if (monthUpdateList.Count > 0)
                            {
                                var deleteKeys = new HashSet<string>(monthUpdateList.Select(x => x.Key));
                                colbk.DeleteMany(x => deleteKeys.Contains(x.Key));
                                colbk.InsertBulk(monthUpdateList);
                            }

                            removeDataList.AddRange(filteredBreakupTargetList);
                        }
                        catch (Exception exOp)
                        {
                            Debug.WriteLine($"litedbBackup file operation exception ... {GetType().Name}::{System.Reflection.MethodBase.GetCurrentMethod().Name} {exOp}");
                        }
                    }

                    // remove data from memory
                    if (removeDataList.Count > 0)
                    {
                        _dataLock.EnterWriteLock();
                        try
                        {
                            var removeKeySet = new HashSet<string>(removeDataList.Select(x => x.Key));

                            _allListInFile.RemoveAll(x => removeKeySet.Contains(x.Key));
                            foreach (var key in removeKeySet)
                            {
                                _allDictInFile.Remove(key);
                            }

                            LastUpdateTime = DateTime.Now;
                        }
                        finally
                        {
                            _dataLock.ExitWriteLock();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"litedbBackup file open exception ... {GetType().Name}::{System.Reflection.MethodBase.GetCurrentMethod().Name} {ex}");
                }

                fileStartTime = fileEndTime;
                fileEndTime = fileStartTime.AddMonths(1);

            } while (fileStartTime < targetLastTime);

            sw.Stop();
            Debug.WriteLine($"litedb Update ... {GetType().Name}::{System.Reflection.MethodBase.GetCurrentMethod().Name} sw = {sw.ElapsedMilliseconds}");

            return removeDataList;
        }
    }
}