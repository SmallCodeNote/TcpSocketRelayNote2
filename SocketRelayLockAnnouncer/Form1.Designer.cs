namespace tcpClient_HTTPcheck
{
    partial class Form1
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

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tabControl_Main = new System.Windows.Forms.TabControl();
            this.tabPage_ClientList = new System.Windows.Forms.TabPage();
            this.tabControl_ClientList = new System.Windows.Forms.TabControl();
            this.tabPage_ClientListView = new System.Windows.Forms.TabPage();
            this.panel_ClietListView_Frame = new System.Windows.Forms.Panel();
            this.panel_ClietListView_Form = new System.Windows.Forms.Panel();
            this.tabPage_ClientListText = new System.Windows.Forms.TabPage();
            this.textBox_ClientList = new System.Windows.Forms.TextBox();
            this.tabPage_MessageSetting = new System.Windows.Forms.TabPage();
            this.textBox_Message3 = new System.Windows.Forms.TextBox();
            this.textBox_Message2 = new System.Windows.Forms.TextBox();
            this.textBox_Message1 = new System.Windows.Forms.TextBox();
            this.groupBox_KeyList = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label_Key1 = new System.Windows.Forms.Label();
            this.tabPage_SystemSetting = new System.Windows.Forms.TabPage();
            this.textBox_HTTPPortNumber = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tabPage_Log = new System.Windows.Forms.TabPage();
            this.groupBox_WebAPI_Sample = new System.Windows.Forms.GroupBox();
            this.label_TestCode = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox_Queue = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer_WebAPIcheck = new System.Windows.Forms.Timer(this.components);
            this.tabControl_Main.SuspendLayout();
            this.tabPage_ClientList.SuspendLayout();
            this.tabControl_ClientList.SuspendLayout();
            this.tabPage_ClientListView.SuspendLayout();
            this.panel_ClietListView_Frame.SuspendLayout();
            this.tabPage_ClientListText.SuspendLayout();
            this.tabPage_MessageSetting.SuspendLayout();
            this.groupBox_KeyList.SuspendLayout();
            this.tabPage_SystemSetting.SuspendLayout();
            this.tabPage_Log.SuspendLayout();
            this.groupBox_WebAPI_Sample.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl_Main
            // 
            this.tabControl_Main.Controls.Add(this.tabPage_ClientList);
            this.tabControl_Main.Controls.Add(this.tabPage_MessageSetting);
            this.tabControl_Main.Controls.Add(this.tabPage_SystemSetting);
            this.tabControl_Main.Controls.Add(this.tabPage_Log);
            this.tabControl_Main.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl_Main.Location = new System.Drawing.Point(0, 0);
            this.tabControl_Main.Name = "tabControl_Main";
            this.tabControl_Main.SelectedIndex = 0;
            this.tabControl_Main.Size = new System.Drawing.Size(536, 425);
            this.tabControl_Main.TabIndex = 0;
            // 
            // tabPage_ClientList
            // 
            this.tabPage_ClientList.Controls.Add(this.tabControl_ClientList);
            this.tabPage_ClientList.Location = new System.Drawing.Point(4, 22);
            this.tabPage_ClientList.Name = "tabPage_ClientList";
            this.tabPage_ClientList.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_ClientList.Size = new System.Drawing.Size(528, 399);
            this.tabPage_ClientList.TabIndex = 0;
            this.tabPage_ClientList.Text = "ClientList";
            this.tabPage_ClientList.UseVisualStyleBackColor = true;
            // 
            // tabControl_ClientList
            // 
            this.tabControl_ClientList.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabControl_ClientList.Controls.Add(this.tabPage_ClientListView);
            this.tabControl_ClientList.Controls.Add(this.tabPage_ClientListText);
            this.tabControl_ClientList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_ClientList.Location = new System.Drawing.Point(3, 3);
            this.tabControl_ClientList.Name = "tabControl_ClientList";
            this.tabControl_ClientList.SelectedIndex = 0;
            this.tabControl_ClientList.Size = new System.Drawing.Size(522, 393);
            this.tabControl_ClientList.TabIndex = 0;
            this.tabControl_ClientList.SelectedIndexChanged += new System.EventHandler(this.tabControl_ClientList_SelectedIndexChanged);
            // 
            // tabPage_ClientListView
            // 
            this.tabPage_ClientListView.Controls.Add(this.panel_ClietListView_Frame);
            this.tabPage_ClientListView.Location = new System.Drawing.Point(4, 4);
            this.tabPage_ClientListView.Name = "tabPage_ClientListView";
            this.tabPage_ClientListView.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_ClientListView.Size = new System.Drawing.Size(514, 367);
            this.tabPage_ClientListView.TabIndex = 0;
            this.tabPage_ClientListView.Text = "View";
            this.tabPage_ClientListView.UseVisualStyleBackColor = true;
            // 
            // panel_ClietListView_Frame
            // 
            this.panel_ClietListView_Frame.AutoScroll = true;
            this.panel_ClietListView_Frame.Controls.Add(this.panel_ClietListView_Form);
            this.panel_ClietListView_Frame.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel_ClietListView_Frame.Location = new System.Drawing.Point(3, 3);
            this.panel_ClietListView_Frame.Name = "panel_ClietListView_Frame";
            this.panel_ClietListView_Frame.Size = new System.Drawing.Size(492, 361);
            this.panel_ClietListView_Frame.TabIndex = 0;
            // 
            // panel_ClietListView_Form
            // 
            this.panel_ClietListView_Form.Location = new System.Drawing.Point(0, 0);
            this.panel_ClietListView_Form.Name = "panel_ClietListView_Form";
            this.panel_ClietListView_Form.Size = new System.Drawing.Size(463, 97);
            this.panel_ClietListView_Form.TabIndex = 0;
            // 
            // tabPage_ClientListText
            // 
            this.tabPage_ClientListText.Controls.Add(this.textBox_ClientList);
            this.tabPage_ClientListText.Location = new System.Drawing.Point(4, 4);
            this.tabPage_ClientListText.Name = "tabPage_ClientListText";
            this.tabPage_ClientListText.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_ClientListText.Size = new System.Drawing.Size(514, 367);
            this.tabPage_ClientListText.TabIndex = 1;
            this.tabPage_ClientListText.Text = "Text";
            this.tabPage_ClientListText.UseVisualStyleBackColor = true;
            // 
            // textBox_ClientList
            // 
            this.textBox_ClientList.AcceptsReturn = true;
            this.textBox_ClientList.AcceptsTab = true;
            this.textBox_ClientList.Dock = System.Windows.Forms.DockStyle.Left;
            this.textBox_ClientList.Location = new System.Drawing.Point(3, 3);
            this.textBox_ClientList.Multiline = true;
            this.textBox_ClientList.Name = "textBox_ClientList";
            this.textBox_ClientList.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_ClientList.Size = new System.Drawing.Size(461, 361);
            this.textBox_ClientList.TabIndex = 0;
            this.textBox_ClientList.WordWrap = false;
            // 
            // tabPage_MessageSetting
            // 
            this.tabPage_MessageSetting.Controls.Add(this.textBox_Message3);
            this.tabPage_MessageSetting.Controls.Add(this.textBox_Message2);
            this.tabPage_MessageSetting.Controls.Add(this.textBox_Message1);
            this.tabPage_MessageSetting.Controls.Add(this.groupBox_KeyList);
            this.tabPage_MessageSetting.Location = new System.Drawing.Point(4, 22);
            this.tabPage_MessageSetting.Name = "tabPage_MessageSetting";
            this.tabPage_MessageSetting.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_MessageSetting.Size = new System.Drawing.Size(528, 399);
            this.tabPage_MessageSetting.TabIndex = 1;
            this.tabPage_MessageSetting.Text = "MessageSetting";
            this.tabPage_MessageSetting.UseVisualStyleBackColor = true;
            // 
            // textBox_Message3
            // 
            this.textBox_Message3.Location = new System.Drawing.Point(8, 330);
            this.textBox_Message3.Multiline = true;
            this.textBox_Message3.Name = "textBox_Message3";
            this.textBox_Message3.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_Message3.Size = new System.Drawing.Size(512, 54);
            this.textBox_Message3.TabIndex = 1;
            // 
            // textBox_Message2
            // 
            this.textBox_Message2.Location = new System.Drawing.Point(8, 237);
            this.textBox_Message2.Multiline = true;
            this.textBox_Message2.Name = "textBox_Message2";
            this.textBox_Message2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_Message2.Size = new System.Drawing.Size(512, 54);
            this.textBox_Message2.TabIndex = 1;
            // 
            // textBox_Message1
            // 
            this.textBox_Message1.Location = new System.Drawing.Point(8, 146);
            this.textBox_Message1.Multiline = true;
            this.textBox_Message1.Name = "textBox_Message1";
            this.textBox_Message1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_Message1.Size = new System.Drawing.Size(512, 54);
            this.textBox_Message1.TabIndex = 1;
            // 
            // groupBox_KeyList
            // 
            this.groupBox_KeyList.Controls.Add(this.label3);
            this.groupBox_KeyList.Controls.Add(this.label2);
            this.groupBox_KeyList.Controls.Add(this.label1);
            this.groupBox_KeyList.Controls.Add(this.label_Key1);
            this.groupBox_KeyList.Location = new System.Drawing.Point(6, 6);
            this.groupBox_KeyList.Name = "groupBox_KeyList";
            this.groupBox_KeyList.Size = new System.Drawing.Size(514, 37);
            this.groupBox_KeyList.TabIndex = 0;
            this.groupBox_KeyList.TabStop = false;
            this.groupBox_KeyList.Text = "KeyList";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(234, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "{ResetTime}";
            this.label3.Click += new System.EventHandler(this.label_Key_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(158, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "{LockFrom}";
            this.label2.Click += new System.EventHandler(this.label_Key_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(82, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "{LockTime}";
            this.label1.Click += new System.EventHandler(this.label_Key_Click);
            // 
            // label_Key1
            // 
            this.label_Key1.AutoSize = true;
            this.label_Key1.Location = new System.Drawing.Point(6, 15);
            this.label_Key1.Name = "label_Key1";
            this.label_Key1.Size = new System.Drawing.Size(70, 12);
            this.label_Key1.TabIndex = 0;
            this.label_Key1.Text = "{ClientName}";
            this.label_Key1.Click += new System.EventHandler(this.label_Key_Click);
            // 
            // tabPage_SystemSetting
            // 
            this.tabPage_SystemSetting.Controls.Add(this.textBox_HTTPPortNumber);
            this.tabPage_SystemSetting.Controls.Add(this.label5);
            this.tabPage_SystemSetting.Location = new System.Drawing.Point(4, 22);
            this.tabPage_SystemSetting.Name = "tabPage_SystemSetting";
            this.tabPage_SystemSetting.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_SystemSetting.Size = new System.Drawing.Size(528, 399);
            this.tabPage_SystemSetting.TabIndex = 2;
            this.tabPage_SystemSetting.Text = "SystemSetting";
            this.tabPage_SystemSetting.UseVisualStyleBackColor = true;
            // 
            // textBox_HTTPPortNumber
            // 
            this.textBox_HTTPPortNumber.Location = new System.Drawing.Point(8, 29);
            this.textBox_HTTPPortNumber.Name = "textBox_HTTPPortNumber";
            this.textBox_HTTPPortNumber.Size = new System.Drawing.Size(100, 19);
            this.textBox_HTTPPortNumber.TabIndex = 1;
            this.textBox_HTTPPortNumber.TextChanged += new System.EventHandler(this.textBox_HTTPPortNumber_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 14);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(94, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "HTTPPortNumber";
            // 
            // tabPage_Log
            // 
            this.tabPage_Log.Controls.Add(this.groupBox_WebAPI_Sample);
            this.tabPage_Log.Controls.Add(this.textBox_Queue);
            this.tabPage_Log.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Log.Name = "tabPage_Log";
            this.tabPage_Log.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Log.Size = new System.Drawing.Size(528, 399);
            this.tabPage_Log.TabIndex = 3;
            this.tabPage_Log.Text = "Log";
            this.tabPage_Log.UseVisualStyleBackColor = true;
            // 
            // groupBox_WebAPI_Sample
            // 
            this.groupBox_WebAPI_Sample.Controls.Add(this.label_TestCode);
            this.groupBox_WebAPI_Sample.Controls.Add(this.label4);
            this.groupBox_WebAPI_Sample.Controls.Add(this.label7);
            this.groupBox_WebAPI_Sample.Controls.Add(this.label6);
            this.groupBox_WebAPI_Sample.Location = new System.Drawing.Point(8, 6);
            this.groupBox_WebAPI_Sample.Name = "groupBox_WebAPI_Sample";
            this.groupBox_WebAPI_Sample.Size = new System.Drawing.Size(512, 82);
            this.groupBox_WebAPI_Sample.TabIndex = 2;
            this.groupBox_WebAPI_Sample.TabStop = false;
            this.groupBox_WebAPI_Sample.Text = "WebAPI Sample";
            // 
            // label_TestCode
            // 
            this.label_TestCode.AutoSize = true;
            this.label_TestCode.Location = new System.Drawing.Point(6, 15);
            this.label_TestCode.Name = "label_TestCode";
            this.label_TestCode.Size = new System.Drawing.Size(159, 12);
            this.label_TestCode.TabIndex = 1;
            this.label_TestCode.Text = "http://localhost/api/Reset?all";
            this.label_TestCode.Click += new System.EventHandler(this.label_Key_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(275, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "http://localhost/api/LockIfErrorFromDeviceName?all";
            this.label4.Click += new System.EventHandler(this.label_Key_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 39);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(364, 12);
            this.label7.TabIndex = 1;
            this.label7.Text = "http://localhost/api/LockIfErrorFromDeviceName?target=ClientName1";
            this.label7.Click += new System.EventHandler(this.label_Key_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 27);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(248, 12);
            this.label6.TabIndex = 1;
            this.label6.Text = "http://localhost/api/Reset?target=ClientName1";
            this.label6.Click += new System.EventHandler(this.label_Key_Click);
            // 
            // textBox_Queue
            // 
            this.textBox_Queue.Location = new System.Drawing.Point(6, 94);
            this.textBox_Queue.Multiline = true;
            this.textBox_Queue.Name = "textBox_Queue";
            this.textBox_Queue.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_Queue.Size = new System.Drawing.Size(514, 299);
            this.textBox_Queue.TabIndex = 0;
            this.textBox_Queue.WordWrap = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(536, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(16, 17);
            this.toolStripStatusLabel2.Text = "...";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(16, 17);
            this.toolStripStatusLabel1.Text = "...";
            // 
            // timer_WebAPIcheck
            // 
            this.timer_WebAPIcheck.Interval = 1000;
            this.timer_WebAPIcheck.Tick += new System.EventHandler(this.timer_WebAPIcheck_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(536, 450);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tabControl_Main);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "LockAnnouncer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.tabControl_Main.ResumeLayout(false);
            this.tabPage_ClientList.ResumeLayout(false);
            this.tabControl_ClientList.ResumeLayout(false);
            this.tabPage_ClientListView.ResumeLayout(false);
            this.panel_ClietListView_Frame.ResumeLayout(false);
            this.tabPage_ClientListText.ResumeLayout(false);
            this.tabPage_ClientListText.PerformLayout();
            this.tabPage_MessageSetting.ResumeLayout(false);
            this.tabPage_MessageSetting.PerformLayout();
            this.groupBox_KeyList.ResumeLayout(false);
            this.groupBox_KeyList.PerformLayout();
            this.tabPage_SystemSetting.ResumeLayout(false);
            this.tabPage_SystemSetting.PerformLayout();
            this.tabPage_Log.ResumeLayout(false);
            this.tabPage_Log.PerformLayout();
            this.groupBox_WebAPI_Sample.ResumeLayout(false);
            this.groupBox_WebAPI_Sample.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl_Main;
        private System.Windows.Forms.TabPage tabPage_ClientList;
        private System.Windows.Forms.TabPage tabPage_MessageSetting;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.TabControl tabControl_ClientList;
        private System.Windows.Forms.TabPage tabPage_ClientListView;
        private System.Windows.Forms.Panel panel_ClietListView_Frame;
        private System.Windows.Forms.TabPage tabPage_ClientListText;
        private System.Windows.Forms.TextBox textBox_ClientList;
        private System.Windows.Forms.Panel panel_ClietListView_Form;
        private System.Windows.Forms.GroupBox groupBox_KeyList;
        private System.Windows.Forms.Label label_Key1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_Message3;
        private System.Windows.Forms.TextBox textBox_Message2;
        private System.Windows.Forms.TextBox textBox_Message1;
        private System.Windows.Forms.TabPage tabPage_SystemSetting;
        private System.Windows.Forms.TextBox textBox_HTTPPortNumber;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Timer timer_WebAPIcheck;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.TabPage tabPage_Log;
        private System.Windows.Forms.TextBox textBox_Queue;
        private System.Windows.Forms.Label label_TestCode;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox_WebAPI_Sample;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
    }
}

