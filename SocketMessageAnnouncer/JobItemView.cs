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
    public partial class JobItemView : UserControl
    {
        public Schedule schedule;
        public Form1 form1;

        public JobItemView()
        {
            InitializeComponent();
            this.Height = 70;
            groupBox_Job.Height = 70;
            panel_Frame.Height = 50;

            timer_update.Start();

        }

        private void button_DeleteJob_Click(object sender, EventArgs e)
        {
            JobManager.RemoveJob(schedule.Name);
            form1.tabPage_View_Enter();

        }

        public void setLabel(Schedule schedule, Form1 form1)
        {
            string[] Cols = schedule.Name.Split('\t');
            int colIndex = 0;

            groupBox_Job.Text = Cols[colIndex]; colIndex++;
            label_Address.Text = Cols[colIndex]; colIndex++;
            label_ScheduleUnit.Text = Cols[colIndex]; colIndex++;
            label_ScheduleUnitParam.Text = Cols[colIndex]; colIndex++;
            label_ClientName.Text = Cols[colIndex]; colIndex++;
            label_Status.Text = Cols[colIndex]; colIndex++;
            label_Message.Text = Cols[colIndex]; colIndex++;
            label_Parameter.Text = Cols[colIndex]; colIndex++;
            label_NeedCheck.Text = Cols[colIndex]; colIndex++;

            this.schedule = schedule;
            this.form1 = form1;
        }

        private void timer_update_Tick(object sender, EventArgs e)
        {
            try
            {
                if (schedule != null)
                {
                    label_Next.Text = getElapsedTimeString(schedule.NextRun - DateTime.Now);
                }
            }
            catch { }
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

        private void button_PanelSwitch_Click(object sender, EventArgs e)
        {
            if (panel_Contents.Top == 0)
            {
                panel_Contents.Top = -panel_Frame.Height;
                button_PanelSwitch.Text = ">";
            }
            else
            {
                panel_Contents.Top = 0;
                button_PanelSwitch.Text = "<";

            }
        }

        private void JobItemView_Load(object sender, EventArgs e)
        {
          
        }
    }

}
