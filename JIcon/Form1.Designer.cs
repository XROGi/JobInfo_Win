namespace JIcon
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.notifyIcon_ji = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.jnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.коммандыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.переподключитьсяToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.запуститьМессенджерToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.выгрузитИзПамятиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.телДляСправок4296ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button10 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon_ji
            // 
            this.notifyIcon_ji.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon_ji.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon_ji.Icon")));
            this.notifyIcon_ji.Text = "JIcon v1.06.26";
            this.notifyIcon_ji.Visible = true;
            this.notifyIcon_ji.BalloonTipClicked += new System.EventHandler(this.NotifyIcon_ji_Click);
            this.notifyIcon_ji.Click += new System.EventHandler(this.NotifyIcon_ji_Click);
            this.notifyIcon_ji.DoubleClick += new System.EventHandler(this.NotifyIcon_ji_Click);
            this.notifyIcon_ji.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon_ji_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.jnToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(193, 26);
            // 
            // jnToolStripMenuItem
            // 
            this.jnToolStripMenuItem.Name = "jnToolStripMenuItem";
            this.jnToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.jnToolStripMenuItem.Text = "Открыть мессенджер";
            this.jnToolStripMenuItem.Click += new System.EventHandler(this.JnToolStripMenuItem_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "user (1).png");
            this.imageList1.Images.SetKeyName(1, "chat.png");
            this.imageList1.Images.SetKeyName(2, "link.png");
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 57.03971F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 42.96029F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label4, 1, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(188, 27);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55.31915F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 44.68085F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(277, 94);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "Чатов с непрочтёнными собщениями:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(160, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "label2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Новых сообщений:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(160, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "label4";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.label6.Location = new System.Drawing.Point(12, 159);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "label6";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 135);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "label5";
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Image = global::JIcon.Properties.Resources.ji1;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button1.Location = new System.Drawing.Point(12, 27);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(170, 94);
            this.button1.TabIndex = 0;
            this.button1.Text = "Открыть мессенджер";
            this.button1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.коммандыToolStripMenuItem,
            this.телДляСправок4296ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(470, 24);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // коммандыToolStripMenuItem
            // 
            this.коммандыToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.переподключитьсяToolStripMenuItem,
            this.запуститьМессенджерToolStripMenuItem,
            this.toolStripMenuItem1,
            this.выгрузитИзПамятиToolStripMenuItem});
            this.коммандыToolStripMenuItem.Name = "коммандыToolStripMenuItem";
            this.коммандыToolStripMenuItem.Size = new System.Drawing.Size(79, 20);
            this.коммандыToolStripMenuItem.Text = "Комманды";
            // 
            // переподключитьсяToolStripMenuItem
            // 
            this.переподключитьсяToolStripMenuItem.Name = "переподключитьсяToolStripMenuItem";
            this.переподключитьсяToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.переподключитьсяToolStripMenuItem.Text = "Переподключиться";
            this.переподключитьсяToolStripMenuItem.Click += new System.EventHandler(this.ПереподключитьсяToolStripMenuItem_Click);
            // 
            // запуститьМессенджерToolStripMenuItem
            // 
            this.запуститьМессенджерToolStripMenuItem.Name = "запуститьМессенджерToolStripMenuItem";
            this.запуститьМессенджерToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.запуститьМессенджерToolStripMenuItem.Text = "Запустить мессенджер";
            this.запуститьМессенджерToolStripMenuItem.Click += new System.EventHandler(this.ЗапуститьМессенджерToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(197, 6);
            // 
            // выгрузитИзПамятиToolStripMenuItem
            // 
            this.выгрузитИзПамятиToolStripMenuItem.Name = "выгрузитИзПамятиToolStripMenuItem";
            this.выгрузитИзПамятиToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.выгрузитИзПамятиToolStripMenuItem.Text = "Выгрузить из памяти";
            this.выгрузитИзПамятиToolStripMenuItem.Click += new System.EventHandler(this.ВыгрузитИзПамятиToolStripMenuItem_Click);
            // 
            // телДляСправок4296ToolStripMenuItem
            // 
            this.телДляСправок4296ToolStripMenuItem.Name = "телДляСправок4296ToolStripMenuItem";
            this.телДляСправок4296ToolStripMenuItem.Size = new System.Drawing.Size(137, 20);
            this.телДляСправок4296ToolStripMenuItem.Text = "тел. для справок 4296";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 30000;
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // button10
            // 
            this.button10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button10.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button10.BackgroundImage")));
            this.button10.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button10.Location = new System.Drawing.Point(413, 120);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(52, 52);
            this.button10.TabIndex = 92;
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.Button10_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(242, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 13);
            this.label7.TabIndex = 93;
            this.label7.Text = "label7";
            this.label7.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(470, 173);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Информатор о сообщениях в JI мессенджере";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.contextMenuStrip1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon_ji;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem коммандыToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem переподключитьсяToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem запуститьМессенджерToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem выгрузитИзПамятиToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem jnToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.ToolStripMenuItem телДляСправок4296ToolStripMenuItem;
        private System.Windows.Forms.Label label7;
    }
}

