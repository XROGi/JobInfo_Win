namespace JobInfo
{
    partial class Form1
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
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.x_WPF_MsgList1 = new XWpfControlLibrary.X_WPF_MsgList();
            this.elementHost2 = new System.Windows.Forms.Integration.ElementHost();
            this.x_WPF_Msg1 = new XWpfControlLibrary.X_WPF_Msg();
            this.elementHost3 = new System.Windows.Forms.Integration.ElementHost();
            this.userControl11 = new XWpfControlLibrary.UserControl1();
            this.SuspendLayout();
            // 
            // elementHost1
            // 
            this.elementHost1.Location = new System.Drawing.Point(39, 12);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(352, 315);
            this.elementHost1.TabIndex = 0;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = this.x_WPF_MsgList1;
            // 
            // elementHost2
            // 
            this.elementHost2.Location = new System.Drawing.Point(406, 12);
            this.elementHost2.Name = "elementHost2";
            this.elementHost2.Size = new System.Drawing.Size(200, 100);
            this.elementHost2.TabIndex = 1;
            this.elementHost2.Text = "elementHost2";
            this.elementHost2.Child = this.x_WPF_Msg1;
            // 
            // elementHost3
            // 
            this.elementHost3.Location = new System.Drawing.Point(39, 338);
            this.elementHost3.Name = "elementHost3";
            this.elementHost3.Size = new System.Drawing.Size(749, 100);
            this.elementHost3.TabIndex = 2;
            this.elementHost3.Text = "elementHost3";
            this.elementHost3.Child = this.userControl11;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.elementHost3);
            this.Controls.Add(this.elementHost2);
            this.Controls.Add(this.elementHost1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private XWpfControlLibrary.X_WPF_MsgList x_WPF_MsgList1;
        private System.Windows.Forms.Integration.ElementHost elementHost2;
        private XWpfControlLibrary.X_WPF_Msg x_WPF_Msg1;
        private System.Windows.Forms.Integration.ElementHost elementHost3;
        private XWpfControlLibrary.UserControl1 userControl11;
    }
}