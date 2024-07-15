namespace tcpClient
{
    partial class OnceJobView
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
            this.groupBox_Job = new System.Windows.Forms.GroupBox();
            this.button_AddJob = new System.Windows.Forms.Button();
            this.label_Message = new System.Windows.Forms.Label();
            this.label_ScheduleUnitParam = new System.Windows.Forms.Label();
            this.label_ScheduleUnit = new System.Windows.Forms.Label();
            this.groupBox_Job.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox_Job
            // 
            this.groupBox_Job.Controls.Add(this.button_AddJob);
            this.groupBox_Job.Controls.Add(this.label_Message);
            this.groupBox_Job.Controls.Add(this.label_ScheduleUnitParam);
            this.groupBox_Job.Controls.Add(this.label_ScheduleUnit);
            this.groupBox_Job.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.groupBox_Job.Location = new System.Drawing.Point(3, 3);
            this.groupBox_Job.Name = "groupBox_Job";
            this.groupBox_Job.Size = new System.Drawing.Size(400, 90);
            this.groupBox_Job.TabIndex = 1;
            this.groupBox_Job.TabStop = false;
            this.groupBox_Job.Text = "JobName";
            // 
            // button_AddJob
            // 
            this.button_AddJob.Location = new System.Drawing.Point(347, 10);
            this.button_AddJob.Name = "button_AddJob";
            this.button_AddJob.Size = new System.Drawing.Size(48, 74);
            this.button_AddJob.TabIndex = 1;
            this.button_AddJob.Text = "<< Add";
            this.button_AddJob.UseVisualStyleBackColor = true;
            this.button_AddJob.Click += new System.EventHandler(this.button_AddJob_Click);
            // 
            // label_Message
            // 
            this.label_Message.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label_Message.Location = new System.Drawing.Point(6, 35);
            this.label_Message.Name = "label_Message";
            this.label_Message.Size = new System.Drawing.Size(335, 49);
            this.label_Message.TabIndex = 0;
            this.label_Message.Text = "Message";
            // 
            // label_ScheduleUnitParam
            // 
            this.label_ScheduleUnitParam.AutoSize = true;
            this.label_ScheduleUnitParam.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label_ScheduleUnitParam.Location = new System.Drawing.Point(94, 20);
            this.label_ScheduleUnitParam.Name = "label_ScheduleUnitParam";
            this.label_ScheduleUnitParam.Size = new System.Drawing.Size(104, 12);
            this.label_ScheduleUnitParam.TabIndex = 0;
            this.label_ScheduleUnitParam.Text = "ScheduleUnitParam";
            // 
            // label_ScheduleUnit
            // 
            this.label_ScheduleUnit.AutoSize = true;
            this.label_ScheduleUnit.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label_ScheduleUnit.Location = new System.Drawing.Point(6, 20);
            this.label_ScheduleUnit.Name = "label_ScheduleUnit";
            this.label_ScheduleUnit.Size = new System.Drawing.Size(72, 12);
            this.label_ScheduleUnit.TabIndex = 0;
            this.label_ScheduleUnit.Text = "ScheduleUnit";
            // 
            // OnceJobView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox_Job);
            this.Name = "OnceJobView";
            this.Size = new System.Drawing.Size(410, 100);
            this.groupBox_Job.ResumeLayout(false);
            this.groupBox_Job.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox_Job;
        private System.Windows.Forms.Button button_AddJob;
        private System.Windows.Forms.Label label_Message;
        private System.Windows.Forms.Label label_ScheduleUnitParam;
        private System.Windows.Forms.Label label_ScheduleUnit;
    }
}
