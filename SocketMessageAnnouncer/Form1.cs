using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using FluentScheduler;

using WinFormStringCnvClass;
using tcpServer;

namespace tcpClient
{
    public partial class Form1 : Form
    {
        string thisExeDirPath;
        TcpSocketClient tcp;
        TcpSocketServer tcp_Reset;

        public Form1()
        {
            InitializeComponent();
            thisExeDirPath = Path.GetDirectoryName(Application.ExecutablePath);

            tcp = new TcpSocketClient();
            tcp_Reset = new TcpSocketServer();

            tokenSource_ClientViewUpdate = new CancellationTokenSource();
            tokenSource_CommandPortListening = new CancellationTokenSource();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string paramFilename = Path.Combine(thisExeDirPath, "_param.txt");
            if (File.Exists(paramFilename))
            {
                WinFormStringCnv.setControlFromString(this, File.ReadAllText(paramFilename));
            }

            if (checkBox_LoadFromStoreAuto.Checked) { button_LoadFromStore_Click(null, null); };

            if (checkBox_EnableReset.Checked)
            {
                if (!int.TryParse(textBox_ResetPortNumber.Text, out int portNumber))
                {
                    tcp_Reset.Start(portNumber, "UTF8");
                }
            }
            UpdateStart_CommandPortListening();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (tcp_Reset != null) tcp_Reset.Stop();

            string FormContents = WinFormStringCnv.ToString(this);
            string paramFilename = Path.Combine(thisExeDirPath, "_param.txt");
            File.WriteAllText(paramFilename, FormContents);

            UpdateStop_ClientView();
            tokenSource_ClientViewUpdate.Dispose();

            tokenSource_CommandPortListening.Dispose();
        }


        private CancellationTokenSource tokenSource_ClientViewUpdate;

        int JobManager_AllSchedules_Count_Buff = 0;
        private Task UpdateTask_ClientView;
        private void UpdateStart_ClientView()
        {
            CancellationToken token = tokenSource_ClientViewUpdate.Token;
            int checkInterval = 1;//sec.

            UpdateTask_ClientView = Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        int jobCount = JobManager.AllSchedules.Count();
                        if (JobManager_AllSchedules_Count_Buff != jobCount)
                        {
                            JobManager_AllSchedules_Count_Buff = jobCount;
                            tabPage_View_Enter();
                        }

                        toolStripStatusLabel_Timer_Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    }
                    catch { }
                    Task.Delay(TimeSpan.FromSeconds(checkInterval), token).Wait();
                }
            }, token);
        }

        private void UpdateStop_ClientView()
        {
            tokenSource_ClientViewUpdate.Cancel();
        }

        private bool ClientLocked = false;
        private CancellationTokenSource tokenSource_CommandPortListening;
        private Task UpdateTask_CommandPortListening;
        private void UpdateStart_CommandPortListening()
        {
            DateTime LastCheckTime = DateTime.Now;
            DateTime UnlockTime = DateTime.Now;
            CancellationToken token = tokenSource_CommandPortListening.Token;
            int checkInterval = 1;//sec.

            UpdateTask_CommandPortListening = Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        if (checkBox_EnableReset.Checked)
                        {
                            //============
                            // New data check from Queue
                            //============
                            if ((tcp_Reset.LastReceiveTime - LastCheckTime).TotalSeconds > 0 && tcp_Reset.ReceivedSocketQueue.Count > 0)
                            {
                                List<string> queueList = new List<string>();

                                string receivedSocketMessage = "";

                                //============
                                // ReadQueue 
                                // ex.1) Reset\t5
                                // ex.2) AddJob\tAddress
                                //============
                                while (tcp_Reset.ReceivedSocketQueue.TryDequeue(out receivedSocketMessage))
                                {
                                    string[] cols = receivedSocketMessage.Split('\t');

                                    int cindex = 0;
                                    string command = cols[cindex]; cindex++;

                                    if (command == "Reset" && cols.Length == 2)
                                    {
                                        if (int.TryParse(cols[1], out int timeSpanMinute))
                                        {
                                            UnlockTime = DateTime.Now + TimeSpan.FromMinutes(timeSpanMinute);
                                        }
                                    }
                                    else if (command == "AddJob" && cols.Length == 2)
                                    {
                                        string Address = cols[1];
                                        SchedulerInitializeFromArray(cols, 2);
                                    }
                                }

                                toolStripStatusLabel_TimerReset.Text = "ResetRecieved Lockuntil" + UnlockTime.ToString("HH:mm:ss");
                            }
                            else if (!ClientLocked)
                            {
                                toolStripStatusLabel_TimerReset.Text = "Client Unlock";
                            }

                            if (tcp_Reset.IsBusy)
                            {
                                toolStripStatusLabel_TimerReset.Text += " / ListeningRun";
                            }
                            else
                            {
                                toolStripStatusLabel_TimerReset.Text += " / ListeningStop";

                            }
                        }
                    }
                    catch { }

                    ClientLocked = ((TimeSpan)(UnlockTime - DateTime.Now)).TotalSeconds >= 0;

                    Task.Delay(TimeSpan.FromSeconds(checkInterval), token).Wait();
                }
            }, token);
        }

        List<JobItemView> jobItemViews = new List<JobItemView>();
        private void updateJobViewList()
        {
            try
            {
                panel_JobViewList.Controls.Clear();

                List<Schedule> sortedSchedules = JobManager.AllSchedules.OrderBy(schedule => schedule.NextRun).ToList();
                int topLocation = 0;

                foreach (var schedule in sortedSchedules)
                {
                    List<string> Cols = new List<string>();

                    var jobItemView = new JobItemView();
                    jobItemView.Top = topLocation;
                    jobItemView.Left = 0;
                    jobItemView.setLabel(schedule, this);
                    panel_JobViewList.Controls.Add(jobItemView);
                    topLocation += jobItemView.Height;
                }

                panel_JobViewList.Height = topLocation;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + " EX:" + ex.ToString());
            }
        }

        
        private string toolStripStatusLabel_Timer_Text
        {
            get { return toolStripStatusLabel_Timer.Text; }
            set
            {
                if (this.InvokeRequired) { this.Invoke((Action)(() => toolStripStatusLabel_Timer_Text = value)); }
                else
                {
                    toolStripStatusLabel_Timer.Text = value;
                }
            }
        }

        private void SchedulerInitializeFromArray(string[] cols, int startIndex = 0)
        {
            SchedulerInitialize(getJobStringFromArray(cols, startIndex));
        }

        public string getJobStringFromArray(string[] cols, int startImdex = 0)
        {
            int colsIndex = startImdex;
            string Address = cols[colsIndex]; colsIndex++;
            string PortNumber = cols[colsIndex]; colsIndex++;

            string JobName = cols[colsIndex]; colsIndex++;
            string ScheduleUnit = cols[colsIndex]; colsIndex++;
            string ScheduleUnitParam = cols[colsIndex]; colsIndex++;
            string ClientName = cols[colsIndex]; colsIndex++;
            string Status = cols[colsIndex]; colsIndex++;
            string Message = cols[colsIndex]; colsIndex++;
            string Parameter = cols[colsIndex]; colsIndex++;
            string CheckStyle = cols[colsIndex]; colsIndex++;

            List<string> ColList = new List<string>();
            ColList.Add(Address);
            ColList.Add(PortNumber);

            ColList.Add(JobName);
            ColList.Add(ScheduleUnit);
            ColList.Add(ScheduleUnitParam);
            ColList.Add(ClientName);
            ColList.Add(Status);
            ColList.Add(Message);
            ColList.Add(Parameter);
            ColList.Add(CheckStyle.ToString());

            return String.Join("\t", ColList.ToArray());
        }

        public string getJobStringFromEditControls()
        {
            string AddressPortSetting = textBox_AddressPortSetting.Text;

            string JobName = textBox_JobName.Text;
            string ScheduleUnit = comboBox_ScheduleUnit.Text;
            string ScheduleUnitParam = textBox_ScheduleUnitParam.Text;
            string ClientName = textBox_ClientName.Text;
            string Status = comboBox_Status.Text;
            string Message = textBox_Message.Text;
            string Parameter = textBox_Parameter.Text;
            string CheckStyle = comboBox_checkStyle.Text;

            List<string> ColList = new List<string>();
            ColList.Add(AddressPortSetting);

            ColList.Add(JobName);
            ColList.Add(ScheduleUnit);
            ColList.Add(ScheduleUnitParam);
            ColList.Add(ClientName);
            ColList.Add(Status);
            ColList.Add(Message);
            ColList.Add(Parameter);
            ColList.Add(CheckStyle.ToString());

            return String.Join("\t", ColList.ToArray());
        }

        private void SchedulerInitializeFromForm()
        {
            SchedulerInitialize(getJobStringFromEditControls());
        }

        private void SchedulerInitialize(string jobString)
        {
            List<string> Lines = new List<string>();
            Lines.Add(jobString);

            var job = new FluentSchedulerRegistry(tcp, Lines.ToArray());
            JobManager.Initialize(job);

            string ScheduleUnit = comboBox_ScheduleUnit.Text;

            if (ScheduleUnit.IndexOf("Once") < 0)
            {
                textBox_Store.Text += string.Join("\r\n", Lines.ToArray()) + "\r\n";
            }
        }

        private void update_OnceJobPanel()
        {
            string[] Lines = textBox_OnceJobPanelStore.Text.Replace("\r\n", "\n").Trim('\n').Split('\n');

            panel_OnceJobList.Controls.Clear();

            int viewTop = 0;
            foreach (var Line in Lines)
            {
                string[] Cols = Line.Split('\t');
                if (Cols.Length == 9)
                {
                    var onceJobView = new OnceJobView(tcp, Cols, this);
                    onceJobView.Top = viewTop;
                    panel_OnceJobList.Controls.Add(onceJobView);
                    panel_OnceJobList.Width = onceJobView.Width;

                    viewTop += onceJobView.Height;
                }
            }
            panel_OnceJobList.Height = viewTop;
        }

        private void textBox_OnceJobPanelStore_TextChanged(object sender, EventArgs e)
        {
            update_OnceJobPanel();
        }

        private void button_StatusListFrame_Sort_Click(object sender, EventArgs e)
        {
            updateJobViewList();
        }

        private void textBox_ScheduleUnitParam_TextChanged(object sender = null, EventArgs e = null)
        {
            TextBox textBox = textBox_ScheduleUnitParam;

            if (comboBox_ScheduleUnit.Text.IndexOf("Day") >= 0)
            {
                string pattern = @"^([01]?[0-9]|2[0-3]):[0-5][0-9]$";
                string[] times = new string[] { textBox.Text };// textBox.Text.Split(',');

                foreach (string time in times)
                {
                    if (!Regex.IsMatch(time, pattern))
                    {
                        textBox.ForeColor = Color.Red;
                        button_AddOnceJobPanel.Enabled = false;
                        button_AddSchedule.Enabled = false;
                        return;
                    }
                }
                textBox.ForeColor = Color.Black;
            }
            else
            {
                string pattern = @"^\d+$";
                string[] times = new string[] { textBox.Text };// textBox.Text.Split(',');

                foreach (string time in times)
                {
                    if (!Regex.IsMatch(time, pattern))
                    {
                        textBox.ForeColor = Color.Red;
                        button_AddOnceJobPanel.Enabled = false;
                        button_AddSchedule.Enabled = false;
                        return;
                    }
                }
                textBox.ForeColor = Color.Black;
            }
            button_AddOnceJobPanel.Enabled = comboBox_ScheduleUnit.Text.IndexOf("Once") >= 0;
            button_AddSchedule.Enabled = true;
        }

        private void textBox_ResetPortNumber_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(textBox_ResetPortNumber.Text, out int Port))
            {

            }
        }

        private void toolTip_textBox_ScheduleUnitParam_Popup(object sender, PopupEventArgs e)
        {

        }

        private void button_SchedulerList_Click(object sender, EventArgs e)
        {
            SchedulerInitializeFromForm();
        }

        private void button_SendMessage_Click(object sender, EventArgs e)
        {
            string sendMessage = textBox_ClientName.Text + "\t" + comboBox_Status.Text + "\t" + textBox_Message.Text + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "\t" + textBox_Parameter.Text + "\t" + comboBox_checkStyle.Text;

            string[] AddresSet = textBox_AddressPortSetting.Text.Trim('/').Split('/');
            List<string> responce = new List<string>();
            List<Task<string>> task = new List<Task<string>>();

            foreach (var AddressLine in AddresSet)
            {
                string[] Cols = AddressLine.Split(':');
                if (Cols.Length > 1)
                {
                    string address = Cols[0];
                    int port = int.Parse(Cols[1]);

                    task.Add(tcp.StartClient(address, port, sendMessage, "UTF8"));
                }
            }

            foreach (var t in task)
            {
                responce.Add(t.Result);
            }

            label_Return.Text = string.Join("/", responce.ToArray());
        }

        private void button_AddSchedule_Click(object sender, EventArgs e)
        {
            SchedulerInitializeFromForm();
        }

        public void tabPage_View_Enter(object sender = null, EventArgs e = null)
        {
            if (sender != null) UpdateStart_ClientView();

            var allSchedules = JobManager.AllSchedules;

            List<string> scheduleList = new List<string>();

            foreach (var schedule in allSchedules)
            {
                List<string> Cols = new List<string>();

                Cols.Add(schedule.Name);
                Cols.Add(schedule.NextRun.Second.ToString());
                Cols.Add(schedule.ToString());

                scheduleList.Add(string.Join("\t", Cols.ToArray()));
            }
            updateJobViewList();
        }

        private void tabPage_View_Leave(object sender, EventArgs e)
        {
            UpdateStop_ClientView();
        }

        private void button_LoadFromStore_Click(object sender, EventArgs e)
        {
            string[] Lines = textBox_Store.Text.Replace("\r\n", "\n").Trim('\n').Split('\n');
            var job = new FluentSchedulerRegistry(tcp, Lines);
            JobManager.RemoveAllJobs();
            JobManager.Initialize(job);
        }

        private void button_EditContentsFromClipboard_Click(object sender, EventArgs e)
        {
            string Line = Clipboard.GetText();
            string[] Cols = Line.Split('\t');

            if (Cols.Length == 10)
            {
                int colIndex = 0;
                textBox_AddressPortSetting.Text = Cols[colIndex]; colIndex++;

                textBox_JobName.Text = Cols[colIndex]; colIndex++;
                comboBox_ScheduleUnit.Text = Cols[colIndex]; colIndex++;
                textBox_ScheduleUnitParam.Text = Cols[colIndex]; colIndex++;
                textBox_ClientName.Text = Cols[colIndex]; colIndex++;
                comboBox_Status.Text = Cols[colIndex]; colIndex++;
                textBox_Message.Text = Cols[colIndex]; colIndex++;
                textBox_Parameter.Text = Cols[colIndex]; colIndex++;
                comboBox_checkStyle.Text = Cols[colIndex];
            }
        }

        private void comboBox_ScheduleUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            button_AddOnceJobPanel.Enabled = comboBox_ScheduleUnit.Text.IndexOf("Once") >= 0;

            //toolTip Update
            if (comboBox_ScheduleUnit.Text == "EveryDays") { toolTip_textBox_ScheduleUnitParam.SetToolTip(textBox_ScheduleUnitParam, "Hour and Minute value(ex. 8:15 )"); }
            else if (comboBox_ScheduleUnit.Text == "EveryHours") { toolTip_textBox_ScheduleUnitParam.SetToolTip(textBox_ScheduleUnitParam, "Minute value(ex. 10)"); }
            else if (comboBox_ScheduleUnit.Text == "EverySeconds") { toolTip_textBox_ScheduleUnitParam.SetToolTip(textBox_ScheduleUnitParam, "Interval in Seconds"); }
            else if (comboBox_ScheduleUnit.Text == "OnceAtSeconds") { toolTip_textBox_ScheduleUnitParam.SetToolTip(textBox_ScheduleUnitParam, "Delay time value in Seconds"); }
            else if (comboBox_ScheduleUnit.Text == "OnceAtMinutes") { toolTip_textBox_ScheduleUnitParam.SetToolTip(textBox_ScheduleUnitParam, "Delay time value in Minutes"); }
            else if (comboBox_ScheduleUnit.Text == "OnceAtHours") { toolTip_textBox_ScheduleUnitParam.SetToolTip(textBox_ScheduleUnitParam, "Delay time value in Hours"); }

            textBox_ScheduleUnitParam_TextChanged();
        }

        private void button_AddOnceJobPanel_Click(object sender, EventArgs e)
        {
            textBox_OnceJobPanelStore.Text += getJobStringFromEditControls() + "\r\n";
        }
    }
}
