namespace HIT_JWC_Helper
{
    partial class GetEncryptPwd
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
            this.cancer = new System.Windows.Forms.Button();
            this.encrypt = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pwd2 = new System.Windows.Forms.TextBox();
            this.pwd1 = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancer
            // 
            this.cancer.Location = new System.Drawing.Point(29, 61);
            this.cancer.Name = "cancer";
            this.cancer.Size = new System.Drawing.Size(87, 60);
            this.cancer.TabIndex = 0;
            this.cancer.Text = "不需要";
            this.cancer.UseVisualStyleBackColor = true;
            this.cancer.Click += new System.EventHandler(this.cancer_Click);
            // 
            // encrypt
            // 
            this.encrypt.Location = new System.Drawing.Point(90, 127);
            this.encrypt.Name = "encrypt";
            this.encrypt.Size = new System.Drawing.Size(79, 37);
            this.encrypt.TabIndex = 1;
            this.encrypt.Text = "加密";
            this.encrypt.UseVisualStyleBackColor = true;
            this.encrypt.Click += new System.EventHandler(this.encrypt_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.pwd2);
            this.groupBox1.Controls.Add(this.pwd1);
            this.groupBox1.Controls.Add(this.encrypt);
            this.groupBox1.Location = new System.Drawing.Point(146, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(217, 180);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "需要加密";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 97);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "再一次";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "密  码";
            // 
            // pwd2
            // 
            this.pwd2.Location = new System.Drawing.Point(65, 88);
            this.pwd2.MaxLength = 8;
            this.pwd2.Name = "pwd2";
            this.pwd2.PasswordChar = 'X';
            this.pwd2.Size = new System.Drawing.Size(146, 21);
            this.pwd2.TabIndex = 3;
            // 
            // pwd1
            // 
            this.pwd1.Location = new System.Drawing.Point(65, 30);
            this.pwd1.MaxLength = 8;
            this.pwd1.Name = "pwd1";
            this.pwd1.PasswordChar = 'X';
            this.pwd1.Size = new System.Drawing.Size(146, 21);
            this.pwd1.TabIndex = 2;
            // 
            // GetEncryptPwd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 204);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cancer);
            this.Name = "GetEncryptPwd";
            this.Text = "是否需要密码";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancer;
        private System.Windows.Forms.Button encrypt;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox pwd2;
        private System.Windows.Forms.TextBox pwd1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}