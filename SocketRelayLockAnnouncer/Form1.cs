using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;
using System.IO;
using WinFormStringCnvClass;

using tcpServer;
using tcpClient;

namespace tcpClient_HTTPcheck
{
    public partial class Form1 : Form
    {
        //===================
        // Constructor
        //===================
        public Form1()
        {
            InitializeComponent();
            tokenSource = new CancellationTokenSource();
            thisExeDirPath = Path.GetDirectoryName(Application.ExecutablePath);
            tcp_srv = new TcpSocketServer();
            tcp_clt = new TcpSocketClient();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "TEXT|*.txt";
            if (false && ofd.ShowDialog() == DialogResult.OK)
            {
                WinFormStringCnv.setControlFromString(this, File.ReadAllText(ofd.FileName));
            }
            else
            {
                string paramFilename = Path.Combine(thisExeDirPath, "_param.txt");
                if (File.Exists(paramFilename))
                {
                    WinFormStringCnv.setControlFromString(this, File.ReadAllText(paramFilename));
                }
            }

            updateClientListView();
            //timer_WebAPIcheck.Start();
            UpdateStart_WebAPIcheck(tokenSource.Token);
            tcp_srv.Start(portNumber_srv, "UTF8");

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            updateClientList();

            string FormContents = WinFormStringCnv.ToString(this);

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "TEXT|*.txt";

            if (false && sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(sfd.FileName, FormContents);
            }
            else
            {
                string paramFilename = Path.Combine(thisExeDirPath, "_param.txt");
                File.WriteAllText(paramFilename, FormContents);
            }

            tokenSource.Cancel();
            Thread.Sleep(100);
            tokenSource.Dispose();

        }

        //===================
        // Member variable
        //===================
        string thisExeDirPath;

        TcpSocketServer tcp_srv;
        int portNumber_srv;
        TcpSocketClient tcp_clt;

        DateTime WebAPI_LastCheckTime;
        private CancellationTokenSource tokenSource;

        //===================
        // Member function
        //===================
        private void updateClientListView()
        {
            string[] Lines = textBox_ClientList.Text.Replace("\r\n", "\n").Trim('\n').Split('\n');
            int panelTop = 0;

            panel_ClietListView_Form.Controls.Clear();
            foreach (var Line in Lines)
            {
                ClientListView clientListView = new ClientListView(Line, tcp_clt);
                clientListView.Top = panelTop;
                panel_ClietListView_Form.Controls.Add(clientListView);
                panelTop += clientListView.Height;
            }
            panel_ClietListView_Form.Height = panelTop;
        }

        private void updateClientList()
        {
            List<string> Lines = new List<string>();

            foreach (var Line in panel_ClietListView_Form.Controls)
            {
                if (Line is ClientListView)
                {
                    Lines.Add(((ClientListView)Line).getContets());
                }
            }

            textBox_ClientList.Text = string.Join("\r\n", Lines);
        }

        private void ResetAction(string[] colsCom)
        {
            if (textBox_Queue.Text.Length > 0) textBox_Queue.Text += "\r\n";
            string urlDec = HttpUtility.UrlDecode(colsCom[1]);
            textBox_Queue.Text += urlDec;

            string[] options = colsCom[1].Replace("/api/Reset?", "").Split('&');

            foreach (var item in options)
            {
                if (item == "all")
                {
                    foreach (var ctrl in panel_ClietListView_Form.Controls)
                    {
                        if (ctrl is ClientListView)
                        {
                            ((ClientListView)ctrl).Release();
                        }
                    }
                }
                else if (item.IndexOf("target=") == 0)
                {
                    foreach (var ctrl in panel_ClietListView_Form.Controls)
                    {
                        if (ctrl is ClientListView
                            && ((ClientListView)ctrl).ClientName.IndexOf(item.Replace("target=", "")) >= 0)
                        {
                            ((ClientListView)ctrl).Release();
                        }
                    }
                }
            }
        }

        private void LockIfNG_Action(string[] colsCom)
        {
            if (textBox_Queue.Text.Length > 0) textBox_Queue.Text += "\r\n";
            string urlDec = HttpUtility.UrlDecode(colsCom[1]);
            textBox_Queue.Text += urlDec;

            string[] options = colsCom[1].Replace("/api/LockIfErrorFrom", "").Split('&');

            string SignalSourceName = options[0].Split('?')[0];
            options[0] = options[0].Replace(SignalSourceName + "?", "");
            foreach (var item in options)
            {
                if (item == "all")
                {
                    foreach (var ctrl in panel_ClietListView_Form.Controls)
                    {
                        if (ctrl is ClientListView
                            && !((ClientListView)ctrl).OK)
                        {
                            ((ClientListView)ctrl).Lock(SignalSourceName);
                        }
                        else if (ctrl is ClientListView
                           && ((ClientListView)ctrl).OK)
                        {
                            ((ClientListView)ctrl).Release();
                        }
                    }
                }
                else if (item.IndexOf("target=") == 0)
                {
                    foreach (var ctrl in panel_ClietListView_Form.Controls)
                    {
                        if (ctrl is ClientListView
                            && ((ClientListView)ctrl).ClientName.IndexOf(item.Replace("target=", "")) >= 0
                            && !((ClientListView)ctrl).OK)
                        {
                            ((ClientListView)ctrl).Lock(SignalSourceName);
                        }
                        else if (ctrl is ClientListView
                           && ((ClientListView)ctrl).ClientName.IndexOf(item.Replace("target=", "")) >= 0
                           && ((ClientListView)ctrl).OK)
                        {
                            ((ClientListView)ctrl).Release();
                        }
                    }
                }
            }
        }

        //===================
        // Event
        //===================
       

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            tabControl_Main.Height = this.Height - 60;
        }

        private void label_Key_Click(object sender, EventArgs e)
        {
            Label src = (Label)sender;
            Clipboard.SetText(src.Text);
        }

        private void tabControl_ClientList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl_Main.SelectedTab == tabPage_ClientList)
            {
                if (tabControl_ClientList.SelectedTab == tabPage_ClientListView)
                {
                    updateClientListView();
                }
                else
                {
                    updateClientList();
                }

            }
            return;
        }

        private void textBox_HTTPPortNumber_TextChanged(object sender, EventArgs e)
        {
            int.TryParse(textBox_HTTPPortNumber.Text, out portNumber_srv);
        }


        private string toolStripStatusLabel2_Text
        {
            get { return toolStripStatusLabel2.Text; }
            set
            {
                if (this.InvokeRequired) { this.Invoke((Action)(() => toolStripStatusLabel2_Text = value)); }
                else
                {
                    toolStripStatusLabel2.Text = value;
                }
            }
        }


        private int UpdateInterval_WebAPIcheck = 1;
        private Task UpdateTask_WebAPIcheck;
        private void UpdateStart_WebAPIcheck(CancellationToken token)
        {
            UpdateTask_WebAPIcheck = Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        toolStripStatusLabel2_Text = DateTime.Now.ToString("HH:mm:ss") + " QueueCount=" + tcp_srv.ReceivedSocketQueue.Count.ToString();

                        if (tcp_srv.IsBusy)
                        {
                            toolStripStatusLabel2_Text += " / socket server Run";
                        }
                        else { toolStripStatusLabel2_Text += " / socket server Stop"; }

                        //============
                        // New data check from Queue
                        //============
                        if ((tcp_srv.LastReceiveTime - WebAPI_LastCheckTime).TotalSeconds > 0 && tcp_srv.ReceivedSocketQueue.Count > 0)
                        {
                            List<string> queueList = new List<string>();

                            string receivedSocketMessage = "";

                            while (tcp_srv.ReceivedSocketQueue.TryDequeue(out receivedSocketMessage))
                            {
                                string[] lines = receivedSocketMessage.Replace("\r\n", "\n").Trim('\n').Split('\n');
                                string[] cols = lines[0].Split('\t');

                                if (cols.Length > 1)
                                {
                                    string getCom = lines[0].Split('\t')[1];
                                    string[] colsCom = getCom.Split(' ');
                                    if (colsCom.Length > 1 && colsCom[1].IndexOf("/api/Reset?") == 0)
                                    {
                                        ResetAction(colsCom);
                                    }
                                    else if (colsCom.Length > 1 && colsCom[1].IndexOf("/api/LockIfErrorFrom") == 0)
                                    {
                                        LockIfNG_Action(colsCom);
                                    }
                                }
                            }
                            WebAPI_LastCheckTime = DateTime.Now;
                        }
                    }
                    catch { }

                    Task.Delay(TimeSpan.FromSeconds(UpdateInterval_WebAPIcheck), token).Wait();
                }
            }, token);

        }
    }
}
