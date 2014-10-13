namespace sqlConStringCreater
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
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.tb_sqlserver = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_userid = new System.Windows.Forms.TextBox();
            this.tb_pwd = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.bt_write = new System.Windows.Forms.Button();
            this.bt_read = new System.Windows.Forms.Button();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 70);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(382, 103);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // tb_sqlserver
            // 
            this.tb_sqlserver.Location = new System.Drawing.Point(138, 14);
            this.tb_sqlserver.Name = "tb_sqlserver";
            this.tb_sqlserver.Size = new System.Drawing.Size(256, 22);
            this.tb_sqlserver.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "sqlServerAddress";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "UserId";
            // 
            // tb_userid
            // 
            this.tb_userid.Location = new System.Drawing.Point(67, 42);
            this.tb_userid.Name = "tb_userid";
            this.tb_userid.Size = new System.Drawing.Size(133, 22);
            this.tb_userid.TabIndex = 4;
            // 
            // tb_pwd
            // 
            this.tb_pwd.Location = new System.Drawing.Point(261, 42);
            this.tb_pwd.Name = "tb_pwd";
            this.tb_pwd.Size = new System.Drawing.Size(133, 22);
            this.tb_pwd.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(221, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "Pwd";
            // 
            // bt_write
            // 
            this.bt_write.Location = new System.Drawing.Point(12, 179);
            this.bt_write.Name = "bt_write";
            this.bt_write.Size = new System.Drawing.Size(75, 30);
            this.bt_write.TabIndex = 7;
            this.bt_write.Text = "生成";
            this.bt_write.UseVisualStyleBackColor = true;
            this.bt_write.Click += new System.EventHandler(this.bt_write_Click);
            // 
            // bt_read
            // 
            this.bt_read.Location = new System.Drawing.Point(93, 179);
            this.bt_read.Name = "bt_read";
            this.bt_read.Size = new System.Drawing.Size(75, 30);
            this.bt_read.TabIndex = 8;
            this.bt_read.Text = "读取";
            this.bt_read.UseVisualStyleBackColor = true;
            this.bt_read.Click += new System.EventHandler(this.bt_read_Click);
            // 
            // richTextBox2
            // 
            this.richTextBox2.Location = new System.Drawing.Point(7, 216);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(382, 103);
            this.richTextBox2.TabIndex = 9;
            this.richTextBox2.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(401, 331);
            this.Controls.Add(this.richTextBox2);
            this.Controls.Add(this.bt_read);
            this.Controls.Add(this.bt_write);
            this.Controls.Add(this.tb_pwd);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tb_userid);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tb_sqlserver);
            this.Controls.Add(this.richTextBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.TextBox tb_sqlserver;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_userid;
        private System.Windows.Forms.TextBox tb_pwd;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button bt_write;
        private System.Windows.Forms.Button bt_read;
        private System.Windows.Forms.RichTextBox richTextBox2;
    }
}

