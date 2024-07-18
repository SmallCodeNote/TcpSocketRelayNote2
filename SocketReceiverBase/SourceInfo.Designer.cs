namespace SourceInfoUserControl
{
    partial class SourceInfo
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
            this.textBox_SaveDirPath = new System.Windows.Forms.TextBox();
            this.textBox_ModelPath = new System.Windows.Forms.TextBox();
            this.label_LatestAnswer = new System.Windows.Forms.Label();
            this.button_Lamp = new System.Windows.Forms.Button();
            this.groupBox_SourceInfo = new System.Windows.Forms.GroupBox();
            this.button_ShiftDown = new System.Windows.Forms.Button();
            this.button_DeleteThis = new System.Windows.Forms.Button();
            this.label_LatestAnswerTime = new System.Windows.Forms.Label();
            this.button_ShiftUp = new System.Windows.Forms.Button();
            this.panel_Frame = new System.Windows.Forms.Panel();
            this.panel = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_Parameter = new System.Windows.Forms.TextBox();
            this.textBox_LastCheckTime = new System.Windows.Forms.TextBox();
            this.textBox_SourceName = new System.Windows.Forms.TextBox();
            this.groupBox_SourceInfo.SuspendLayout();
            this.panel_Frame.SuspendLayout();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "SaveDirPath";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "ModelPath";
            // 
            // textBox_SaveDirPath
            // 
            this.textBox_SaveDirPath.Location = new System.Drawing.Point(80, 2);
            this.textBox_SaveDirPath.Name = "textBox_SaveDirPath";
            this.textBox_SaveDirPath.Size = new System.Drawing.Size(244, 19);
            this.textBox_SaveDirPath.TabIndex = 2;
            this.textBox_SaveDirPath.TextChanged += new System.EventHandler(this.textBox_SaveDirPath_TextChanged);
            // 
            // textBox_ModelPath
            // 
            this.textBox_ModelPath.Location = new System.Drawing.Point(80, 27);
            this.textBox_ModelPath.Name = "textBox_ModelPath";
            this.textBox_ModelPath.Size = new System.Drawing.Size(244, 19);
            this.textBox_ModelPath.TabIndex = 3;
            this.textBox_ModelPath.TextChanged += new System.EventHandler(this.textBox_ModelPath_TextChanged);
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
            // groupBox_SourceInfo
            // 
            this.groupBox_SourceInfo.Controls.Add(this.button_ShiftDown);
            this.groupBox_SourceInfo.Controls.Add(this.button_DeleteThis);
            this.groupBox_SourceInfo.Controls.Add(this.button_Lamp);
            this.groupBox_SourceInfo.Controls.Add(this.label_LatestAnswerTime);
            this.groupBox_SourceInfo.Controls.Add(this.button_ShiftUp);
            this.groupBox_SourceInfo.Controls.Add(this.panel_Frame);
            this.groupBox_SourceInfo.Controls.Add(this.label_LatestAnswer);
            this.groupBox_SourceInfo.Location = new System.Drawing.Point(0, 0);
            this.groupBox_SourceInfo.Name = "groupBox_SourceInfo";
            this.groupBox_SourceInfo.Size = new System.Drawing.Size(400, 272);
            this.groupBox_SourceInfo.TabIndex = 5;
            this.groupBox_SourceInfo.TabStop = false;
            this.groupBox_SourceInfo.Text = "SourceName";
            // 
            // button_ShiftDown
            // 
            this.button_ShiftDown.Location = new System.Drawing.Point(3, 51);
            this.button_ShiftDown.Name = "button_ShiftDown";
            this.button_ShiftDown.Size = new System.Drawing.Size(18, 21);
            this.button_ShiftDown.TabIndex = 6;
            this.button_ShiftDown.Text = "<";
            this.button_ShiftDown.UseVisualStyleBackColor = true;
            this.button_ShiftDown.Click += new System.EventHandler(this.button_ShiftDown_Click);
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
            // button_ShiftUp
            // 
            this.button_ShiftUp.Location = new System.Drawing.Point(3, 28);
            this.button_ShiftUp.Name = "button_ShiftUp";
            this.button_ShiftUp.Size = new System.Drawing.Size(18, 21);
            this.button_ShiftUp.TabIndex = 1;
            this.button_ShiftUp.Text = ">";
            this.button_ShiftUp.UseVisualStyleBackColor = true;
            this.button_ShiftUp.Click += new System.EventHandler(this.button_Shift_Click);
            // 
            // panel_Frame
            // 
            this.panel_Frame.Controls.Add(this.panel);
            this.panel_Frame.Location = new System.Drawing.Point(27, 28);
            this.panel_Frame.Name = "panel_Frame";
            this.panel_Frame.Size = new System.Drawing.Size(335, 214);
            this.panel_Frame.TabIndex = 0;
            // 
            // panel
            // 
            this.panel.Controls.Add(this.label2);
            this.panel.Controls.Add(this.label5);
            this.panel.Controls.Add(this.label3);
            this.panel.Controls.Add(this.label4);
            this.panel.Controls.Add(this.label1);
            this.panel.Controls.Add(this.textBox_Parameter);
            this.panel.Controls.Add(this.textBox_LastCheckTime);
            this.panel.Controls.Add(this.textBox_SourceName);
            this.panel.Controls.Add(this.textBox_SaveDirPath);
            this.panel.Controls.Add(this.textBox_ModelPath);
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(335, 152);
            this.panel.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 110);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "Parameter";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "LastCheckTime";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "Name";
            // 
            // textBox_Parameter
            // 
            this.textBox_Parameter.Location = new System.Drawing.Point(10, 125);
            this.textBox_Parameter.Name = "textBox_Parameter";
            this.textBox_Parameter.Size = new System.Drawing.Size(314, 19);
            this.textBox_Parameter.TabIndex = 2;
            this.textBox_Parameter.TextChanged += new System.EventHandler(this.textBox_Parameter_TextChanged);
            // 
            // textBox_LastCheckTime
            // 
            this.textBox_LastCheckTime.Location = new System.Drawing.Point(98, 76);
            this.textBox_LastCheckTime.Name = "textBox_LastCheckTime";
            this.textBox_LastCheckTime.Size = new System.Drawing.Size(139, 19);
            this.textBox_LastCheckTime.TabIndex = 2;
            this.textBox_LastCheckTime.TextChanged += new System.EventHandler(this.textBox_LastCheckTime_TextChanged);
            // 
            // textBox_SourceName
            // 
            this.textBox_SourceName.Location = new System.Drawing.Point(46, 55);
            this.textBox_SourceName.Name = "textBox_SourceName";
            this.textBox_SourceName.Size = new System.Drawing.Size(204, 19);
            this.textBox_SourceName.TabIndex = 2;
            this.textBox_SourceName.TextChanged += new System.EventHandler(this.textBox_SourceName_TextChanged);
            // 
            // SourceInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox_SourceInfo);
            this.Name = "SourceInfo";
            this.Size = new System.Drawing.Size(400, 296);
            this.Load += new System.EventHandler(this.SourceInfo_Load);
            this.groupBox_SourceInfo.ResumeLayout(false);
            this.panel_Frame.ResumeLayout(false);
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_SaveDirPath;
        private System.Windows.Forms.TextBox textBox_ModelPath;
        private System.Windows.Forms.Label label_LatestAnswer;
        private System.Windows.Forms.Button button_Lamp;
        private System.Windows.Forms.GroupBox groupBox_SourceInfo;
        private System.Windows.Forms.Panel panel_Frame;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_SourceName;
        private System.Windows.Forms.Button button_ShiftUp;
        private System.Windows.Forms.Label label_LatestAnswerTime;
        private System.Windows.Forms.Button button_DeleteThis;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_LastCheckTime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_Parameter;
        private System.Windows.Forms.Button button_ShiftDown;
    }
}
