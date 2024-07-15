using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FluentScheduler;

namespace tcpClient
{
    public partial class OnceJobView : UserControl
    {
        TcpSocketClient tcp;
        public Form1 form1;

        string addressPortSet;
        
        string jobName { get { return groupBox_Job.Text; } set { groupBox_Job.Text = value; } }
        string scheduleUnit { get { return label_ScheduleUnit.Text; } set { label_ScheduleUnit.Text = value; } }
        string scheduleUnitParam { get { return label_ScheduleUnitParam.Text; } set { label_ScheduleUnitParam.Text = value; } }
        string clientName;
        string status;
        string message { get { return label_Message.Text; } set { label_Message.Text = value; } }
        string parameter;
        string checkStyle;

        public OnceJobView(TcpSocketClient tcp, string[] Cols, Form1 form1)
        {
            InitializeComponent();

            this.tcp = tcp;

            
            int colIndex = 0;

            addressPortSet = Cols[colIndex]; colIndex++;
            
            jobName = Cols[colIndex]; colIndex++;
            scheduleUnit = Cols[colIndex]; colIndex++;
            scheduleUnitParam = Cols[colIndex]; colIndex++;
            clientName = Cols[colIndex]; colIndex++;
            status = Cols[colIndex]; colIndex++;
            message = Cols[colIndex]; colIndex++;
            parameter = Cols[colIndex]; colIndex++;
            checkStyle = Cols[colIndex]; colIndex++;

            this.form1 = form1;

        }

        private void button_AddJob_Click(object sender, EventArgs e)
        {

            SchedulerInitialize();

            form1.tabControl_Main.TabPages["tabPage_View"].Select();
        }

        private void SchedulerInitialize()
        {
            List<string> ColList = new List<string>();
            ColList.Add(addressPortSet);

            ColList.Add(jobName);
            ColList.Add(scheduleUnit);
            ColList.Add(scheduleUnitParam);
            ColList.Add(clientName);
            ColList.Add(status);
            ColList.Add(message);
            ColList.Add(parameter);
            ColList.Add(checkStyle.ToString());

            List<string> Lines = new List<string>();
            Lines.Add(String.Join("\t", ColList.ToArray()));

            var job = new FluentSchedulerRegistry(tcp, Lines.ToArray());
            JobManager.Initialize(job);

        }

    }
}
