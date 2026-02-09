using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FluentScheduler;

namespace SocketSignalServer
{
    public class FluentSchedulerRegistry_FromScheduleLines : Registry
    {
        private readonly List<ClientInfo> clientList;
        public List<string> ScheduleList { get; }
        private readonly LiteDB_Worker liteDB_Worker;

        private const int Name_idx = 0;
        private const int Unit_idx = 1;
        private const int At_idx = 2;

        public FluentSchedulerRegistry_FromScheduleLines(
            LiteDB_Worker liteDB_Worker,
            NoticeTransmitter noticeTransmitter,
            string[] lines,
            List<ClientInfo> clientList)
        {
            ScheduleList = new List<string>();
            this.liteDB_Worker = liteDB_Worker;
            this.clientList = clientList;

            if (lines == null || lines.Length == 0)
            {
                ScheduleList.Add("ERROR: No schedule lines");
                return;
            }

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    ScheduleList.Add("ERROR: Empty line");
                    continue;
                }

                var cols = line.Split('\t');
                if (cols.Length < 3)
                {
                    ScheduleList.Add("ERROR: Invalid format → " + line);
                    continue;
                }

                RegisterSchedule(
                    noticeTransmitter,
                    cols[Name_idx],
                    cols[Unit_idx],
                    cols[At_idx]);
            }
        }

        public FluentSchedulerRegistry_FromScheduleLines(
            LiteDB_Worker liteDB_Worker,
            NoticeTransmitter noticeTransmitter,
            string targetStatusName,
            string intervalUnitString,
            string intervalParam,
            List<ClientInfo> clientList)
        {
            ScheduleList = new List<string>();
            this.liteDB_Worker = liteDB_Worker;
            this.clientList = clientList;

            RegisterSchedule(
                noticeTransmitter,
                targetStatusName,
                intervalUnitString,
                intervalParam);
        }

        private void RegisterSchedule(
            NoticeTransmitter noticeTransmitter,
            string targetStatusName,
            string intervalUnitString,
            string intervalParam)
        {
            string line = $"{targetStatusName}\t{intervalUnitString}\t{intervalParam}";

            try
            {
                switch (intervalUnitString)
                {
                    case "EveryDays":
                        RegisterDaily(noticeTransmitter, targetStatusName, intervalParam);
                        break;

                    case "EveryHours":
                        RegisterHourly(noticeTransmitter, targetStatusName, intervalParam);
                        break;

                    case "EverySeconds":
                        RegisterSeconds(noticeTransmitter, targetStatusName, intervalParam);
                        break;

                    default:
                        ScheduleList.Add($"ERROR: Unknown interval unit '{intervalUnitString}' → {line}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{GetType().Name}::{nameof(RegisterSchedule)} {ex}");
                ScheduleList.Add("ERROR: " + line);
            }
        }

        // ============================
        // Daily
        // ============================
        private void RegisterDaily(
            NoticeTransmitter noticeTransmitter,
            string targetStatusName,
            string intervalParam)
        {
            if (string.IsNullOrWhiteSpace(intervalParam))
            {
                ScheduleList.Add("ERROR: Empty time list");
                return;
            }

            foreach (var t in intervalParam.Split(','))
            {
                if (!TryParseHourMinute(t, out int h, out int m))
                {
                    ScheduleList.Add("ERROR: Invalid time → " + t);
                    continue;
                }

                try
                {
                    var job = new FluentSchedulerJob_SchedulerLineRun(
                        liteDB_Worker, noticeTransmitter, targetStatusName,
                        TimeSpan.FromDays(1), clientList);

                    Schedule(job.Execute())
                        .WithName(targetStatusName)
                        .ToRunEvery(1).Days().At(h, m);

                    ScheduleList.Add("EveryDays at " + t);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("RegisterDaily: " + ex);
                    ScheduleList.Add("ERROR: EveryDays → " + t);
                }
            }
        }

        // ============================
        // Hourly
        // ============================
        private void RegisterHourly(
            NoticeTransmitter noticeTransmitter,
            string targetStatusName,
            string intervalParam)
        {
            if (string.IsNullOrWhiteSpace(intervalParam))
            {
                ScheduleList.Add("ERROR: Empty minute list");
                return;
            }

            foreach (var mStr in intervalParam.Split(','))
            {
                if (!int.TryParse(mStr, out int m))
                {
                    ScheduleList.Add("ERROR: Invalid minute → " + mStr);
                    continue;
                }

                try
                {
                    var job = new FluentSchedulerJob_SchedulerLineRun(
                        liteDB_Worker, noticeTransmitter, targetStatusName,
                        TimeSpan.FromHours(1), clientList);

                    Schedule(job.Execute())
                        .WithName(targetStatusName)
                        .ToRunEvery(1).Hours().At(m);

                    ScheduleList.Add("EveryHours at " + m);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("RegisterHourly: " + ex);
                    ScheduleList.Add("ERROR: EveryHours → " + mStr);
                }
            }
        }

        // ============================
        // Seconds
        // ============================
        private void RegisterSeconds(
            NoticeTransmitter noticeTransmitter,
            string targetStatusName,
            string intervalParam)
        {
            if (string.IsNullOrWhiteSpace(intervalParam))
            {
                ScheduleList.Add("ERROR: Empty seconds list");
                return;
            }

            foreach (var sStr in intervalParam.Split(','))
            {
                if (!int.TryParse(sStr, out int s))
                {
                    ScheduleList.Add("ERROR: Invalid seconds → " + sStr);
                    continue;
                }

                try
                {
                    var job = new FluentSchedulerJob_SchedulerLineRun(
                        liteDB_Worker, noticeTransmitter, targetStatusName,
                        TimeSpan.FromSeconds(s), clientList);

                    Schedule(job.Execute())
                        .WithName(targetStatusName)
                        .ToRunEvery(s).Seconds();

                    ScheduleList.Add("EverySeconds at " + s);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("RegisterSeconds: " + ex);
                    ScheduleList.Add("ERROR: EverySeconds → " + sStr);
                }
            }
        }

        // ============================
        // Utility
        // ============================
        private bool TryParseHourMinute(string text, out int h, out int m)
        {
            h = m = 0;
            var hm = text.Split(':');
            return hm.Length == 2 &&
                   int.TryParse(hm[0], out h) &&
                   int.TryParse(hm[1], out m);
        }
    }

    
    // ============================================================
    // Job
    // ============================================================
    public class FluentSchedulerJob_SchedulerLineRun
    {
        public string TargetStatusName { get; private set; }
        public DateTime LastRunTime { get; private set; }

        private readonly LiteDB_Worker liteDB_Worker;
        private readonly NoticeTransmitter noticeTransmitter;
        private readonly List<ClientInfo> clientList;

        bool detect_null_Clientname = false;

        public FluentSchedulerJob_SchedulerLineRun(
            LiteDB_Worker liteDB_Worker,
            NoticeTransmitter noticeTransmitter,
            string targetStatusName,
            TimeSpan jobInterval,
            List<ClientInfo> clientList)
        {
            this.liteDB_Worker = liteDB_Worker;
            this.noticeTransmitter = noticeTransmitter;
            this.TargetStatusName = targetStatusName;
            this.clientList = clientList;

            LastRunTime = DateTime.Now - jobInterval;
        }

        public Action Execute()
        {
            return () =>
            {
                try
                {
                    var allData = liteDB_Worker.LoadData();
                    if (allData == null || allData.Count == 0)
                        return;

                    // Pre-filter
                    var uncheckedData = allData
                        .Where(x => x.status == TargetStatusName && !x.check)
                        .ToArray();

                    if (uncheckedData.Length == 0)
                        return;

                    var onceData = uncheckedData
                        .Where(x => x.checkStyle == "Once")
                        .ToArray();


                    // clientName null detection
                    var invalidRecords = uncheckedData.Where(x => string.IsNullOrEmpty(x.clientName)).ToArray();
                    if (!detect_null_Clientname && invalidRecords.Length > 0)
                    {
                        Debug.WriteLine($"WARNING: {invalidRecords.Length} records have null or empty clientName.");
                        detect_null_Clientname = true;
                    }
                    else
                    {
                        detect_null_Clientname = false;
                    }


                    // Group by clientName → latest record
                    var latestByClient = uncheckedData
                        .Where(x => !string.IsNullOrEmpty(x.clientName))
                        .GroupBy(x => x.clientName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.OrderByDescending(x => x.connectTime).First()
                        );


                    var recordsToCheck = new HashSet<SocketMessage>();

                    foreach (var client in clientList)
                    {
                        if (client == null) continue;

                        // Latest message
                        if (latestByClient.TryGetValue(client.Name, out var latestRecord))
                        {
                            try
                            {
                                noticeTransmitter.AddNotice(client, latestRecord);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"AddNotice error for client {client.Name}: {ex}");
                            }
                        }

                        // Once messages
                        foreach (var msg in onceData.Where(x => x.clientName == client.Name))
                        {
                            recordsToCheck.Add(msg);
                        }
                    }

                    // Update check flag
                    foreach (var record in recordsToCheck)
                    {
                        try
                        {
                            record.check = true;
                            liteDB_Worker.SaveData(record);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"SaveData error for record (client={record.clientName}, status={record.status}): {ex}");
                        }
                    }

                    LastRunTime = DateTime.Now;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{GetType().Name}::{nameof(Execute)} {ex}");
                }
            };
        }
    }
}