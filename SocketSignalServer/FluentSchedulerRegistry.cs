using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Diagnostics;

using FluentScheduler;
using LiteDB;

namespace SocketSignalServer
{
    public class FluentSchedulerRegistry_FromScheduleLines : Registry
    {
        private List<ClientInfo> clientList;
        public List<string> ScheduleList;
        private LiteDB_Worker liteDB_Worker;

        private List<FluentSchedulerJob_SchedulerLineRun> jobList;

        private int Name_idx = 0;
        private int Unit_idx = 1;
        private int At_idx = 2;

        public FluentSchedulerRegistry_FromScheduleLines(LiteDB_Worker liteDB_Worker, NoticeTransmitter noticeTransmitter, string[] Lines, List<ClientInfo> clientList)
        {
            this.ScheduleList = new List<string>();
            this.jobList = new List<FluentSchedulerJob_SchedulerLineRun>();

            this.liteDB_Worker = liteDB_Worker;
            this.clientList = clientList;

            foreach (string Line in Lines)
            {
                string[] cols = Line.Split('\t');

                string TargetStatusName = cols[Name_idx];
                string IntervalUnitString = cols[Unit_idx];
                string IntervalParam = cols[At_idx];

                FluentSchedulerRegistry(liteDB_Worker, noticeTransmitter, TargetStatusName, IntervalUnitString, IntervalParam, clientList);
            }
        }

        public FluentSchedulerRegistry_FromScheduleLines(LiteDB_Worker liteDB_Worker, NoticeTransmitter noticeTransmitter, string TargetStatusName, string IntervalUnitString, string IntervalParam, List<ClientInfo> clientList)
        {
            FluentSchedulerRegistry(liteDB_Worker, noticeTransmitter, TargetStatusName, IntervalUnitString, IntervalParam, clientList);
        }

        private void FluentSchedulerRegistry(LiteDB_Worker liteDB_Worker, NoticeTransmitter noticeTransmitter, string TargetStatusName, string IntervalUnitString, string IntervalParam, List<ClientInfo> clientList)
        {
            string Line = TargetStatusName + "\t" + IntervalUnitString + "\t" + IntervalParam;

            if (IntervalUnitString == "EveryDays")
            {
                try
                {
                    string[] atinfo = IntervalParam.Split(',');
                    foreach (string t in atinfo)
                    {
                        int[] hm = Array.ConvertAll(t.Split(':'), s => int.Parse(s));
                        int h = hm[0];
                        int m = hm[1];

                        var job = new FluentSchedulerJob_SchedulerLineRun(liteDB_Worker, noticeTransmitter, TargetStatusName, new TimeSpan(24, 0, 0), clientList);
                        Schedule(job.Execute()).WithName(TargetStatusName).ToRunEvery(1).Days().At(h, m);
                        ScheduleList.Add("EveryDays at " + t);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ex.ToString());
                    ScheduleList.Add("ERROR: " + Line);
                }
            }
            else if (IntervalUnitString == "EveryHours")
            {
                try
                {
                    int[] atinfo = Array.ConvertAll(IntervalParam.Split(','), s => int.Parse(s));
                    foreach (int m in atinfo)
                    {
                        var job = new FluentSchedulerJob_SchedulerLineRun(liteDB_Worker, noticeTransmitter, TargetStatusName, new TimeSpan(1, 0, 0), clientList);
                        Schedule(job.Execute()).WithName(TargetStatusName).ToRunEvery(1).Hours().At(m);
                        ScheduleList.Add("EveryHours at " + m.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ex.ToString());
                    ScheduleList.Add("ERROR: " + Line);
                }
            }
            else if (IntervalUnitString == "EverySeconds")
            {
                try
                {
                    int[] atinfo = Array.ConvertAll(IntervalParam.Split(','), s => int.Parse(s));
                    foreach (int s in atinfo)
                    {
                        var job = new FluentSchedulerJob_SchedulerLineRun(liteDB_Worker, noticeTransmitter, TargetStatusName, new TimeSpan(0, 0, s), clientList);
                        Schedule(job.Execute()).WithName(TargetStatusName).ToRunEvery(s).Seconds();
                        ScheduleList.Add("EverySeconds at " + s.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ex.ToString());
                    ScheduleList.Add("ERROR: " + Line);
                }
            }
        }
    }

    public class FluentSchedulerJob_SchedulerLineRun
    {
        public string TargetStatusName;
        public DateTime LastRunTime;

        private LiteDB_Worker liteDB_Worker;
        private NoticeTransmitter noticeTransmitter;
        private List<ClientInfo> clientList;

        public FluentSchedulerJob_SchedulerLineRun(LiteDB_Worker liteDB_Worker, NoticeTransmitter noticeTransmitter, string TargetStatusName, TimeSpan jobInterval, List<ClientInfo> clientList)
        {
            this.liteDB_Worker = liteDB_Worker;
            this.noticeTransmitter = noticeTransmitter;
            this.TargetStatusName = TargetStatusName;

            this.LastRunTime = DateTime.Now - jobInterval;
            this.clientList = clientList;
        }

        public Action Execute()
        {
            return new Action(delegate ()
            {
                try
                {
                    SocketMessage[] dataset;
                    SocketMessage[] datasetOnce;
                    List<SocketMessage> records = new List<SocketMessage>();

                    dataset = liteDB_Worker.LoadData()
                            .Where(x => x.status == TargetStatusName && !x.check)
                                      .OrderByDescending(x => x.connectTime).ToArray();
                    datasetOnce = liteDB_Worker.LoadData()
                            .Where(x => x.status == TargetStatusName && !x.check && x.checkStyle == "Once").ToArray();

                    foreach (var targetClient in clientList)
                    {
                        //get Latest unchecked message 
                        var latestTargetClientRecord_haveTargetStatusName
                                = dataset.Where(x => x.clientName == targetClient.Name).FirstOrDefault();

                        if (latestTargetClientRecord_haveTargetStatusName != null)
                        {
                            noticeTransmitter.AddNotice(targetClient, latestTargetClientRecord_haveTargetStatusName);
                        }

                        //style==Once Message check update
                        records.AddRange(datasetOnce.Where(x => x.clientName == targetClient.Name).ToList());
                    }

                    try
                    {
                        foreach (var record in records)
                        {
                            record.check = true;
                            liteDB_Worker.SaveData(record);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Write(GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name);
                        Debug.WriteLine(ex.ToString());
                    }

                    LastRunTime = DateTime.Now;

                }
                catch (Exception ex)
                {
                    Debug.Write(GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name);
                    Debug.WriteLine(ex.ToString());
                }
            });
        }


    }
}
