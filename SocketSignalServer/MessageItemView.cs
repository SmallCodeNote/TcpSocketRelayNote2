using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Diagnostics;

using LiteDB;

namespace SocketSignalServer
{
    public partial class MessageItemView : UserControl
    {
        public MessageItemView()
        {
            InitializeComponent();

            this.label_Status.Text = "";
            this.label_LastConnectTime.Text = "";
            this.label_ElapsedTime.Text = "";
            this.label_LastMessage.Text = "";
            _socketMessage = new SocketMessage();

        }

        private SocketMessage _socketMessage;

        public SocketMessage socketMessage
        {
            get { return _socketMessage; }
            set
            {
                if (this.InvokeRequired) { this.Invoke((Action)(() => socketMessage = value)); }
                else
                {
                    _socketMessage = value;
                    clientName = value.clientName;
                    status = value.status;
                    message = value.message;
                    check = value.check;
                    connectTime = value.connectTime;
                }
            }
        }

        public DateTime connectTime { get { return _socketMessage.connectTime; }    set { if (this.InvokeRequired) { this.Invoke((Action)(() => connectTime = value)); }else { _socketMessage.connectTime = value; this.label_ElapsedTime.Text = getElapsedTimeString(DateTime.Now - _socketMessage.connectTime); this.label_LastConnectTime.Text = _socketMessage.connectTime.ToString("yyyy/MM/dd HH:mm:ss"); } } }
        public string clientName {      get { return _socketMessage.clientName; }   set { if (this.InvokeRequired) { this.Invoke((Action)(() => clientName = value)); } else { _socketMessage.clientName = value; this.groupBox_ClientName.Text = value; } } }
        public string status {          get { return _socketMessage.status; }       set { if (this.InvokeRequired) { this.Invoke((Action)(() => status = value)); }     else { _socketMessage.status = value; this.label_Status.Text = value; } } }
        public string message {         get { return _socketMessage.message; }      set { if (this.InvokeRequired) { this.Invoke((Action)(() => message = value)); }    else { _socketMessage.message = value; this.label_LastMessage.Text = value; } } }
        public bool check {             get { return _socketMessage.check; }        set { if (this.InvokeRequired) { this.Invoke((Action)(() => check = value)); }      else { _socketMessage.check = value; this.checkBox_check.Checked = value; } } }
        public string checkStyle {      get { return _socketMessage.checkStyle; }   set { if (this.InvokeRequired) { this.Invoke((Action)(() => checkStyle = value)); } else { _socketMessage.checkStyle = value;  } } }
        public string parameter {       get { return _socketMessage.parameter; }    set { if (this.InvokeRequired) { this.Invoke((Action)(() => parameter = value)); }  else { _socketMessage.parameter = value; } } }

        public string getElapsedTimeString(TimeSpan elapsedTime)
        {
            if (elapsedTime.TotalDays >= 365) { return (elapsedTime.TotalDays / 365.2425).ToString("0") + " year"; }
            if (elapsedTime.TotalDays >= 30) { return (elapsedTime.TotalDays / 30.436875).ToString("0") + " month"; }
            if (elapsedTime.TotalDays >= 7) { return (elapsedTime.TotalDays / 7).ToString("0") + " week"; }
            if (elapsedTime.TotalDays >= 1) { return (elapsedTime.TotalDays / 7).ToString("0") + " day"; }
            if (elapsedTime.TotalHours >= 1) { return (elapsedTime.TotalHours).ToString("0") + " hour"; }
            if (elapsedTime.TotalMinutes >= 1) { return (elapsedTime.TotalMinutes).ToString("0") + " minute"; }
            return "now";
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            check = checkBox_check.Checked;
        }

        private void button_AllCheck_Click(object sender, EventArgs e)
        {

        }
    }
}
