namespace ServerInfoUserControl
{
    partial class ServerInfo
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_Address = new System.Windows.Forms.TextBox();
            this.textBox_Port = new System.Windows.Forms.TextBox();
            this.label_LatestAnswer = new System.Windows.Forms.Label();
            this.button_Lamp = new System.Windows.Forms.Button();
            this.groupBox_ServerInfo = new System.Windows.Forms.GroupBox();
            this.label_LatestAnswerTime = new System.Windows.Forms.Label();
            this.button_Shift = new System.Windows.Forms.Button();
            this.panel_Frame = new System.Windows.Forms.Panel();
            this.panel = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_ServerName = new System.Windows.Forms.TextBox();
            this.button_DeleteThis = new System.Windows.Forms.Button();
            this.groupBox_ServerInfo.SuspendLayout();
            this.panel_Frame.SuspendLayout();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Address";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(214, 2);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "Port";
            // 
            // textBox_Address
            // 
            this.textBox_Address.Location = new System.Drawing.Point(6, 14);
            this.textBox_Address.Name = "textBox_Address";
            this.textBox_Address.Size = new System.Drawing.Size(191, 19);
            this.textBox_Address.TabIndex = 2;
            this.textBox_Address.Text = "localhost";
            this.textBox_Address.TextChanged += new System.EventHandler(this.textBox_Address_TextChanged);
            // 
            // textBox_Port
            // 
            this.textBox_Port.Location = new System.Drawing.Point(216, 14);
            this.textBox_Port.Name = "textBox_Port";
            this.textBox_Port.Size = new System.Drawing.Size(63, 19);
            this.textBox_Port.TabIndex = 3;
            this.textBox_Port.TextChanged += new System.EventHandler(this.textBox_Port_TextChanged);
            // 
            // label_LatestAnswer
            // 
            this.label_LatestAnswer.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label_LatestAnswer.Location = new System.Drawing.Point(2, 15);
            this.label_LatestAnswer.Name = "label_LatestAnswer";
            this.label_LatestAnswer.Size = new System.Drawing.Size(398, 15);
            this.label_LatestAnswer.TabIndex = 0;
            this.label_LatestAnswer.Text = "...";
            // 
            // button_Lamp
            // 
            this.button_Lamp.Location = new System.Drawing.Point(365, 28);
            this.button_Lamp.Name = "button_Lamp";
            this.button_Lamp.Size = new System.Drawing.Size(32, 32);
            this.button_Lamp.TabIndex = 4;
            this.button_Lamp.UseVisualStyleBackColor = true;
            // 
            // groupBox_ServerInfo
            // 
            this.groupBox_ServerInfo.Controls.Add(this.button_DeleteThis);
            this.groupBox_ServerInfo.Controls.Add(this.button_Lamp);
            this.groupBox_ServerInfo.Controls.Add(this.label_LatestAnswerTime);
            this.groupBox_ServerInfo.Controls.Add(this.button_Shift);
            this.groupBox_ServerInfo.Controls.Add(this.panel_Frame);
            this.groupBox_ServerInfo.Controls.Add(this.label_LatestAnswer);
            this.groupBox_ServerInfo.Location = new System.Drawing.Point(0, 0);
            this.groupBox_ServerInfo.Name = "groupBox_ServerInfo";
            this.groupBox_ServerInfo.Size = new System.Drawing.Size(400, 166);
            this.groupBox_ServerInfo.TabIndex = 5;
            this.groupBox_ServerInfo.TabStop = false;
            this.groupBox_ServerInfo.Text = "ServerName";
            // 
            // label_LatestAnswerTime
            // 
            this.label_LatestAnswerTime.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.label_LatestAnswerTime.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label_LatestAnswerTime.Location = new System.Drawing.Point(220, 0);
            this.label_LatestAnswerTime.Name = "label_LatestAnswerTime";
            this.label_LatestAnswerTime.Size = new System.Drawing.Size(153, 15);
            this.label_LatestAnswerTime.TabIndex = 0;
            this.label_LatestAnswerTime.Text = "...";
            // 
            // button_Shift
            // 
            this.button_Shift.Location = new System.Drawing.Point(3, 28);
            this.button_Shift.Name = "button_Shift";
            this.button_Shift.Size = new System.Drawing.Size(18, 32);
            this.button_Shift.TabIndex = 1;
            this.button_Shift.Text = ">";
            this.button_Shift.UseVisualStyleBackColor = true;
            this.button_Shift.Click += new System.EventHandler(this.button_Shift_Click);
            // 
            // panel_Frame
            // 
            this.panel_Frame.Controls.Add(this.panel);
            this.panel_Frame.Location = new System.Drawing.Point(27, 28);
            this.panel_Frame.Name = "panel_Frame";
            this.panel_Frame.Size = new System.Drawing.Size(335, 117);
            this.panel_Frame.TabIndex = 0;
            // 
            // panel
            // 
            this.panel.Controls.Add(this.label2);
            this.panel.Controls.Add(this.label4);
            this.panel.Controls.Add(this.label1);
            this.panel.Controls.Add(this.textBox_ServerName);
            this.panel.Controls.Add(this.textBox_Address);
            this.panel.Controls.Add(this.textBox_Port);
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(335, 90);
            this.panel.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "Name";
            // 
            // textBox_ServerName
            // 
            this.textBox_ServerName.Location = new System.Drawing.Point(41, 45);
            this.textBox_ServerName.Name = "textBox_ServerName";
            this.textBox_ServerName.Size = new System.Drawing.Size(226, 19);
            this.textBox_ServerName.TabIndex = 2;
            this.textBox_ServerName.TextChanged += new System.EventHandler(this.textBox_ServerName_TextChanged);
            // 
            // button_DeleteThis
            // 
            this.button_DeleteThis.Location = new System.Drawing.Point(376, 0);
            this.button_DeleteThis.Name = "button_DeleteThis";
            this.button_DeleteThis.Size = new System.Drawing.Size(24, 24);
            this.button_DeleteThis.TabIndex = 5;
            this.button_DeleteThis.Text = "X";
            this.button_DeleteThis.UseVisualStyleBackColor = true;
            this.button_DeleteThis.Click += new System.EventHandler(this.button_DeleteThis_Click);
            // 
            // ServerInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox_ServerInfo);
            this.Name = "ServerInfo";
            this.Size = new System.Drawing.Size(400, 214);
            this.Load += new System.EventHandler(this.ServerInfo_Load);
            this.groupBox_ServerInfo.ResumeLayout(false);
            this.panel_Frame.ResumeLayout(false);
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_Address;
        private System.Windows.Forms.TextBox textBox_Port;
        private System.Windows.Forms.Label label_LatestAnswer;
        private System.Windows.Forms.Button button_Lamp;
        private System.Windows.Forms.GroupBox groupBox_ServerInfo;
        private System.Windows.Forms.Panel panel_Frame;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_ServerName;
        private System.Windows.Forms.Button button_Shift;
        private System.Windows.Forms.Label label_LatestAnswerTime;
        private System.Windows.Forms.Button button_DeleteThis;
    }
}
