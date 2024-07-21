using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using tcpClient;

namespace tcpClient_HTTPcheck
{
    public partial class ClientListView : UserControl
    {
        //===================
        // Constructor
        //===================
        public ClientListView(string Line, TcpSocketClient tcpClt)
        {
            InitializeComponent();
            LockReleaseTime = DateTime.Now;
            setContents(Line);

            timer_TimeLabelUpdate.Start();

            
            this.tcpClt = tcpClt;
            this.tcpClt_port = PortNumber;
        }

        //===================
        // Member variable
        //===================
        TcpSocketClient tcpClt;
        int tcpClt_port = 1025;

        public DateTime LockReleaseTime;
        public DateTime LockTime;

        public string SignalSourceName = "";

        public string Address { get { return textBox_Address.Text; } set { textBox_Address.Text = value; } }
        public string ClientName { get { return textBox_ClientName.Text; } set { textBox_ClientName.Text = value; } }
        public string LockTimeString { get { return textBox_LockTime.Text; } set { textBox_LockTime.Text = value; } }

        public string PortNumberString
        {
            get
            {
                return textBox_PortNumber.Text;
            }
            set
            {
                textBox_PortNumber.Text = value;
            }
        }

        public int PortNumber
        {
            get
            {
                int b = -1;
                if (!int.TryParse(textBox_PortNumber.Text, out b)) { b = -1; }
                return b;
            }
            set
            {
                textBox_PortNumber.Text = value.ToString();
            }
        }

        public bool OK
        {
            get { return button_StatusLamp.BackColor == Color.YellowGreen; }
            set
            {
                if (this.InvokeRequired) { this.Invoke(new Action<bool>(this.updateStatusLampOK), value); }
                else { updateStatusLampOK(value); }
            }
        }

        public bool nonConnection
        {
            get { return button_StatusLamp.BackColor == Color.LightGray; }
            set
            {
                if (this.InvokeRequired) { this.Invoke(new Action<bool>(this.updateStatusLampNonConnection), value); }
                else { updateStatusLampNonConnection(value); }
            }
        }

        public bool Locked
        {
            get { return LockReleaseTime > DateTime.Now; }
        }

        //===================
        // Member function
        //===================
        public string getContets()
        {
            return Address + "\t" + ClientName + "\t" + LockTimeString + "\t" + PortNumberString;
        }

        public void updateStatusLampOK(bool value)
        {
            if (value) { button_StatusLamp.BackColor = Color.YellowGreen; }
            else { button_StatusLamp.BackColor = Color.Red; }
        }

        public void updateStatusLampNonConnection(bool value)
        {
            if (value) { button_StatusLamp.BackColor = Color.LightGray; }
        }

        public void setContents(string Line)
        {
            string[] cols = Line.Split('\t');

            if (cols.Length > 3)
            {
                Address = cols[0];
                ClientName = cols[1];
                LockTimeString = cols[2];
                PortNumberString = cols[3];
            }
        }

        /// <summary>
        /// send lock message ex.) Lock\t8
        /// </summary>
        /// <param name="Minutes"></param>
        private async void LockClient(int Minutes)
        {
            LockTime = DateTime.Now;
            LockReleaseTime = DateTime.Now + new TimeSpan(0, Minutes, 0);
            LabelUpdate();

            if (PortNumber >= 0)
            {
                Task<string> clientMessageTask = tcpClt.StartClient(Address, PortNumber, "Lock\t" + Minutes.ToString()+"\t"+ SignalSourceName, "UTF8");
                string clientMessage = await clientMessageTask;
                if (clientMessage.IndexOf("OK") >= 0) { this.OK = true; }
                else if (clientMessage.IndexOf("NG") >= 0) { this.OK = false; }

            }
        }

        private void LockClient()
        {
            if (int.TryParse(LockTimeString, out int Minutes))
            {
                LockClient(Minutes);
            }
        }

        private async void ReleaseClient()
        {
            if (PortNumber >= 0)
            {
                Task<string> clientMessageTask = tcpClt.StartClient(Address, PortNumber, "Release", "UTF8");
                string clientMessage = await clientMessageTask;
                if (clientMessage.IndexOf("OK") >= 0) { this.OK = true; }
                else if (clientMessage.IndexOf("NG") >= 0) { this.OK = false; }

                LockReleaseTime = DateTime.Now;
                LabelUpdate();
            }
        }

        private async void AskStatusToClient()
        {
            if (PortNumber >= 0)
            {
                Task<string> clientMessageTask = tcpClt.StartClient(Address, PortNumber, "Status", "UTF8");
                string clientMessage = await clientMessageTask;
                if (clientMessage.IndexOf("OK") >= 0) { this.OK = true; }
                else if (clientMessage.IndexOf("NG") >= 0) { this.OK = false; }
                else { this.nonConnection = true; }

            }
        }

        private void LabelUpdate()
        {
            if (button_Lock.Enabled)
            {
                if (Locked)
                {
                    label_LockedFrom.Text = SignalSourceName;
                    label_LockedTime.Text = LockTime.ToString("yyyy/MM/dd HH:mm:ss");
                    label_ResetTime.Text = LockReleaseTime.ToString("yyyy/MM/dd HH:mm:ss");
                    label_RemainingTime.Text = getElapsedTimeString(LockReleaseTime - DateTime.Now);

                    button_Lock.Text = "";
                    button_Lock.BackColor = Color.Gray;
                }
                else
                {
                    label_LockedFrom.Text = "- Release -";
                    label_LockedTime.Text = "- Release -";
                    label_ResetTime.Text = "- Release -";
                    button_Lock.Text = "";
                    button_Lock.BackColor = Color.YellowGreen;
                }
            }
            else
            {
                button_Lock.BackColor = Color.LightGray;
            }

        }

        private void ClientListView_Load(object sender, EventArgs e)
        {
            groupBox_ClientListViewTitle.Size = new Size(431, 91);
            panel_Frame.Size = new Size(386, 67);

            this.Size = new Size(449, 100);
        }

        public void Lock(string SignalSourceName)
        {
            this.SignalSourceName = SignalSourceName;
            LockClient();
            LabelUpdate();
        }

        public void Release()
        {
            ReleaseClient();

            LockReleaseTime = DateTime.Now;
            label_LockedFrom.Text = "- Release -";
            label_LockedTime.Text = "- Release -";
            label_ResetTime.Text = "- Release -";
            button_Lock.Text = "";
            button_Lock.BackColor = Color.YellowGreen;
            label_RemainingTime.Text = "...";
        }

        //===================
        // Event
        //===================
        private void textBox_Name_TextChanged(object sender, EventArgs e)
        {
            this.groupBox_ClientListViewTitle.Text = ClientName;
        }

        private void timer_TimeLabelUpdate_Tick(object sender, EventArgs e)
        {
            LabelUpdate();
            AskStatusToClient();
        }

        private void button_Lock_Click(object sender, EventArgs e)
        {
            if ((DateTime.Now - LockReleaseTime).TotalSeconds > 0)
            {
                Lock("this panel");
            }
            else
            {
                Release();
            }
        }

        private void button_PanelShift_Click(object sender, EventArgs e)
        {
            if (panel_Form.Top == 0)
            {
                panel_Form.Top = -panel_Frame.Height;
                button_PanelShift.Text = ">";
            }
            else
            {
                panel_Form.Top = 0;
                button_PanelShift.Text = "<";
            }
        }

        public string getElapsedTimeString(TimeSpan elapsedTime)
        {
            if (elapsedTime.TotalDays >= 365) { return (elapsedTime.TotalDays / 365.2425).ToString("0") + " year"; }
            if (elapsedTime.TotalDays >= 30) { return (elapsedTime.TotalDays / 30.436875).ToString("0") + " month"; }
            if (elapsedTime.TotalDays >= 7) { return (elapsedTime.TotalDays / 7).ToString("0") + " week"; }
            if (elapsedTime.TotalDays >= 1) { return (elapsedTime.TotalDays / 7).ToString("0") + " day"; }
            if (elapsedTime.TotalHours >= 1) { return (elapsedTime.TotalHours).ToString("0") + " hour"; }
            if (elapsedTime.TotalMinutes >= 1) { return (elapsedTime.TotalMinutes).ToString("0") + " minute"; }
            if (elapsedTime.TotalSeconds >= 1) { return (elapsedTime.TotalSeconds).ToString("0") + " sec."; }
            return "now";
        }

        private void button_ErrorLamp_Click(object sender, EventArgs e)
        {
            OK = !OK;
        }

        private void textBox_PortNumber_TextChanged(object sender, EventArgs e)
        {
            button_Lock.Enabled = int.TryParse(textBox_PortNumber.Text, out int b);
        }

    }

}
