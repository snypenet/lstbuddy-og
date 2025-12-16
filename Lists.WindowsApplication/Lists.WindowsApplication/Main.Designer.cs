namespace Lists.WindowsApplication
{
    partial class Main
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
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.lblShowFrom = new System.Windows.Forms.Label();
            this.lblShowTo = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.radTypeWarning = new System.Windows.Forms.RadioButton();
            this.radTypeAnnouncement = new System.Windows.Forms.RadioButton();
            this.radTypeInformation = new System.Windows.Forms.RadioButton();
            this.grpMessageType = new System.Windows.Forms.GroupBox();
            this.dtpShowFrom = new System.Windows.Forms.DateTimePicker();
            this.dtpShowTo = new System.Windows.Forms.DateTimePicker();
            this.lblNotification = new System.Windows.Forms.Label();
            this.grpMessageType.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(12, 60);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(217, 71);
            this.txtMessage.TabIndex = 0;
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(12, 248);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(75, 23);
            this.btnSubmit.TabIndex = 3;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(129, 248);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 4;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lblShowFrom
            // 
            this.lblShowFrom.AutoSize = true;
            this.lblShowFrom.Location = new System.Drawing.Point(9, 143);
            this.lblShowFrom.Name = "lblShowFrom";
            this.lblShowFrom.Size = new System.Drawing.Size(60, 13);
            this.lblShowFrom.TabIndex = 5;
            this.lblShowFrom.Text = "Show From";
            // 
            // lblShowTo
            // 
            this.lblShowTo.AutoSize = true;
            this.lblShowTo.Location = new System.Drawing.Point(12, 194);
            this.lblShowTo.Name = "lblShowTo";
            this.lblShowTo.Size = new System.Drawing.Size(50, 13);
            this.lblShowTo.TabIndex = 6;
            this.lblShowTo.Text = "Show To";
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(9, 44);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(50, 13);
            this.lblMessage.TabIndex = 7;
            this.lblMessage.Text = "Message";
            // 
            // radTypeWarning
            // 
            this.radTypeWarning.AutoSize = true;
            this.radTypeWarning.Location = new System.Drawing.Point(6, 23);
            this.radTypeWarning.Name = "radTypeWarning";
            this.radTypeWarning.Size = new System.Drawing.Size(65, 17);
            this.radTypeWarning.TabIndex = 8;
            this.radTypeWarning.TabStop = true;
            this.radTypeWarning.Text = "Warning";
            this.radTypeWarning.UseVisualStyleBackColor = true;
            // 
            // radTypeAnnouncement
            // 
            this.radTypeAnnouncement.AutoSize = true;
            this.radTypeAnnouncement.Location = new System.Drawing.Point(6, 46);
            this.radTypeAnnouncement.Name = "radTypeAnnouncement";
            this.radTypeAnnouncement.Size = new System.Drawing.Size(97, 17);
            this.radTypeAnnouncement.TabIndex = 9;
            this.radTypeAnnouncement.TabStop = true;
            this.radTypeAnnouncement.Text = "Announcement";
            this.radTypeAnnouncement.UseVisualStyleBackColor = true;
            // 
            // radTypeInformation
            // 
            this.radTypeInformation.AutoSize = true;
            this.radTypeInformation.Location = new System.Drawing.Point(6, 68);
            this.radTypeInformation.Name = "radTypeInformation";
            this.radTypeInformation.Size = new System.Drawing.Size(77, 17);
            this.radTypeInformation.TabIndex = 10;
            this.radTypeInformation.TabStop = true;
            this.radTypeInformation.Text = "Information";
            this.radTypeInformation.UseVisualStyleBackColor = true;
            // 
            // grpMessageType
            // 
            this.grpMessageType.Controls.Add(this.radTypeWarning);
            this.grpMessageType.Controls.Add(this.radTypeAnnouncement);
            this.grpMessageType.Controls.Add(this.radTypeInformation);
            this.grpMessageType.Location = new System.Drawing.Point(246, 46);
            this.grpMessageType.Name = "grpMessageType";
            this.grpMessageType.Size = new System.Drawing.Size(171, 114);
            this.grpMessageType.TabIndex = 12;
            this.grpMessageType.TabStop = false;
            this.grpMessageType.Text = "Message Type";
            // 
            // dtpShowFrom
            // 
            this.dtpShowFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpShowFrom.Location = new System.Drawing.Point(12, 159);
            this.dtpShowFrom.Name = "dtpShowFrom";
            this.dtpShowFrom.Size = new System.Drawing.Size(217, 20);
            this.dtpShowFrom.TabIndex = 13;
            this.dtpShowFrom.Value = System.DateTime.Today;
            // 
            // dtpShowTo
            // 
            this.dtpShowTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpShowTo.Location = new System.Drawing.Point(12, 210);
            this.dtpShowTo.Name = "dtpShowTo";
            this.dtpShowTo.Size = new System.Drawing.Size(217, 20);
            this.dtpShowTo.TabIndex = 14;
            this.dtpShowTo.Value = System.DateTime.Today.AddDays(1);
            // 
            // lblNotification
            // 
            this.lblNotification.AutoSize = true;
            this.lblNotification.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNotification.Location = new System.Drawing.Point(15, 13);
            this.lblNotification.Name = "lblNotification";
            this.lblNotification.Size = new System.Drawing.Size(0, 18);
            this.lblNotification.TabIndex = 15;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(429, 286);
            this.Controls.Add(this.lblNotification);
            this.Controls.Add(this.dtpShowTo);
            this.Controls.Add(this.dtpShowFrom);
            this.Controls.Add(this.grpMessageType);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.lblShowTo);
            this.Controls.Add(this.lblShowFrom);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.txtMessage);
            this.Name = "Main";
            this.Text = "Lists Management";
            this.Load += new System.EventHandler(this.Main_Load);
            this.grpMessageType.ResumeLayout(false);
            this.grpMessageType.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lblShowFrom;
        private System.Windows.Forms.Label lblShowTo;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.RadioButton radTypeWarning;
        private System.Windows.Forms.RadioButton radTypeAnnouncement;
        private System.Windows.Forms.RadioButton radTypeInformation;
        private System.Windows.Forms.GroupBox grpMessageType;
        private System.Windows.Forms.DateTimePicker dtpShowFrom;
        private System.Windows.Forms.DateTimePicker dtpShowTo;
        private System.Windows.Forms.Label lblNotification;
    }
}

