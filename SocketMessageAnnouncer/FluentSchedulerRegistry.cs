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
        public TcpSocketClient tcp;
        public List<string> ScheduleList;
        public string DataBaseFilePath;

        public List<FluentSchedulerJob> ScheduleJobList;

        public string[] ScheduleUnitList = new string[] { "Once", "EveryDays", "EveryHours", "EverySeconds" };


        public FluentSchedulerRegistry(TcpSocketClient tcp, string[] Lines)
        {
            ScheduleList = new List<string>();
            this.tcp = tcp;
            ScheduleJobList = new List<FluentSchedulerJob>();

            foreach (string Line in Lines)
            {
                try
                {
                    FluentSchedulerJobParam param = new FluentSchedulerJobParam(tcp, Line);

                    if (param.ScheduleUnit == "EveryDays")
                    {
                        try
                        {
                            string[] atinfo = param.ScheduleUnitParam.Split(',');
                            foreach (string t in atinfo)
                            {
                                int[] hm = Array.ConvertAll(t.Split(':'), s => int.Parse(s));
                                int h = hm[0];
                                int m = hm[1];

                                FluentSchedulerJob job = new FluentSchedulerJob(param);
                                Schedule(job.Execute()).WithName(param.ToString()).ToRunEvery(1).Days().At(h, m);

                                ScheduleList.Add("EveryDays at " + t);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ex.ToString());

                            ScheduleList.Add("ERROR: " + Line);
                        }

                    }
                    else if (param.ScheduleUnit == "EveryHours")
                    {
                        try
                        {
                            int[] atinfo = Array.ConvertAll(param.ScheduleUnitParam.Split(','), s => int.Parse(s));
                            foreach (int m in atinfo)
                            {
                                FluentSchedulerJob job = new FluentSchedulerJob(param);
                                Schedule(job.Execute()).WithName(param.ToString()).ToRunEvery(1).Hours().At(m);
                                ScheduleList.Add("EveryHours at " + m.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ex.ToString());

                            ScheduleList.Add("ERROR: " + Line);
                        }

                    }
                    else if (param.ScheduleUnit == "EverySeconds")
                    {
                        try
                        {
                            int[] atinfo = Array.ConvertAll(param.ScheduleUnitParam.Split(','), s => int.Parse(s));
                            foreach (int s in atinfo)
                            {
                                FluentSchedulerJob job = new FluentSchedulerJob(param);
                                Schedule(job.Execute()).WithName(param.ToString()).ToRunEvery(s).Seconds();
                                ScheduleList.Add("EverySeconds at " + s.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ex.ToString());

                            ScheduleList.Add("ERROR: " + Line);
                        }

                    }
                    else if (param.ScheduleUnit == "OnceAtSeconds")
                    {
                        try
                        {
                            int[] atinfo = Array.ConvertAll(param.ScheduleUnitParam.Split(','), s => int.Parse(s));
                            foreach (int s in atinfo)
                            {
                                FluentSchedulerJob job = new FluentSchedulerJob(param);
                                Schedule(job.Execute()).WithName(param.ToString()).ToRunOnceIn(s).Seconds();
                                ScheduleList.Add("Once at " + s.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ex.ToString());

                            ScheduleList.Add("ERROR: " + Line);
                        }

                    }
                    else if (param.ScheduleUnit == "OnceAtMinutes")
                    {
                        try
                        {
                            int[] atinfo = Array.ConvertAll(param.ScheduleUnitParam.Split(','), s => int.Parse(s));
                            foreach (int s in atinfo)
                            {
                                FluentSchedulerJob job = new FluentSchedulerJob(param);
                                Schedule(job.Execute()).WithName(param.ToString()).ToRunOnceIn(s).Minutes();
                                ScheduleList.Add("Once at " + s.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ex.ToString());

                            ScheduleList.Add("ERROR: " + Line);
                        }

                    }
                    else if (param.ScheduleUnit == "OnceAtHours")
                    {
                        try
                        {
                            int[] atinfo = Array.ConvertAll(param.ScheduleUnitParam.Split(','), s => int.Parse(s));
                            foreach (int s in atinfo)
                            {
                                FluentSchedulerJob job = new FluentSchedulerJob(param);
                                Schedule(job.Execute()).WithName(param.ToString()).ToRunOnceIn(s).Hours();
                                ScheduleList.Add("Once at " + s.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ex.ToString());

                            ScheduleList.Add("ERROR: " + Line);
                        }

                    }
                }
                catch { }
            }

        }

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
        public FluentSchedulerJobParam param;
        public string Responce { get; private set; } = "";

        public FluentSchedulerJob(FluentSchedulerJobParam param)
        {
            this.param = param;
        }

        public override string ToString()
        {
            return param.ToString();
        }

        public FluentSchedulerJob(TcpSocketClient tcp, string addressPortSet, string jobname, string scheduleUnit, string clientName, string status, string message, string parameter, string CheckStyle)
        {

            param.tcp = tcp;

            param.AddressPortSet = addressPortSet;

            param.JobName = jobname;
            param.ScheduleUnit = scheduleUnit;
            param.ClientName = clientName;
            param.Status = status;
            param.Message = message;
            param.Parameter = parameter;
            param.CheckStyle = CheckStyle;

        }

        public Action Execute()
        {
            return new Action(delegate ()
            {
                string sendMessage = param.ClientName + "\t" + param.Status + "\t" + param.Message + "\t"
                + param.Parameter + "\t" + param.CheckStyle.ToString();

                string[] AddresSet = param.AddressPortSet.Trim('/').Split('/');
                List<string> responce = new List<string>();
                List<Task<string>> task = new List<Task<string>>();

                foreach (var AddressLine in AddresSet)
                {
                    string[] Cols = AddressLine.Split(':');
                    if (Cols.Length > 1)
                    {
                        string address = Cols[0];
                        int port = int.Parse(Cols[1]);

                        task.Add(param.tcp.StartClient(address, port, sendMessage, "UTF8"));
                    }
                }

                foreach (var t in task)
                {
                    responce.Add(t.Result);
                }

                if (param.ScheduleUnit.IndexOf("Once") >= 0)
                {
                    JobManager.RemoveJob(param.ToString());
                    int jobCount = 1;
                    int timeout = 10;
                    do
                    {
                        System.Threading.Thread.Sleep(10);
                        jobCount = JobManager.AllSchedules.Where((x) => x.Name == param.ToString()).ToArray().Length;
                        timeout--;
                    } while (jobCount > 0 && timeout > 0);

                }
            });
        }
    }
}
