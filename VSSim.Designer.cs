namespace VSSimulator
{
    partial class VSSim
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.vsPortTextBox = new System.Windows.Forms.TextBox();
            this.connectButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.sendDataButton = new System.Windows.Forms.Button();
            this.statusTextBox = new System.Windows.Forms.TextBox();
            this.initAppButton = new System.Windows.Forms.Button();
            this.echoRecvdCheckBox = new System.Windows.Forms.CheckBox();
            this.sendCountTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // vsPortTextBox
            // 
            this.vsPortTextBox.Location = new System.Drawing.Point(439, 147);
            this.vsPortTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.vsPortTextBox.Name = "vsPortTextBox";
            this.vsPortTextBox.Size = new System.Drawing.Size(124, 38);
            this.vsPortTextBox.TabIndex = 0;
            this.vsPortTextBox.Text = "4024";
            this.vsPortTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(29, 137);
            this.connectButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(184, 62);
            this.connectButton.TabIndex = 1;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(274, 153);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 32);
            this.label1.TabIndex = 2;
            this.label1.Text = "VS Port";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // sendDataButton
            // 
            this.sendDataButton.Location = new System.Drawing.Point(32, 223);
            this.sendDataButton.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.sendDataButton.Name = "sendDataButton";
            this.sendDataButton.Size = new System.Drawing.Size(184, 62);
            this.sendDataButton.TabIndex = 3;
            this.sendDataButton.Text = "Send Data";
            this.sendDataButton.UseVisualStyleBackColor = true;
            this.sendDataButton.Click += new System.EventHandler(this.sendDataButton_Click);
            // 
            // statusTextBox
            // 
            this.statusTextBox.Location = new System.Drawing.Point(32, 323);
            this.statusTextBox.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.statusTextBox.Multiline = true;
            this.statusTextBox.Name = "statusTextBox";
            this.statusTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.statusTextBox.Size = new System.Drawing.Size(1008, 671);
            this.statusTextBox.TabIndex = 5;
            // 
            // initAppButton
            // 
            this.initAppButton.Location = new System.Drawing.Point(32, 52);
            this.initAppButton.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.initAppButton.Name = "initAppButton";
            this.initAppButton.Size = new System.Drawing.Size(181, 62);
            this.initAppButton.TabIndex = 6;
            this.initAppButton.Text = "Init App";
            this.initAppButton.UseVisualStyleBackColor = true;
            this.initAppButton.Click += new System.EventHandler(this.initAppButton_Click);
            // 
            // echoRecvdCheckBox
            // 
            this.echoRecvdCheckBox.AutoSize = true;
            this.echoRecvdCheckBox.Location = new System.Drawing.Point(467, 31);
            this.echoRecvdCheckBox.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.echoRecvdCheckBox.Name = "echoRecvdCheckBox";
            this.echoRecvdCheckBox.Size = new System.Drawing.Size(272, 36);
            this.echoRecvdCheckBox.TabIndex = 7;
            this.echoRecvdCheckBox.Text = "Echo Recvd Data";
            this.echoRecvdCheckBox.UseVisualStyleBackColor = true;
            this.echoRecvdCheckBox.CheckedChanged += new System.EventHandler(this.echoRecvdCheckBox_CheckedChanged);
            // 
            // sendCountTextBox
            // 
            this.sendCountTextBox.Location = new System.Drawing.Point(439, 223);
            this.sendCountTextBox.Name = "sendCountTextBox";
            this.sendCountTextBox.Size = new System.Drawing.Size(120, 38);
            this.sendCountTextBox.TabIndex = 8;
            this.sendCountTextBox.Text = "123";
            this.sendCountTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(241, 239);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(145, 32);
            this.label2.TabIndex = 9;
            this.label2.Text = "Send Size";
            // 
            // VSSim
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1057, 1010);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.sendCountTextBox);
            this.Controls.Add(this.echoRecvdCheckBox);
            this.Controls.Add(this.initAppButton);
            this.Controls.Add(this.statusTextBox);
            this.Controls.Add(this.sendDataButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.vsPortTextBox);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "VSSim";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.VSSim_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox vsPortTextBox;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button sendDataButton;
        private System.Windows.Forms.TextBox statusTextBox;
        private System.Windows.Forms.Button initAppButton;
        internal System.Windows.Forms.CheckBox echoRecvdCheckBox;
        private System.Windows.Forms.TextBox sendCountTextBox;
        private System.Windows.Forms.Label label2;
    }
}

