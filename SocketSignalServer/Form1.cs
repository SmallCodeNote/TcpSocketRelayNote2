using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using WinFormStringCnvClass;
using FluentScheduler;

namespace SocketSignalServer
{
    public partial class Form1 : Form
    {
        private string thisExeDirPath;
        List<ClientInfo> clientList;
        NoticeTransmitter noticeTransmitter;

        public Form1()
        {
            InitializeComponent();
            thisExeDirPath = Path.GetDirectoryName(Application.ExecutablePath);

            JobManager.Initialize();

            noticeTransmitter = new NoticeTransmitter(checkBox_voiceOffSwitch.Checked);
            noticeTransmitter.Start();

            clientList = new List<ClientInfo>();

            icon_voiceON = Properties.Resources.VoiceON048;
            icon_voiceOFF = Properties.Resources.VoiceOFF048;

            tokenSource = new CancellationTokenSource();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string paramFilename = Path.Combine(thisExeDirPath, "_param.txt");

            if (File.Exists(paramFilename))
            {
                WinFormStringCnv.setControlFromString(this, File.ReadAllText(paramFilename));
            }

            RefreshFailoverSystemView();
            AddressListInitialize();
            ClientListInitialize();

            if (WorkersDirectoryCheckAndCreate()) return;

            //CreateWorkers
            liteDB_Worker = new LiteDB_Worker(textBox_DataBaseFilePath.Text);
            debugFilePathWorker = new DebugFilePathWorker(textBox_DebugOutDirPath.Text);

            SchedulerInitialize();

            WorkersAutoStartTry();

            UpdateStart_toolStripStatusLabel1(tokenSource.Token);
            UpdateStart_StatusList(tokenSource.Token);
            UpdateStart_panel_FailoverSystemView(tokenSource.Token);
            UpdateStart_dataGridView_AddressList(tokenSource.Token);

            checkBox_voiceOffSwitch_CheckedChanged(null, null);
        }

        private bool WorkersDirectoryCheckAndCreate()
        {
            if (!Directory.Exists(Path.GetDirectoryName(textBox_DataBaseFilePath.Text))) { Directory.CreateDirectory(Path.GetDirectoryName(textBox_DataBaseFilePath.Text)); }
            if (!Directory.Exists(textBox_DebugOutDirPath.Text)) { Directory.CreateDirectory(textBox_DebugOutDirPath.Text); }

            return (!Directory.Exists(Path.GetDirectoryName(textBox_DataBaseFilePath.Text))
                 || !Directory.Exists(textBox_DebugOutDirPath.Text));
        }


        //===================
        // Member variable
        //===================
        private LiteDB_Worker liteDB_Worker;
        private DebugFilePathWorker debugFilePathWorker;
        private TimeoutCheckWorker timeoutCheckWorker;
        private SocketListeningAndStoreWorker socketListeningAndStoreWorker;
        private CancellationTokenSource tokenSource;

        private DestinationsBook addressBook;

        Bitmap icon_voiceON;
        Bitmap icon_voiceOFF;

        private void AddressListInitialize()
        {
            ButtonEnable(button_AddressListLoad, false);

            List<string> addressList = new List<string>();
            for (int i = 0; i < dataGridView_AddressList.RowCount - 1; i++)
            {
                var cells = dataGridView_AddressList.Rows[i].Cells;
                string code = cells[0].Value.ToString();
                code += cells.Count > 1 && cells[1].Value != null ? "\t" + cells[1].Value.ToString() : "\t";

                if (code != "") addressList.Add(code);
            }

            addressBook = new DestinationsBook(addressList.ToArray());

            if (int.TryParse(textBox_httpTimeout.Text, out int httpTimeout))
            {
                noticeTransmitter.HttpTimeout = httpTimeout;
            }
        }



        private void ClientListInitialize()
        {
            clientList.Clear();

            for (int i = 0; i < dataGridView_ClientList.RowCount - 1; i++)
            {
                try
                {
                    var cells = dataGridView_ClientList.Rows[i].Cells;

                    string clientName = cells[0].Value.ToString();
                    string addressKeys = cells[1].Value.ToString();
                    string timeoutCheck = cells[2].Value.ToString();
                    string timeoutLength = cells[3].Value.ToString();
                    string timeoutMessage = cells[4].Value.ToString();

                    string code = clientName + "\t" + timeoutCheck + "\t" + timeoutLength + "\t" + timeoutMessage;

                    ClientInfo cd = new ClientInfo(code, addressBook.getDestinations(addressKeys));

                    if (cd.Name != "") clientList.Add(cd);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + " EX:" + ex.ToString());
                }
            }
            ButtonEnable(button_ClientListLoad, false);
        }

        private void SchedulerInitialize()
        {
            JobManager.StopAndBlock();
            JobManager.RemoveAllJobs();

            List<string> Lines = new List<string>();
            for (int i = 0; i < dataGridView_SchedulerList.RowCount - 1; i++)
            {
                var cells = dataGridView_SchedulerList.Rows[i].Cells;
                string code = cells[0].Value.ToString();
                code += cells.Count > 1 && cells[1].Value != null ? "\t" + cells[1].Value.ToString() : "\t";
                code += cells.Count > 2 && cells[2].Value != null ? "\t" + cells[2].Value.ToString() : "\t";
                code += cells.Count > 3 && cells[3].Value != null ? "\t" + cells[3].Value.ToString() : "\t";

                if (code != "") Lines.Add(code);
            }
            var job = new FluentSchedulerRegistry_FromScheduleLines(liteDB_Worker, noticeTransmitter, Lines.ToArray(), clientList);
            JobManager.Initialize(job);
            ButtonEnable(button_SchedulerList, false);
        }

        private bool WorkersStart()
        {
            try
            {
                button_Start.Text = "ServerStop";

                if (int.TryParse(textBox_portNumber.Text, out int Port))
                {
                    if (socketListeningAndStoreWorker == null) socketListeningAndStoreWorker = new SocketListeningAndStoreWorker(liteDB_Worker, Port);
                    if (!socketListeningAndStoreWorker.IsBusy) socketListeningAndStoreWorker.Start();
                }

                if (int.TryParse(textBox_TimeoutCheckInterval.Text, out int TimeoutCheckInterval))
                {
                    if (timeoutCheckWorker == null) timeoutCheckWorker = new TimeoutCheckWorker(liteDB_Worker, noticeTransmitter, clientList, textBox_TimeoutMessageParameter.Text);
                    if (!timeoutCheckWorker.IsBusy) timeoutCheckWorker.Start();
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + " EX:" + ex.ToString());
                return false;
            }

            button_Start.Text = "ServerStop";
            return true;
        }

        private bool WorkersStop()
        {
            socketListeningAndStoreWorker.Stop();
            timeoutCheckWorker.Stop();
            button_Start.Text = "ServerStart";
            return true;
        }

        private void WorkersAutoStartTry()
        {
            if (checkBox_serverAutoStart.Checked)
            {
                WorkersStart();
            }
            else
            {
                tabPage_Status.Select();
            }
        }
        private void UpdateStart_panel_FailoverSystemView(CancellationToken token)
        {
            Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    if (!int.TryParse(textBox_FailoverActiveListCheckInterval.Text, out int checkInterval)) checkInterval = 10;
                    try
                    {
                        Update_panel_FailoverSystemView();
                        Task.Delay(TimeSpan.FromSeconds(checkInterval), token).Wait();
                    }
                    catch { }
                }
            }, token);
        }

        private void UpdateStart_dataGridView_AddressList(CancellationToken token)
        {
            int checkInterval = 10;

            Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    dataGridView_AddressList_InfoUpdate();
                    Task.Delay(TimeSpan.FromSeconds(checkInterval), token).Wait();
                }
            }, token);
        }

        public void dataGridView_AddressList_InfoUpdate()
        {
            if (this.InvokeRequired) { this.Invoke((Action)(() => dataGridView_AddressList_InfoUpdate())); }
            else
            {
                for (int i = 0; i < dataGridView_AddressList.RowCount - 1; i++)
                {
                    var cells = dataGridView_AddressList.Rows[i].Cells;
                    string AddressString = cells[0].Value.ToString();

                    if (noticeTransmitter.FailLogDictionary.ContainsKey(AddressString))
                    {
                        bool bEnable = button_AddressListLoad.Enabled;
                        cells[2].Value = "TimeOut[" +
                            noticeTransmitter.FailCountDictionary[AddressString].ToString() + "]: " +
                            noticeTransmitter.FailLogDictionary[AddressString].SendNoticeTime.ToString("MM/dd HH:mm:ss");

                        button_AddressListLoad.Enabled = bEnable;
                    }
                    else
                    {
                        cells[2].Value = "--";

                    }
                }
            }
        }

        private void Update_panel_FailoverSystemView()
        {
            foreach (FailoverActiveView ctrl in panel_FailoverSystemView.Controls)
            {
                ctrl.askAlive();
            }

            bool primaryAlive = false;

            foreach (FailoverActiveView ctrl in panel_FailoverSystemView.Controls)
            {
                primaryAlive = primaryAlive || ctrl.Alive;
            }

            noticeTransmitter.isActive = !primaryAlive;

            Update_StatusStrip(primaryAlive);
        }

        private void Update_StatusStrip(bool primaryAlive)
        {
            if (this.InvokeRequired) { this.Invoke((Action)(() => Update_StatusStrip(primaryAlive))); }
            else
            {
                if (primaryAlive)
                {
                    toolStripStatusLabel2.Text = "StandbyMode(PrimaryAlive)";
                    toolStripDropDownButton_Class.Image = Properties.Resources.Standby048;
                }
                else if (!primaryAlive && panel_FailoverSystemView.Controls.Count > 0)
                {
                    toolStripStatusLabel2.Text = "PrimaryMode(PrimaryDown)";
                    toolStripDropDownButton_Class.Image = Properties.Resources.Active048;
                }
                else
                {
                    toolStripStatusLabel2.Text = "PrimaryMode";
                    toolStripDropDownButton_Class.Image = Properties.Resources.Active048;
                }
            }
        }

        private void Update_toolStripDropDownButton_VoiceSwitch()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => Update_toolStripDropDownButton_VoiceSwitch()));
            }
            else
            {
                if (noticeTransmitter.voiceOFF)
                {
                    toolStripDropDownButton_VoiceSwitch.Image = icon_voiceOFF;
                }
                else
                {
                    toolStripDropDownButton_VoiceSwitch.Image = icon_voiceON;
                }
            }
        }


        private void UpdateStart_toolStripStatusLabel1(CancellationToken token)
        {
            Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        UpdateStart_toolStripStatusLabel1();
                        Task.Delay(TimeSpan.FromMilliseconds(1000), token).Wait();
                    }
                    catch { }
                }
            }, token);
        }

        private void UpdateStart_toolStripStatusLabel1()
        {
            if (this == null || toolStripStatusLabel1 == null) return;

            if (this.InvokeRequired) { this.Invoke((Action)(() => UpdateStart_toolStripStatusLabel1())); }
            else
            {
                if (socketListeningAndStoreWorker == null)
                {
                    toolStripStatusLabel1.Text = "Standby";
                }
                else
                {
                    TimeSpan dt = DateTime.Now - socketListeningAndStoreWorker.LastCheckTime;
                    string info = socketListeningAndStoreWorker.LastCheckTime.ToString("yyyy/MM/dd HH:mm:ss") + " / " + getElapsedTimeString(dt);
                    toolStripStatusLabel1.Text = info;
                }
            }
        }

        public string getElapsedTimeString(TimeSpan elapsedTime)
        {
            if (elapsedTime.TotalDays >= 365) { return (elapsedTime.TotalDays / 365.2425).ToString("0") + " year"; }
            if (elapsedTime.TotalDays >= 30) { return (elapsedTime.TotalDays / 30.436875).ToString("0") + " month"; }
            if (elapsedTime.TotalDays >= 7) { return (elapsedTime.TotalDays / 7).ToString("0") + " week"; }
            if (elapsedTime.TotalDays >= 1) { return (elapsedTime.TotalDays / 7).ToString("0") + " day"; }
            if (elapsedTime.TotalHours >= 1) { return (elapsedTime.TotalHours).ToString("0") + " hour"; }
            if (elapsedTime.TotalMinutes >= 1) { return (elapsedTime.TotalMinutes).ToString("0") + " min."; }
            if (elapsedTime.TotalSeconds >= 1) { return (elapsedTime.TotalSeconds).ToString("0") + " sec."; }
            return "now";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            tokenSource.Cancel();

            for (int RowCount = 0; RowCount < dataGridView_AddressList.Rows.Count - 1; RowCount++)
            {
                dataGridView_AddressList.Rows[RowCount].Cells[2].Value = "--";
            }

            string FormContents = WinFormStringCnv.ToString(this);
            string paramFilename = Path.Combine(thisExeDirPath, "_param.txt");
            File.WriteAllText(paramFilename, FormContents);
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            toolStripLabel1.Text = this.Size.ToString();
        }

        private void button_CreateDammyData_Click(object sender, EventArgs e)
        {
            TimeSpan TP = new TimeSpan(0, 8, 0, 0);

            string dbFilename = textBox_DataBaseFilePath.Text;
            if (!File.Exists(dbFilename)) return;
            for (DateTime connectTime = DateTime.Today - TimeSpan.FromDays(1000); connectTime < DateTime.Today; connectTime += TP)
            {
                SocketMessage socketMessage = new SocketMessage(connectTime, "Test", "Test", "Test", "parameterTest");

                liteDB_Worker.SaveData(socketMessage);
            }
        }

        private void button_LoadDataTest_Click(object sender, EventArgs e)
        {
            textBox_LiteDBLoadTestResult.Text = string.Join("\r\n", liteDB_Worker.LoadData().Select(x => x.ToString()).ToArray());
        }

        private void button_Start_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox_portNumber.Text, out int portNumber))
            { textBox_portNumber.Select(); return; }

            if (button_Start.Text == "ServerStart")
            {
                WorkersStart();
            }
            else
            {
                WorkersStop();
            }
        }

        private void UpdateStart_StatusList(CancellationToken token)
        {
            Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        updateStatusTab();
                        Task.Delay(TimeSpan.FromMilliseconds(1000), token).Wait();
                    }
                    catch { }
                }
            }, token);
        }

        private void updateStatusTab()
        {
            try
            {
                List<SocketMessage> colQuery = liteDB_Worker.LoadData();
                List<SocketMessage> dataset;

                dataset = colQuery.OrderByDescending(x => x.connectTime).ToList();

                foreach (MessageItemView messageItemView in panel_StatusList.Controls)
                {
                    string clientName = messageItemView.groupBox_ClientName.Text;
                    var query = dataset.Where(x => x.clientName == clientName).ToList();
                    if (query.Count() > 0)
                    {
                        SocketMessage socketMessage = query.First();
                        messageItemView.socketMessage = socketMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write(GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name);
                Debug.WriteLine(ex.ToString());
            }
        }

        private void tabPage_Status_Enter(object sender, EventArgs e)
        {
            int ClientCount = dataGridView_ClientList.Rows.Count - 1;
            panel_StatusList.Height = ClientCount * 100;

            if (panel_StatusList.Controls.Count != ClientCount)
            {
                clearControlCollection(panel_StatusList.Controls);

                int TopBuff = 0;
                for (int i = 0; i < ClientCount; i++)
                {
                    MessageItemView messageItemView = new MessageItemView();
                    panel_StatusList.Controls.Add(messageItemView);
                    panel_StatusList.Controls[i].Top = TopBuff;
                    ((MessageItemView)panel_StatusList.Controls[i]).clientName = dataGridView_ClientList.Rows[i].Cells[0].Value.ToString();

                    TopBuff += panel_StatusList.Controls[i].Height;
                }
                updateStatusTab();
            }
        }

        private void clearControlCollection(System.Windows.Forms.Control.ControlCollection cc)
        {
            for (int i = 0; i < cc.Count; i++) { cc[i].Dispose(); }
            cc.Clear();
        }

        private void DeleteFailoverSystemView(int targetIndex)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => DeleteFailoverSystemView(targetIndex)));
            }
            else
            {
                if (targetIndex < panel_FailoverSystemView.Controls.Count)
                {
                    panel_FailoverSystemView.Controls.RemoveAt(targetIndex);
                    UpdateFailoverSystemText();
                    UpdateLayoutFailoverSystemView();
                }
            }
        }

        private void RefreshFailoverSystemView()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => RefreshFailoverSystemView()));
            }
            else
            {
                panel_FailoverSystemView.Controls.Clear();
                panel_FailoverSystemView.Height = 0;

                string[] Lines = textBox_FailoverActiveList.Text.Replace("\r\n", "\n").Trim('\n').Split('\n');
                int PositionTop = 0;
                int ctrlIndex = 0;


                foreach (var Line in Lines)
                {
                    if (Line == "") continue;
                    var ctrl = new FailoverActiveView(ctrlIndex, Line);
                    ctrl.DeleteThis = (Action<int>)((int x) => DeleteFailoverSystemView(x));
                    ctrl.LoadThis = (Action)(() => EnableLoadFailoverSystemView());
                    ctrl.Top = PositionTop;
                    PositionTop += ctrl.Height;
                    panel_FailoverSystemView.Controls.Add(ctrl);

                    ctrlIndex++;
                }
                panel_FailoverSystemView.Height = PositionTop;
            }
        }

        private void UpdateFailoverSystemText()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => UpdateFailoverSystemText()));
            }
            else
            {
                List<string> Lines = new List<string>();
                foreach (FailoverActiveView View in panel_FailoverSystemView.Controls)
                {
                    Lines.Add(View.ToString());
                }

                textBox_FailoverActiveList.Text = string.Join("\r\n", Lines.ToArray());
            }
        }

        private void UpdateLayoutFailoverSystemView()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => UpdateLayoutFailoverSystemView()));
            }
            else
            {
                panel_FailoverSystemView.Height = 0;
                int PositionTop = 0;
                int ctrlIndex = 0;
                foreach (FailoverActiveView ctrl in panel_FailoverSystemView.Controls)
                {
                    ctrl.Top = PositionTop;
                    ctrl.Index = ctrlIndex;
                    PositionTop += ctrl.Height;

                    ctrlIndex++;
                }
                panel_FailoverSystemView.Height = PositionTop;
            }
        }

        private void EnableLoadFailoverSystemView()
        {
            ButtonEnable(button_LoadFailoverSystemView, true);
        }

        private void ButtonEnable(Button button, bool enable)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => ButtonEnable(button, enable)));
            }
            else
            {
                if (enable)
                {
                    button.Enabled = true;
                    button.BackColor = Color.GreenYellow;
                }
                else
                {
                    button.Enabled = false;
                    button.BackColor = Color.Transparent;
                }
            }
        }

        private void button_AddFailoverActiveList_Click(object sender, EventArgs e)
        {
            int ctrlIndex = panel_FailoverSystemView.Controls.Count;
            FailoverActiveView ctrl = new FailoverActiveView(ctrlIndex);
            ctrl.DeleteThis = (Action<int>)((int x) => DeleteFailoverSystemView(ctrlIndex));
            ctrl.LoadThis = (Action)(() => EnableLoadFailoverSystemView());
            ctrl.Top = panel_FailoverSystemView.Height;

            panel_FailoverSystemView.Controls.Add(ctrl);
            panel_FailoverSystemView.Height += ctrl.Height;
            UpdateFailoverSystemText();

        }

        private void button_LoadFailoverSystemView_Click(object sender, EventArgs e)
        {
            UpdateFailoverSystemText();
            if (int.TryParse(textBox_FailoverActiveListCheckInterval.Text, out int b))
            {
                //timer_FailoverActiveListUpdate.Interval = b * 1000;
            }
            ButtonEnable(button_LoadFailoverSystemView, false);
        }

        private void checkBox_voiceOffSwitch_CheckedChanged(object sender, EventArgs e)
        {
            noticeTransmitter.voiceOFF = checkBox_voiceOffSwitch.Checked;
            Update_toolStripDropDownButton_VoiceSwitch();
        }

        private void button_AddressListLoad_Click(object sender, EventArgs e)
        {
            AddressListInitialize();
        }

        private void button_ClientListLoad_Click(object sender, EventArgs e)
        {
            ClientListInitialize();

            if (int.TryParse(textBox_TimeoutCheckInterval.Text, out int TimeoutCheckInterval))
            {
                timeoutCheckWorker.Interval = TimeoutCheckInterval * 1000;
            }
        }

        private void button_SchedulerList_Click(object sender, EventArgs e)
        {
            SchedulerInitialize();
        }

        private void label_IntervalSelector_Click(object sender, EventArgs e)
        {
            string clip = Clipboard.GetText();

            if (clip == "EveryDays") { Clipboard.SetText("EveryHours"); }
            else if (clip == "EveryHours") { Clipboard.SetText("EverySeconds"); }
            else { Clipboard.SetText("EveryDays"); }

            label_IntervalSelectorNow.Text = Clipboard.GetText();
        }

        private void tabControl_FailoverSystemView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl_FailoverSystemView.SelectedTab == tabPage_FailoverSystemText)
            {
                UpdateFailoverSystemText();
            }
            else if (tabControl_FailoverSystemView.SelectedTab == tabPage_FailoverSystemView)
            {

            }
        }

        private void toolStripDropDownButton_VoiceSwitch_Click(object sender, EventArgs e)
        {
            if (toolStripDropDownButton_VoiceSwitch.Image == icon_voiceOFF)
            {
                checkBox_voiceOffSwitch.Checked = false;
            }
            else
            {
                checkBox_voiceOffSwitch.Checked = true;
            }
        }

        private void dataGridView_ClientList_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            button_ClientListLoad.Enabled = true;
            button_ClientListLoad.BackColor = Color.GreenYellow;
        }

        private void dataGridView_AddressList_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            ButtonEnable(button_AddressListLoad, true);
        }

        private void textBox_httpTimeout_TextChanged(object sender, EventArgs e)
        {
            ButtonEnable(button_AddressListLoad, true);
        }

        private void dataGridView_SchedulerList_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            ButtonEnable(button_SchedulerList, true);
        }

    }
}
