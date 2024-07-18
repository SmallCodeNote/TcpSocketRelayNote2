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

namespace SourceInfoUserControl
{
    public partial class SourceInfo : UserControl, IPanelChildUserControl
    {
        //===================
        // Constructor
        //===================
        public SourceInfo(int Index, string SourceName, string SaveDirPath, string ModelPath, string LastCheckTimeString = "", string Parameter = "")
        {
            InitializeComponent();
            tcpClt = new TcpSocketClient();
            SourceInfoUpdate(Index, SourceName, SaveDirPath, ModelPath, LastCheckTimeString, Parameter);
            if (LastCheckTimeString == "") { LastCheckTime = DateTime.Now.AddHours(-1); }
            else if (DateTime.TryParse(LastCheckTimeString, out DateTime dateTime)) { LastCheckTime = dateTime; }

        }

        public SourceInfo(int Index, string Line = "\t\t\t\t")
        {
            InitializeComponent();
            tcpClt = new TcpSocketClient();

            string[] cols = Line.Split('\t');
            string SourceName = cols.Length > 0 ? cols[0] : "";
            string SaveDirPath = cols.Length > 1 ? cols[1] : "";
            string ModelPath = cols.Length > 2 ? cols[2] : "";
            string LastCheckTimeString = cols.Length > 3 ? cols[3] : "";
            string Parameter = cols.Length > 4 ? cols[4] : "";

            SourceInfoUpdate(Index, SourceName, SaveDirPath, ModelPath, LastCheckTimeString, Parameter);

            if (LastCheckTimeString == "") LastCheckTime = DateTime.Now.AddHours(-1);
        }

        public SourceInfo()
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

        private DateTime _LastCheckTime;
        public DateTime LastCheckTime
        {
            get { return _LastCheckTime; }
            set { _LastCheckTime = value; textBox_LastCheckTime.Text = value.ToString("yyyy/MM/dd HH:mm:ss"); }
        }

        public string Parameter
        {
            get { return textBox_Parameter.Text; }
            set
            {
                if (this.InvokeRequired)
                {
                    this.Invoke((Action)(() => Parameter = value));
                }
                else
                {
                    textBox_Parameter.Text = value;
                }

            }
        }
        public string SaveDirPath
        {
            get { return textBox_SaveDirPath.Text; }
            set
            {
                if (this.InvokeRequired)
                {
                    this.Invoke((Action)(() => SaveDirPath = value));
                }
                else
                {
                    textBox_SaveDirPath.Text = value;
                }
            }
        }

        public string ModelPath
        {
            get
            {
                return textBox_ModelPath.Text;
            }
            set
            {
                if (this.InvokeRequired)
                {
                    this.Invoke((Action)(() => ModelPath = value));
                }
                else
                {
                    textBox_ModelPath.Text = value;
                }
            }
        }

        public string SourceName
        {
            get { return textBox_SourceName.Text; }
            set
            {
                if (this.InvokeRequired)
                {
                    this.Invoke((Action)(() => SourceName = value));
                }
                else
                {
                    groupBox_SourceInfo.Text = value;
                    textBox_SourceName.Text = value;
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

        public void SourceInfoUpdate(int Index, string SourceName, string SaveDirPath, string ModelPath, string LastCheckTimeString = "", string Parameter = "")
        {
            this.Height = 80;

            this.SourceName = SourceName;
            this.SaveDirPath = SaveDirPath;
            this.ModelPath = ModelPath;
            this.ChildIndex = Index;

            if (DateTime.TryParse(LastCheckTimeString, out DateTime datetime)) { this.LastCheckTime = datetime; }

            this.Parameter = Parameter;
        }

        public override string ToString()
        {
            return SourceName + "\t" + SaveDirPath + "\t" + ModelPath.ToString() + "\t" + LastCheckTime.ToString("yyyy/MM/dd HH:mm:ss") + "\t" + Parameter;
        }


        public void ParamSetFromString(string Line)
        {
            List<string> cols = new List<string>(Line.Split('\t'));
            if (cols[0] == this.GetType().Name) { cols.RemoveAt(0); }

            if (cols.Count > 0) this.SourceName = cols[0];
            if (cols.Count > 1) this.SaveDirPath = cols[1];
            if (cols.Count > 2) this.ModelPath = cols[2];
            if (cols.Count > 3) this.LastCheckTime = DateTime.Parse( cols[3]);
            if (cols.Count > 4) this.Parameter = cols[4];
            
        }


        public IPanelChildUserControl Clone()
        {
            SourceInfo childControl = new SourceInfo(this.ChildIndex, this.SourceName, this.SaveDirPath, this.ModelPath, this.LastCheckTime.ToString("yyyy/MM/dd HH:mm:ss"), this.Parameter);
            return childControl;
        }

        public IPanelChildUserControl New(string Line)
        {
            SourceInfo childControl = new SourceInfo(0, Line);
            return childControl;
        }



        //===================
        // Event
        //===================
        private void SourceInfo_Load(object sender, EventArgs e)
        {
            this.groupBox_SourceInfo.Height = 78;
            this.panel_Frame.Height = 55;
            this.panel.Top = 0;
        }

        private void button_Shift_Click(object sender, EventArgs e)
        {
            if (this.panel.Top == 0)
            {
                this.panel.Top = -55;
            }
            else if (this.panel.Top == -55)
            {
                this.panel.Top = -110;
            }
        }

        private void button_ShiftDown_Click(object sender, EventArgs e)
        {
            if (this.panel.Top == -55)
            {
                this.panel.Top = 0;
            }
            else if (this.panel.Top == -110)
            {
                this.panel.Top = -55;
            }

        }
        private void button_DeleteThis_Click(object sender, EventArgs e)
        {
            if (ControlContentsChanged != null) ControlContentsChanged();
            DeleteThis(ChildIndex);
        }

        private void textBox_SaveDirPath_TextChanged(object sender, EventArgs e)
        {
            if (ControlContentsChanged != null) ControlContentsChanged();
        }

        private void textBox_ModelPath_TextChanged(object sender, EventArgs e)
        {
            if (ControlContentsChanged != null) ControlContentsChanged();
        }

        private void textBox_SourceName_TextChanged(object sender, EventArgs e)
        {
            if (ControlContentsChanged != null) ControlContentsChanged();
            groupBox_SourceInfo.Text = textBox_SourceName.Text;
        }

        private void textBox_LastCheckTime_TextChanged(object sender, EventArgs e)
        {
            if (DateTime.TryParse(textBox_LastCheckTime.Text, out DateTime dateTime))
            {
                _LastCheckTime = dateTime;
            }
        }

        private void textBox_Parameter_TextChanged(object sender, EventArgs e)
        {
            Parameter = textBox_Parameter.Text;
        }
    }
}
