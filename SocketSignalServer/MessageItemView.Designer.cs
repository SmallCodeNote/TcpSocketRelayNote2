namespace SocketSignalServer
{
    partial class MessageItemView
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
            this.groupBox_ClientName = new System.Windows.Forms.GroupBox();
            this.checkBox_check = new System.Windows.Forms.CheckBox();
            this.button_AllCheck = new System.Windows.Forms.Button();
            this.label_LastMessage = new System.Windows.Forms.Label();
            this.label_ElapsedTime = new System.Windows.Forms.Label();
            this.label_LastConnectTime = new System.Windows.Forms.Label();
            this.label_Status = new System.Windows.Forms.Label();
            this.groupBox_ClientName.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox_ClientName
            // 
            this.groupBox_ClientName.Controls.Add(this.checkBox_check);
            this.groupBox_ClientName.Controls.Add(this.button_AllCheck);
            this.groupBox_ClientName.Controls.Add(this.label_LastMessage);
            this.groupBox_ClientName.Controls.Add(this.label_ElapsedTime);
            this.groupBox_ClientName.Controls.Add(this.label_LastConnectTime);
            this.groupBox_ClientName.Controls.Add(this.label_Status);
            this.groupBox_ClientName.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.groupBox_ClientName.Location = new System.Drawing.Point(0, 0);
            this.groupBox_ClientName.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox_ClientName.Name = "groupBox_ClientName";
            this.groupBox_ClientName.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox_ClientName.Size = new System.Drawing.Size(478, 75);
            this.groupBox_ClientName.TabIndex = 0;
            this.groupBox_ClientName.TabStop = false;
            this.groupBox_ClientName.Text = "ClientName";
            // 
            // checkBox_check
            // 
            this.checkBox_check.AutoSize = true;
            this.checkBox_check.Enabled = false;
            this.checkBox_check.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.checkBox_check.Location = new System.Drawing.Point(7, 30);
            this.checkBox_check.Margin = new System.Windows.Forms.Padding(2);
            this.checkBox_check.Name = "checkBox_check";
            this.checkBox_check.Size = new System.Drawing.Size(54, 16);
            this.checkBox_check.TabIndex = 3;
            this.checkBox_check.Text = "check";
            this.checkBox_check.UseVisualStyleBackColor = true;
            this.checkBox_check.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // button_AllCheck
            // 
            this.button_AllCheck.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button_AllCheck.Location = new System.Drawing.Point(64, 27);
            this.button_AllCheck.Margin = new System.Windows.Forms.Padding(2);
            this.button_AllCheck.Name = "button_AllCheck";
            this.button_AllCheck.Size = new System.Drawing.Size(60, 18);
            this.button_AllCheck.TabIndex = 2;
            this.button_AllCheck.Text = "AllCheck";
            this.button_AllCheck.UseVisualStyleBackColor = true;
            this.button_AllCheck.Click += new System.EventHandler(this.button_AllCheck_Click);
            // 
            // label_LastMessage
            // 
            this.label_LastMessage.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label_LastMessage.Location = new System.Drawing.Point(143, 14);
            this.label_LastMessage.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_LastMessage.Name = "label_LastMessage";
            this.label_LastMessage.Size = new System.Drawing.Size(331, 58);
            this.label_LastMessage.TabIndex = 1;
            this.label_LastMessage.Text = "Message";
            // 
            // label_ElapsedTime
            // 
            this.label_ElapsedTime.AutoSize = true;
            this.label_ElapsedTime.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label_ElapsedTime.Location = new System.Drawing.Point(4, 58);
            this.label_ElapsedTime.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_ElapsedTime.Name = "label_ElapsedTime";
            this.label_ElapsedTime.Size = new System.Drawing.Size(70, 12);
            this.label_ElapsedTime.TabIndex = 0;
            this.label_ElapsedTime.Text = "ElapsedTime";
            // 
            // label_LastConnectTime
            // 
            this.label_LastConnectTime.AutoSize = true;
            this.label_LastConnectTime.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label_LastConnectTime.Location = new System.Drawing.Point(4, 46);
            this.label_LastConnectTime.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_LastConnectTime.Name = "label_LastConnectTime";
            this.label_LastConnectTime.Size = new System.Drawing.Size(94, 12);
            this.label_LastConnectTime.TabIndex = 0;
            this.label_LastConnectTime.Text = "LastConnectTime";
            // 
            // label_Status
            // 
            this.label_Status.AutoSize = true;
            this.label_Status.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label_Status.Location = new System.Drawing.Point(4, 17);
            this.label_Status.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_Status.Name = "label_Status";
            this.label_Status.Size = new System.Drawing.Size(38, 12);
            this.label_Status.TabIndex = 0;
            this.label_Status.Text = "Status";
            // 
            // MessageItemView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox_ClientName);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MessageItemView";
            this.Size = new System.Drawing.Size(480, 80);
            this.groupBox_ClientName.ResumeLayout(false);
            this.groupBox_ClientName.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label_ElapsedTime;
        private System.Windows.Forms.Button button_AllCheck;
        public System.Windows.Forms.GroupBox groupBox_ClientName;
        public System.Windows.Forms.Label label_Status;
        public System.Windows.Forms.Label label_LastConnectTime;
        public System.Windows.Forms.Label label_LastMessage;
        private System.Windows.Forms.CheckBox checkBox_check;
    }
}
