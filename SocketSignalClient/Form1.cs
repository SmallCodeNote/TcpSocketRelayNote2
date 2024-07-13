using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using tcpServer;

using WinFormStringCnvClass;

namespace SocketSignalClient
{
    public partial class Form1 : Form
    {
        private TcpSocketServer tcpSrv;
        private CancellationTokenSource tokenSource;
        private Task worker;
        bool IsBusy = false;
        int WorkerWait = 100;
        public DateTime LastCheckTime { get; private set; }
        public string MessageLabel
        {
            get { return label_Message.Text; }
            set
            {
                if (this.InvokeRequired) { this.Invoke((Action)(() => MessageLabel = value)); }
                else { label_Message.Text = value; }
            }
        }

        public string TimeLabel
        {
            get { return label_Time.Text; }
            set
            {
                if (this.InvokeRequired) { this.Invoke((Action)(() => TimeLabel = value)); }
                else { label_Time.Text = value; }
            }
        }

        string thisExeDirPath = "";



        public Form1()
        {
            InitializeComponent();
            tokenSource = new CancellationTokenSource();
            tcpSrv = new TcpSocketServer(80, "UTF8");
            tcpSrv.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string paramFilename = Path.Combine(thisExeDirPath, "_param.txt");

            if (File.Exists(paramFilename))
            {
                WinFormStringCnv.setControlFromString(this, File.ReadAllText(paramFilename));
            }
            tcpSrv.ResponceMessage = textBox_statusXML.Text;
            Start();

        }

        public void Start()
        {
            if (worker == null)
            {
                var token = tokenSource.Token;
                worker = Task.Run(() => Worker(token), token);
                IsBusy = true;
            }
        }

        public async void Stop()
        {
            try
            {
                tokenSource.Cancel();
                Thread.Sleep(100);
                await worker;
                IsBusy = false;
            }
            catch (Exception ex)
            {
                //Debug.Write(GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name);
                //Debug.WriteLine(ex.ToString());
            }
        }

        private void Worker(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if ((tcpSrv.LastReceiveTime - LastCheckTime).TotalSeconds > 0 && tcpSrv.ReceivedSocketQueue.Count > 0)
                {
                    if (tcpSrv.ReceivedSocketQueue.TryDequeue(out string MessageString))
                    {
                        string[] Cols = MessageString.Replace("\r\n", "\n").Trim('\n').Split('\n')[0].Split('\t');

                        if (Cols.Length >1) { MessageLabel = Cols[1]; } else { MessageLabel = MessageString; }
                        TimeLabel = Cols[0];
                    }

                    LastCheckTime = DateTime.Now;
                }
                Thread.Sleep(WorkerWait);
            }
            token.ThrowIfCancellationRequested();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            string FormContents = WinFormStringCnv.ToString(this);
            string paramFilename = Path.Combine(thisExeDirPath, "_param.txt");
            File.WriteAllText(paramFilename, FormContents);
        }
    }
}
