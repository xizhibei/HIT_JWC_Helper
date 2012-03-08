using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace HIT_JWC_Helper
{
    public partial class GetEncryptPwd : Form
    {
        public string pwd;
        public GetEncryptPwd()
        {
            InitializeComponent();
        }

        private void encrypt_Click(object sender, EventArgs e)
        {
            if (pwd1.Text.ToString() == "")
            {
                MessageBox.Show("亲，你逗我呢！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (pwd1.Text.ToString() == pwd2.Text.ToString())
            {
                pwd = pwd2.Text.ToString();
                char[] p = new char[8];
                for (int i = 0; i < 8; i++)
                    p[i] = '0';
                if (pwd.Length < 8)
                    pwd.CopyTo(0, p, 0, pwd.Length);
                else
                    pwd.CopyTo(0, p, 0, 8);
                pwd = new String(p);
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("亲，俩密码不一样哦！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void cancer_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
        protected override void WndProc(ref   Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_CLOSE = 0xF060;
            if (m.Msg == WM_SYSCOMMAND && (int)m.WParam == SC_CLOSE)
            {
                //this.WindowState = FormWindowState.Minimized;
                return;
            }
            base.WndProc(ref   m);
        } 
    }
}
