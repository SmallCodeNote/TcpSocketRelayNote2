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
using UserControlPanelViewSet;

namespace ServerInfoUserControl
{
    public partial class ServerInfo : UserControl, IPanelChildUserControl
    {
        //===================
        // Constructor
        //===================
        public ServerInfo(int Index, string ServerName, string Address, int Port)
        {
            InitializeComponent();
            tcpClt = new TcpSocketClient();
            ServerInfoUpdate(Index, ServerName, Address, Port);
        }

        public ServerInfo(int Index, string Line = "\t\t")
        {
            InitializeComponent();
            tcpClt = new TcpSocketClient();

            string[] cols = Line.Split('\t');
            string ServerName = cols[0];
            string Address = cols[1];
            int.TryParse(cols[2], out int Port);

            ServerInfoUpdate(Index, ServerName, Address, Port);
        }

        public ServerInfo()
        {
            InitializeComponent();
            tcpClt = new TcpSocketClient();
        }

        //===================
        // Member variable
        //===================
        TcpSocketClient tcpClt;
        public int TimeOutCount = 0;
        public int ChildIndex { get; set; }

        public Action<int> DeleteThis { get; set; }
        public Action ControlContentsChanged { get; set; }

        public string Address
        {
            get { return textBox_Address.Text; }
            set
            {
                if (this.InvokeRequired)
                {
                    this.Invoke((Action)(() => Address = value));
                }
                else
                {
                    textBox_Address.Text = value;
                }
            }
        }

        public int Port
        {
            get
            {
                int b = -1;
                if (!int.TryParse(textBox_Port.Text, out b)) { b = -1; };
                return b;
            }
            set
            {
                if (this.InvokeRequired)
                {
                    this.Invoke((Action)(() => Port = value));
                }
                else
                {
                    textBox_Port.Text = value.ToString();
                }
            }
        }

        public string ServerName
        {
            get { return textBox_ServerName.Text; }
            set
            {
                if (this.InvokeRequired)
                {
                    this.Invoke((Action)(() => ServerName = value));
                }
                else
                {
                    groupBox_ServerInfo.Text = value;
                    textBox_ServerName.Text = value;
                }
            }
        }

        public string LatestAnswer
        {
            get { return label_LatestAnswer.Text; }
            set
            {
                if (this.InvokeRequired)
                {
                    this.Invoke((Action)(() => { LatestAnswer = value; }));
                }
                else
                {
                    label_LatestAnswer.Text = value;
                    label_LatestAnswerTime.Text = DateTime.Now.ToString("MM/dd HH:mm:ss");

                    if (value == "") { button_Lamp.BackColor = Color.Red; label_LatestAnswerTime.Text += " (TimeOut x" + TimeOutCount.ToString() + ")"; } else if (value != "Connecting...") { button_Lamp.BackColor = Color.YellowGreen; }
                }
            }
        }

        public bool Alive { get { return button_Lamp.BackColor != Color.Red; } }

        //===================
        // Member function
        //===================
        public void ParamSetFromString(string Line)
        {
            List<string> cols = new List<string>(Line.Split('\t'));
            if (cols[0] == this.GetType().Name) { cols.RemoveAt(0); }

            if (cols.Count > 0) this.ServerName = cols[0];
            if (cols.Count > 1) this.Address = cols[1];
            if (cols.Count > 2) this.Port = int.Parse(cols[2]);
        }

        public IPanelChildUserControl Clone()
        {
            ServerInfo childControl = new ServerInfo(this.ChildIndex, this.Name, this.Address, this.Port);
            return childControl;
        }

        public IPanelChildUserControl New(string Line)
        {
            ServerInfo childControl = new ServerInfo(0, Line);
            return childControl;
        }

        public void ServerInfoUpdate(int ChildIndex, string ServerName, string Address, int Port)
        {
            this.Height = 70;

            this.ServerName = ServerName;
            this.Address = Address;
            this.Port = Port;
            this.ChildIndex = ChildIndex;
        }

        public override string ToString()
        {
            return ServerName + "\t" + Address + "\t" + Port.ToString();
        }

        public async void SendMessage(string request)
        {
            if (Port >= 1024)
            {
                //LatestAnswer = "Connecting...";
                LatestAnswer = await tcpClt.StartClient(Address, Port, request, "UTF8");
                if (LatestAnswer == "") { TimeOutCount++; } else { TimeOutCount = 0; }
            }
        }

        //===================
        // Event
        //===================
        private void ServerInfo_Load(object sender, EventArgs e)
        {
            this.groupBox_ServerInfo.Height = 68;
            this.panel_Frame.Height = 45;
            this.panel.Top = 0;
        }

        private void button_Shift_Click(object sender, EventArgs e)
        {
            if (button_Shift.Text == ">")
            {
                this.panel.Top = -45;
                button_Shift.Text = "<";
            }
            else
            {
                this.panel.Top = 0;
                button_Shift.Text = ">";
            }
        }

        private void button_DeleteThis_Click(object sender, EventArgs e)
        {
            if (ControlContentsChanged != null) ControlContentsChanged();
            DeleteThis(ChildIndex);
        }

        private void textBox_Address_TextChanged(object sender, EventArgs e)
        {
            if (ControlContentsChanged != null) ControlContentsChanged();
        }

        private void textBox_Port_TextChanged(object sender, EventArgs e)
        {
            if (ControlContentsChanged != null) ControlContentsChanged();
        }

        private void textBox_ServerName_TextChanged(object sender, EventArgs e)
        {
            if (ControlContentsChanged != null)ControlContentsChanged();
            groupBox_ServerInfo.Text = textBox_ServerName.Text;
        }
    }
}
