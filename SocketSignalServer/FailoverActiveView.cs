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

namespace SocketSignalServer
{
    public partial class FailoverActiveView : UserControl
    {
        //===================
        // Constructor
        //===================
        public FailoverActiveView(int Index, string Address, int Port)
        {
            InitializeComponent();
            this.Address = Address;
            this.Port = Port;
            this.Index = Index;

            Alive = true;

            tcpClient = new TcpSocketClient();
        }

        public FailoverActiveView(int Index, string Line = "\t")
        {
            InitializeComponent();

            string[] Cols = Line.Split('\t');

            if (Cols.Length > 0) { this.Address = Cols[0]; }
            if (Cols.Length > 1) { int b = -1; int.TryParse(Cols[1], out b); this.Port = b; }

            this.Index = Index;

            Alive = true;

            tcpClient = new TcpSocketClient();
        }

        //===================
        // Member variable
        //===================
        TcpSocketClient tcpClient;

        public string Address { get { return textBox_Address.Text; } set { textBox_Address.Text = value; } }
        public int Port { get { int b = -1; if (!int.TryParse(textBox_Port.Text, out b)) { b = -1; } return b; } set { textBox_Port.Text = value.ToString(); } }
        public bool Alive { get { return button_Status.BackColor != Color.Red; } set { if (value) { button_Status.BackColor = Color.YellowGreen; } else { button_Status.BackColor = Color.Red; } } }
        public int Index;

        public string LastMessage
        {
            set
            {
                if (this.InvokeRequired) { this.Invoke((Action)(() => LastMessage = value)); }
                else { label_LastMessage.Text = value; }
            }
            get { return label_LastMessage.Text; }
        }

        public Action<int> DeleteThis;
        public Action LoadThis;

        //===================
        // Member function
        //===================
        public override string ToString()
        {
            return Address + "\t" + Port.ToString();
        }

        //===================
        // Event
        //===================
        private void button_DeleteThis_Click(object sender, EventArgs e)
        {
            DeleteThis(Index);
        }


        private void textBox_Address_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if(LoadThis!=null) LoadThis();
            }
            catch
            {
            }
        }

        private void textBox_Port_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (LoadThis != null) LoadThis();
            }
            catch
            {
            }
        }

        private bool _askNow = false;

        public void askAlive()
        {
            if (!_askNow)
            {
                Task.Run(() => _askAlive());
            }
        }

        private async void _askAlive()
        {
            string result = await tcpClient.StartClient(Address, Port, "askAlive", "UTF8");
            LastMessage = result;
            if (result == "" || result.IndexOf("DatabaseLocked") >= 0) { Alive = false; } else { Alive = true; }
            _askNow = false;

        }

    }
}
