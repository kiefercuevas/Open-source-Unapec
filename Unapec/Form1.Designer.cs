namespace Unapec
{
    partial class UnapecForm
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
            this.lbUserName = new System.Windows.Forms.Label();
            this.TBXuserName = new System.Windows.Forms.TextBox();
            this.TBXpassword = new System.Windows.Forms.TextBox();
            this.LBpassword = new System.Windows.Forms.Label();
            this.BTNinitProcess = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbUserName
            // 
            this.lbUserName.AutoSize = true;
            this.lbUserName.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbUserName.Location = new System.Drawing.Point(12, 18);
            this.lbUserName.Name = "lbUserName";
            this.lbUserName.Size = new System.Drawing.Size(211, 25);
            this.lbUserName.TabIndex = 0;
            this.lbUserName.Text = "Nombre de usuario";
            // 
            // TBXuserName
            // 
            this.TBXuserName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TBXuserName.Location = new System.Drawing.Point(17, 50);
            this.TBXuserName.Name = "TBXuserName";
            this.TBXuserName.Size = new System.Drawing.Size(255, 26);
            this.TBXuserName.TabIndex = 1;
            // 
            // TBXpassword
            // 
            this.TBXpassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TBXpassword.Location = new System.Drawing.Point(17, 127);
            this.TBXpassword.Name = "TBXpassword";
            this.TBXpassword.PasswordChar = '*';
            this.TBXpassword.Size = new System.Drawing.Size(255, 26);
            this.TBXpassword.TabIndex = 3;
            // 
            // LBpassword
            // 
            this.LBpassword.AutoSize = true;
            this.LBpassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LBpassword.Location = new System.Drawing.Point(12, 96);
            this.LBpassword.Name = "LBpassword";
            this.LBpassword.Size = new System.Drawing.Size(72, 25);
            this.LBpassword.TabIndex = 2;
            this.LBpassword.Text = "Clave";
            // 
            // BTNinitProcess
            // 
            this.BTNinitProcess.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BTNinitProcess.Location = new System.Drawing.Point(17, 177);
            this.BTNinitProcess.Name = "BTNinitProcess";
            this.BTNinitProcess.Size = new System.Drawing.Size(255, 45);
            this.BTNinitProcess.TabIndex = 4;
            this.BTNinitProcess.Text = "Iniciar proceso";
            this.BTNinitProcess.UseVisualStyleBackColor = true;
            this.BTNinitProcess.Click += new System.EventHandler(this.BTNinitProcess_Click);
            // 
            // UnapecForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 240);
            this.Controls.Add(this.BTNinitProcess);
            this.Controls.Add(this.TBXpassword);
            this.Controls.Add(this.LBpassword);
            this.Controls.Add(this.TBXuserName);
            this.Controls.Add(this.lbUserName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "UnapecForm";
            this.Text = "Unapec";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbUserName;
        private System.Windows.Forms.TextBox TBXuserName;
        private System.Windows.Forms.TextBox TBXpassword;
        private System.Windows.Forms.Label LBpassword;
        private System.Windows.Forms.Button BTNinitProcess;
    }
}

