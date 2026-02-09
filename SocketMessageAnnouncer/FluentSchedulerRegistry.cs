using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentScheduler;
using System.Diagnostics;

namespace tcpClient
{
    public class FluentSchedulerRegistry : Registry
    {
        public TcpSocketClient Tcp { get; }
        public List<string> ScheduleList { get; }
        public List<FluentSchedulerJob> ScheduleJobList { get; }

        public FluentSchedulerRegistry(TcpSocketClient tcp, string[] lines)
        {
            Tcp = tcp;
            ScheduleList = new List<string>();
            ScheduleJobList = new List<FluentSchedulerJob>();

            foreach (var line in lines)
            {
                try
                {
                    var param = new FluentSchedulerJobParam(tcp, line);
                    RegisterJob(param);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Registry::Constructor Error: {ex}");
                    ScheduleList.Add("ERROR: " + line);
                }
            }
        }

        private void RegisterJob(FluentSchedulerJobParam param)
        {
            switch (param.ScheduleUnit)
            {
                case "EveryDays":
                    RegisterDaily(param);
                    break;

                case "EveryHours":
                    RegisterHourly(param);
                    break;

                case "EverySeconds":
                    RegisterSeconds(param);
                    break;

                case "OnceAtSeconds":
                    RegisterOnce(param, TimeUnit.Seconds);
                    break;

                case "OnceAtMinutes":
                    RegisterOnce(param, TimeUnit.Minutes);
                    break;

                case "OnceAtHours":
                    RegisterOnce(param, TimeUnit.Hours);
                    break;

                default:
                    ScheduleList.Add("ERROR: Unknown ScheduleUnit: " + param.ScheduleUnit);
                    break;
            }
        }

        private void RegisterDaily(FluentSchedulerJobParam param)
        {
            try
            {
                foreach (var t in param.ScheduleUnitParam.Split(','))
                {
                    if (!TryParseHM(t, out int h, out int m))
                    {
                        ScheduleList.Add("ERROR: Invalid time " + t);
                        continue;
                    }

                    var job = new FluentSchedulerJob(param);
                    Schedule(job.Execute())
                        .WithName(param.ToString())
                        .ToRunEvery(1).Days().At(h, m);

                    ScheduleList.Add("EveryDays at " + t);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"RegisterDaily Error: {ex}");
                ScheduleList.Add("ERROR: " + param.ToString());
            }
        }

        private void RegisterHourly(FluentSchedulerJobParam param)
        {
            try
            {
                foreach (var s in SplitIntList(param.ScheduleUnitParam))
                {
                    var job = new FluentSchedulerJob(param);
                    Schedule(job.Execute())
                        .WithName(param.ToString())
                        .ToRunEvery(1).Hours().At(s);

                    ScheduleList.Add("EveryHours at " + s);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"RegisterHourly Error: {ex}");
                ScheduleList.Add("ERROR: " + param.ToString());
            }
        }

        private void RegisterSeconds(FluentSchedulerJobParam param)
        {
            try
            {
                foreach (var s in SplitIntList(param.ScheduleUnitParam))
                {
                    var job = new FluentSchedulerJob(param);
                    Schedule(job.Execute())
                        .WithName(param.ToString())
                        .ToRunEvery(s).Seconds();

                    ScheduleList.Add("EverySeconds at " + s);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"RegisterSeconds Error: {ex}");
                ScheduleList.Add("ERROR: " + param.ToString());
            }
        }

        private void RegisterOnce(FluentSchedulerJobParam param, TimeUnit unit)
        {
            try
            {
                foreach (var s in SplitIntList(param.ScheduleUnitParam))
                {
                    var job = new FluentSchedulerJob(param);

                    var schedule = Schedule(job.Execute()).WithName(param.ToString());

                    switch (unit)
                    {
                        case TimeUnit.Seconds: schedule.ToRunOnceIn(s).Seconds(); break;
                        case TimeUnit.Minutes: schedule.ToRunOnceIn(s).Minutes(); break;
                        case TimeUnit.Hours: schedule.ToRunOnceIn(s).Hours(); break;
                    }

                    ScheduleList.Add("Once at " + s);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"RegisterOnce Error: {ex}");
                ScheduleList.Add("ERROR: " + param.ToString());
            }
        }

        private bool TryParseHM(string text, out int h, out int m)
        {
            h = m = 0;
            var parts = text.Split(':');
            if (parts.Length != 2) return false;
            return int.TryParse(parts[0], out h) && int.TryParse(parts[1], out m);
        }

        private IEnumerable<int> SplitIntList(string text)
        {
            foreach (var s in text.Split(','))
            {
                if (int.TryParse(s, out int v))
                    yield return v;
            }
        }

        private enum TimeUnit { Seconds, Minutes, Hours }
    }


    public class FluentSchedulerJobParam
    {
        public Form1 form1;
        public TcpSocketClient tcp;

        public string AddressPortSet = "";

        public string JobName = "";
        public string ScheduleUnit = "";
        public string ScheduleUnitParam = "";
        public string ClientName = "";
        public string Status = "";
        public string Message = "";
        public string Parameter = "";
        public string CheckStyle = "Once";
        public DateTime createTime;

        public FluentSchedulerJobParam(TcpSocketClient tcp, string addressPortSet, string JobName, string scheduleUnit, string scheduleUnitParam, string clientName, string status, string message, string parameter, string CheckStyle)
        {
            this.tcp = tcp;

            this.AddressPortSet = addressPortSet;

            this.JobName = JobName;
            this.ScheduleUnit = scheduleUnit;
            this.ScheduleUnitParam = scheduleUnitParam;
            this.ClientName = clientName;
            this.Status = status;
            this.Message = message;
            this.Parameter = parameter;
            this.CheckStyle = CheckStyle;

            createTime = DateTime.Now;

        }

        public FluentSchedulerJobParam(TcpSocketClient tcp, string Line)
        {
            string[] cols = Line.Split('\t');

            this.tcp = tcp;
            int i = 0;

            this.AddressPortSet = cols[i]; i++;

            this.JobName = cols[i]; i++;
            this.ScheduleUnit = cols[i]; i++;
            this.ScheduleUnitParam = cols[i]; i++;
            this.ClientName = cols[i]; i++;
            this.Status = cols[i]; i++;
            this.Message = cols[i]; i++;
            this.Parameter = cols[i]; i++;
            this.CheckStyle = cols[i];

            createTime = DateTime.Now;

        }

        public override string ToString()
        {
            List<string> Cols = new List<string>();

            Cols.Add(this.JobName);
            Cols.Add(this.AddressPortSet);
            Cols.Add(this.ScheduleUnit);
            Cols.Add(this.ScheduleUnitParam);
            Cols.Add(this.ClientName);
            Cols.Add(this.Status);
            Cols.Add(this.Message);
            Cols.Add(this.Parameter);
            Cols.Add(this.CheckStyle.ToString());
            Cols.Add(this.createTime.ToString("yyyy/MM/dd HH:mm:ss.fff"));

            return string.Join("\t", Cols.ToArray());
        }
    }

    public class FluentSchedulerJob
    {
        public FluentSchedulerJobParam Param { get; }
        public string Response { get; private set; } = "";

        public FluentSchedulerJob(FluentSchedulerJobParam param)
        {
            Param = param;
        }

        public Action Execute()
        {
            return async () =>
            {
                try
                {
                    string sendMessage =
                        $"{Param.ClientName}\t{Param.Status}\t{Param.Message}\t{Param.Parameter}\t{Param.CheckStyle}";

                    var addressList = Param.AddressPortSet.Trim('/').Split('/');
                    var tasks = new List<Task<string>>();

                    foreach (var line in addressList)
                    {
                        var cols = line.Split(':');
                        if (cols.Length == 2 && int.TryParse(cols[1], out int port))
                        {
                            tasks.Add(Param.tcp.StartClient(cols[0], port, sendMessage, "UTF8"));
                        }
                    }

                    var results = await Task.WhenAll(tasks);
                    Response = string.Join("\n", results);

                    if (Param.ScheduleUnit.Contains("Once"))
                    {
                        JobManager.RemoveJob(Param.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"FluentSchedulerJob.Execute Error: {ex}");
                }
            };
        }
    }

}
