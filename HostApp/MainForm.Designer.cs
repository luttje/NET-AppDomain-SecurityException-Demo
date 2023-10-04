namespace HostApp
{
    partial class MainForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lstMessages = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkProblems = new System.Windows.Forms.CheckBox();
            this.lblHint = new System.Windows.Forms.Label();
            this.chkWorkaround = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lstMessages);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupBox1.Location = new System.Drawing.Point(462, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(338, 450);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Messages";
            // 
            // lstMessages
            // 
            this.lstMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstMessages.FormattingEnabled = true;
            this.lstMessages.Location = new System.Drawing.Point(3, 16);
            this.lstMessages.Name = "lstMessages";
            this.lstMessages.Size = new System.Drawing.Size(332, 257);
            this.lstMessages.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chkProblems);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.lblHint);
            this.panel1.Controls.Add(this.chkWorkaround);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 273);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(10);
            this.panel1.Size = new System.Drawing.Size(332, 174);
            this.panel1.TabIndex = 1;
            // 
            // chkProblems
            // 
            this.chkProblems.AutoSize = true;
            this.chkProblems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkProblems.Location = new System.Drawing.Point(10, 49);
            this.chkProblems.Name = "chkProblems";
            this.chkProblems.Size = new System.Drawing.Size(312, 42);
            this.chkProblems.TabIndex = 0;
            this.chkProblems.Text = "Toggle Problems (by performing actions WndProc that cause SecurityExceptions when" +
    " call from across the AppDomain)";
            this.chkProblems.UseVisualStyleBackColor = true;
            this.chkProblems.CheckedChanged += new System.EventHandler(this.chkProblems_CheckedChanged);
            // 
            // lblHint
            // 
            this.lblHint.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHint.Location = new System.Drawing.Point(10, 91);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(312, 48);
            this.lblHint.TabIndex = 1;
            this.lblHint.Text = "Alright, now go interact with that plugin by clicking the button on the left.";
            this.lblHint.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblHint.Visible = false;
            // 
            // chkWorkaround
            // 
            this.chkWorkaround.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.chkWorkaround.Location = new System.Drawing.Point(10, 139);
            this.chkWorkaround.Name = "chkWorkaround";
            this.chkWorkaround.Size = new System.Drawing.Size(312, 25);
            this.chkWorkaround.TabIndex = 2;
            this.chkWorkaround.Text = "Toggle Undesirable Workaround";
            this.chkWorkaround.UseVisualStyleBackColor = true;
            this.chkWorkaround.Visible = false;
            this.chkWorkaround.CheckedChanged += new System.EventHandler(this.chkWorkaround_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(10, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(312, 39);
            this.label1.TabIndex = 3;
            this.label1.Text = "Note that the problems mentioned have been fixed in this branch :)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.groupBox1);
            this.Name = "MainForm";
            this.Text = "Main Form - Sandboxer";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lstMessages;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chkProblems;
        private System.Windows.Forms.Label lblHint;
        private System.Windows.Forms.CheckBox chkWorkaround;
        private System.Windows.Forms.Label label1;
    }
}

