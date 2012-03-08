namespace HIT_JWC_Helper
{
    partial class GetDecryptPwd
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
            this.submit = new System.Windows.Forms.Button();
            this.pwd1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cancer = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // submit
            // 
            this.submit.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.submit.Location = new System.Drawing.Point(56, 105);
            this.submit.Name = "submit";
            this.submit.Size = new System.Drawing.Size(81, 42);
            this.submit.TabIndex = 0;
            this.submit.Text = "确认";
            this.submit.UseVisualStyleBackColor = true;
            this.submit.Click += new System.EventHandler(this.submit_Click);
            // 
            // pwd1
            // 
            this.pwd1.Location = new System.Drawing.Point(56, 54);
            this.pwd1.MaxLength = 8;
            this.pwd1.Name = "pwd1";
            this.pwd1.Size = new System.Drawing.Size(175, 21);
            this.pwd1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "密码";
            // 
            // cancer
            // 
            this.cancer.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancer.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cancer.Location = new System.Drawing.Point(150, 105);
            this.cancer.Name = "cancer";
            this.cancer.Size = new System.Drawing.Size(81, 42);
            this.cancer.TabIndex = 0;
            this.cancer.Text = "取消";
            this.cancer.UseVisualStyleBackColor = true;
            // 
            // GetDecryptPwd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(290, 193);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pwd1);
            this.Controls.Add(this.cancer);
            this.Controls.Add(this.submit);
            this.Name = "GetDecryptPwd";
            this.Text = "请输入密码";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button submit;
        private System.Windows.Forms.TextBox pwd1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cancer;
    }
}