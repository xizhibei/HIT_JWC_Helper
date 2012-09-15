using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
/*
 * Author:  Xu Zhipei
 * Email:   xuzhipei@gmail.com
 * Licence: MIT
 * */
namespace HIT_JWC_Helper
{
    public partial class GetDecryptPwd : Form
    {
        public string pwd;
        public GetDecryptPwd()
        {
            InitializeComponent();
        }

        private void submit_Click(object sender, EventArgs e)
        {
            pwd = pwd1.Text.ToString();
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
