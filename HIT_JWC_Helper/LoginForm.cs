using System;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace HIT_JWC_Helper
{
    public partial class LoginForm : Form
    {
        Web web;
        
        const string codeImgUrl = "http://xscj.hit.edu.cn/hitjwgl/public/getcode.asp";
        const string loginUrl = "http://xscj.hit.edu.cn/hitjwgl/xs/Login.asp";
        public string username;
        public LoginForm(ref Web web)
        {
            InitializeComponent();
            this.web = web;
            web.getPage("http://xscj.hit.edu.cn/hitjwgl/xs/log.asp",Encoding.GetEncoding("GB2312"),null,true);
            codeImg.Image = web.getCodeImg(codeImgUrl, "http://xscj.hit.edu.cn/hitjwgl/xs/log.asp");
            stuNum.Text = INI.ReadIniData("user", "uid", "", @".\helper.ini");
            string password = INI.ReadIniData("user", "pwd", "", @".\helper.ini");

            if (password != "")
                this.remember.Checked = true;
            if(password != "")
                pwd.Text = Crypt.Decrypt(password);  
            this.AcceptButton = login;

        }

        private void login_Click(object sender, EventArgs e)
        {
            string postData = "uid={0}&pwd={1}&yzm={2}&Submit2.x=18&Submit2.y=18&Submit2=%CC%E1%BD%BB";
            postData = String.Format(postData, stuNum.Text.ToString(), pwd.Text.ToString(), code.Text.ToString());
            string ret = web.postData(loginUrl, postData, Encoding.GetEncoding("GB2312"), "http://xscj.hit.edu.cn/hitjwgl/xs/log.asp", true);
            if (ret.Contains("验证码输入错误"))
            {
                MessageBox.Show("验证码错误");
                codeImg.Image = web.getCodeImg(codeImgUrl, "http://xscj.hit.edu.cn/hitjwgl/xs/log.asp");
            }
            else if (ret.Contains("学号或密码错误"))
            {
                MessageBox.Show("学号或密码错误");
                codeImg.Image = web.getCodeImg(codeImgUrl, "http://xscj.hit.edu.cn/hitjwgl/xs/log.asp");
            }
            else
            {
                if (remember.Checked)
                {
                    INI.WriteIniData("user", "uid", stuNum.Text.ToString(), @".\helper.ini");
                    string password = pwd.Text.ToString();
                    password = Crypt.Encrypt(password);
                    INI.WriteIniData("user", "pwd", password, @".\helper.ini");
                }
                else
                {
                    // 取消记忆
                    INI.WriteIniData("user", "uid", "", @".\helper.ini");
                    INI.WriteIniData("user", "pwd", "", @".\helper.ini");
                }
                username = Regex.Match(ret, "<SPAN>欢迎您：([^<]+)", RegexOptions.Multiline | RegexOptions.IgnoreCase).Groups[1].ToString();
                this.DialogResult = DialogResult.OK;
            }
        }

        private void codeImg_Click(object sender, EventArgs e)
        {
            codeImg.Image = web.getCodeImg(codeImgUrl, "http://xscj.hit.edu.cn/hitjwgl/xs/log.asp");
        }

        private void button1_Click(object sender, EventArgs e)
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
