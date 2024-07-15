namespace tcpClient
{
    partial class JobItemView
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox_Job = new System.Windows.Forms.GroupBox();
            this.label_Next = new System.Windows.Forms.Label();
            this.panel_Frame = new System.Windows.Forms.Panel();
            this.panel_Contents = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label_ScheduleUnitParam = new System.Windows.Forms.Label();
            this.label_Parameter = new System.Windows.Forms.Label();
            this.label_Message = new System.Windows.Forms.Label();
            this.label_NeedCheck = new System.Windows.Forms.Label();
            this.label_Address = new System.Windows.Forms.Label();
            this.label_ScheduleUnit = new System.Windows.Forms.Label();
            this.label_Status = new System.Windows.Forms.Label();
            this.label_ClientName = new System.Windows.Forms.Label();
            this.button_PanelSwitch = new System.Windows.Forms.Button();
            this.button_DeleteJob = new System.Windows.Forms.Button();
            this.timer_update = new System.Windows.Forms.Timer(this.components);
            this.groupBox_Job.SuspendLayout();
            this.panel_Frame.SuspendLayout();
            this.panel_Contents.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox_Job
            // 
            this.groupBox_Job.Controls.Add(this.label_Next);
            this.groupBox_Job.Controls.Add(this.panel_Frame);
            this.groupBox_Job.Controls.Add(this.button_PanelSwitch);
            this.groupBox_Job.Controls.Add(this.button_DeleteJob);
            this.groupBox_Job.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.groupBox_Job.Location = new System.Drawing.Point(0, 0);
            this.groupBox_Job.Name = "groupBox_Job";
            this.groupBox_Job.Size = new System.Drawing.Size(400, 127);
            this.groupBox_Job.TabIndex = 0;
            this.groupBox_Job.TabStop = false;
            this.groupBox_Job.Text = "JobName";
            // 
            // label_Next
            // 
            this.label_Next.AutoSize = true;
            this.label_Next.BackColor = System.Drawing.SystemColors.Control;
            this.label_Next.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label_Next.Location = new System.Drawing.Point(321, 0);
            this.label_Next.Name = "label_Next";
            this.label_Next.Size = new System.Drawing.Size(19, 15);
            this.label_Next.TabIndex = 2;
            this.label_Next.Text = "...";
            // 
            // panel_Frame
            // 
            this.panel_Frame.Controls.Add(this.panel_Contents);
            this.panel_Frame.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.panel_Frame.Location = new System.Drawing.Point(33, 15);
            this.panel_Frame.Name = "panel_Frame";
            this.panel_Frame.Size = new System.Drawing.Size(330, 106);
            this.panel_Frame.TabIndex = 1;
            // 
            // panel_Contents
            // 
            this.panel_Contents.Controls.Add(this.label1);
            this.panel_Contents.Controls.Add(this.label_ScheduleUnitParam);
            this.panel_Contents.Controls.Add(this.label_Parameter);
            this.panel_Contents.Controls.Add(this.label_Message);
            this.panel_Contents.Controls.Add(this.label_NeedCheck);
            this.panel_Contents.Controls.Add(this.label_Address);
            this.panel_Contents.Controls.Add(this.label_ScheduleUnit);
            this.panel_Contents.Controls.Add(this.label_Status);
            this.panel_Contents.Controls.Add(this.label_ClientName);
            this.panel_Contents.Location = new System.Drawing.Point(0, 0);
            this.panel_Contents.Name = "panel_Contents";
            this.panel_Contents.Size = new System.Drawing.Size(320, 100);
            this.panel_Contents.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(227, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "CheckStyle";
            // 
            // label_ScheduleUnitParam
            // 
            this.label_ScheduleUnitParam.AutoSize = true;
            this.label_ScheduleUnitParam.Location = new System.Drawing.Point(115, 1);
            this.label_ScheduleUnitParam.Name = "label_ScheduleUnitParam";
            this.label_ScheduleUnitParam.Size = new System.Drawing.Size(104, 12);
            this.label_ScheduleUnitParam.TabIndex = 0;
            this.label_ScheduleUnitParam.Text = "ScheduleUnitParam";
            // 
            // label_Parameter
            // 
            this.label_Parameter.AutoSize = true;
            this.label_Parameter.Location = new System.Drawing.Point(4, 85);
            this.label_Parameter.Name = "label_Parameter";
            this.label_Parameter.Size = new System.Drawing.Size(57, 12);
            this.label_Parameter.TabIndex = 0;
            this.label_Parameter.Text = "Parameter";
            // 
            // label_Message
            // 
            this.label_Message.Location = new System.Drawing.Point(3, 17);
            this.label_Message.Name = "label_Message";
            this.label_Message.Size = new System.Drawing.Size(314, 26);
            this.label_Message.TabIndex = 0;
            this.label_Message.Text = "Message";
            // 
            // label_NeedCheck
            // 
            this.label_NeedCheck.AutoSize = true;
            this.label_NeedCheck.Location = new System.Drawing.Point(227, 66);
            this.label_NeedCheck.Name = "label_NeedCheck";
            this.label_NeedCheck.Size = new System.Drawing.Size(63, 12);
            this.label_NeedCheck.TabIndex = 0;
            this.label_NeedCheck.Text = "NeedCheck";
            // 
            // label_Address
            // 
            this.label_Address.AutoSize = true;
            this.label_Address.Location = new System.Drawing.Point(3, 54);
            this.label_Address.Name = "label_Address";
            this.label_Address.Size = new System.Drawing.Size(47, 12);
            this.label_Address.TabIndex = 0;
            this.label_Address.Text = "Address";
            // 
            // label_ScheduleUnit
            // 
            this.label_ScheduleUnit.AutoSize = true;
            this.label_ScheduleUnit.Location = new System.Drawing.Point(3, 1);
            this.label_ScheduleUnit.Name = "label_ScheduleUnit";
            this.label_ScheduleUnit.Size = new System.Drawing.Size(72, 12);
            this.label_ScheduleUnit.TabIndex = 0;
            this.label_ScheduleUnit.Text = "ScheduleUnit";
            // 
            // label_Status
            // 
            this.label_Status.AutoSize = true;
            this.label_Status.Location = new System.Drawing.Point(108, 70);
            this.label_Status.Name = "label_Status";
            this.label_Status.Size = new System.Drawing.Size(38, 12);
            this.label_Status.TabIndex = 0;
            this.label_Status.Text = "Status";
            // 
            // label_ClientName
            // 
            this.label_ClientName.AutoSize = true;
            this.label_ClientName.Location = new System.Drawing.Point(4, 70);
            this.label_ClientName.Name = "label_ClientName";
            this.label_ClientName.Size = new System.Drawing.Size(64, 12);
            this.label_ClientName.TabIndex = 0;
            this.label_ClientName.Text = "ClientName";
            // 
            // button_PanelSwitch
            // 
            this.button_PanelSwitch.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button_PanelSwitch.Location = new System.Drawing.Point(7, 16);
            this.button_PanelSwitch.Name = "button_PanelSwitch";
            this.button_PanelSwitch.Size = new System.Drawing.Size(25, 49);
            this.button_PanelSwitch.TabIndex = 1;
            this.button_PanelSwitch.Text = "<";
            this.button_PanelSwitch.UseVisualStyleBackColor = true;
            this.button_PanelSwitch.Click += new System.EventHandler(this.button_PanelSwitch_Click);
            // 
            // button_DeleteJob
            // 
            this.button_DeleteJob.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button_DeleteJob.Location = new System.Drawing.Point(369, 14);
            this.button_DeleteJob.Name = "button_DeleteJob";
            this.button_DeleteJob.Size = new System.Drawing.Size(25, 49);
            this.button_DeleteJob.TabIndex = 1;
            this.button_DeleteJob.Text = "X";
            this.button_DeleteJob.UseVisualStyleBackColor = true;
            this.button_DeleteJob.Click += new System.EventHandler(this.button_DeleteJob_Click);
            // 
            // timer_update
            // 
            this.timer_update.Interval = 1000;
            this.timer_update.Tick += new System.EventHandler(this.timer_update_Tick);
            // 
            // JobItemView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox_Job);
            this.Name = "JobItemView";
            this.Size = new System.Drawing.Size(410, 155);
            this.Load += new System.EventHandler(this.JobItemView_Load);
            this.groupBox_Job.ResumeLayout(false);
            this.groupBox_Job.PerformLayout();
            this.panel_Frame.ResumeLayout(false);
            this.panel_Contents.ResumeLayout(false);
            this.panel_Contents.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox_Job;
        private System.Windows.Forms.Label label_Address;
        private System.Windows.Forms.Label label_NeedCheck;
        private System.Windows.Forms.Label label_Parameter;
        private System.Windows.Forms.Label label_Message;
        private System.Windows.Forms.Label label_Status;
        private System.Windows.Forms.Label label_ClientName;
        private System.Windows.Forms.Label label_ScheduleUnitParam;
        private System.Windows.Forms.Label label_ScheduleUnit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_DeleteJob;
        private System.Windows.Forms.Timer timer_update;
        private System.Windows.Forms.Label label_Next;
        private System.Windows.Forms.Panel panel_Frame;
        private System.Windows.Forms.Panel panel_Contents;
        private System.Windows.Forms.Button button_PanelSwitch;
    }
}
