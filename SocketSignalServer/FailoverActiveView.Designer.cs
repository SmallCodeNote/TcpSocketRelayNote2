namespace SocketSignalServer
{
    partial class FailoverActiveView
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_DeleteThis = new System.Windows.Forms.Button();
            this.textBox_Port = new System.Windows.Forms.TextBox();
            this.textBox_Address = new System.Windows.Forms.TextBox();
            this.label_LastMessage = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button_Status = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_DeleteThis);
            this.groupBox1.Controls.Add(this.textBox_Port);
            this.groupBox1.Controls.Add(this.textBox_Address);
            this.groupBox1.Controls.Add(this.label_LastMessage);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.button_Status);
            this.groupBox1.Location = new System.Drawing.Point(3, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(330, 60);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // button_DeleteThis
            // 
            this.button_DeleteThis.Location = new System.Drawing.Point(302, 11);
            this.button_DeleteThis.Name = "button_DeleteThis";
            this.button_DeleteThis.Size = new System.Drawing.Size(22, 22);
            this.button_DeleteThis.TabIndex = 4;
            this.button_DeleteThis.Text = "X";
            this.button_DeleteThis.UseVisualStyleBackColor = true;
            this.button_DeleteThis.Click += new System.EventHandler(this.button_DeleteThis_Click);
            // 
            // textBox_Port
            // 
            this.textBox_Port.Location = new System.Drawing.Point(250, 11);
            this.textBox_Port.Name = "textBox_Port";
            this.textBox_Port.Size = new System.Drawing.Size(46, 19);
            this.textBox_Port.TabIndex = 3;
            this.textBox_Port.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBox_Port.TextChanged += new System.EventHandler(this.textBox_Port_TextChanged);
            // 
            // textBox_Address
            // 
            this.textBox_Address.Location = new System.Drawing.Point(89, 11);
            this.textBox_Address.Name = "textBox_Address";
            this.textBox_Address.Size = new System.Drawing.Size(131, 19);
            this.textBox_Address.TabIndex = 3;
            this.textBox_Address.TextChanged += new System.EventHandler(this.textBox_Address_TextChanged);
            // 
            // label_LastMessage
            // 
            this.label_LastMessage.AutoSize = true;
            this.label_LastMessage.Location = new System.Drawing.Point(36, 40);
            this.label_LastMessage.Name = "label_LastMessage";
            this.label_LastMessage.Size = new System.Drawing.Size(11, 12);
            this.label_LastMessage.TabIndex = 2;
            this.label_LastMessage.Text = "...";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(218, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Port";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(36, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Address";
            // 
            // button_Status
            // 
            this.button_Status.Location = new System.Drawing.Point(6, 11);
            this.button_Status.Name = "button_Status";
            this.button_Status.Size = new System.Drawing.Size(24, 41);
            this.button_Status.TabIndex = 0;
            this.button_Status.UseVisualStyleBackColor = true;
            // 
            // FailoverActiveView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "FailoverActiveView";
            this.Size = new System.Drawing.Size(340, 65);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_Status;
        private System.Windows.Forms.TextBox textBox_Port;
        private System.Windows.Forms.TextBox textBox_Address;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label_LastMessage;
        private System.Windows.Forms.Button button_DeleteThis;
    }
}
