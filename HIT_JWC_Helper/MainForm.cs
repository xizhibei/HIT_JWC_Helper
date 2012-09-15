using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Web;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using System.Drawing.Drawing2D;
/*
 * Author:  Xu Zhipei
 * Email:   xuzhipei@gmail.com
 * Licence: MIT
 * */
namespace HIT_JWC_Helper
{
    public partial class MainForm : Form
    {
        Web web;
        ScoreManager sm;

        private void InitValue()
        {
            web = new Web();
            sm = new ScoreManager();

            if (!File.Exists("helper.ini"))
            {
                //INI.WriteIniData("basic", "DEPARTMENT_PATH", "department.txt", @".\helper.ini");
                INI.WriteIniData("user", "uid", "", @".\helper.ini");
                INI.WriteIniData("user", "pwd", "", @".\helper.ini");
            }

            term.Text = "全部学期";
            pro_class.Text = "第一学位";
        }

        public MainForm()
        {
            InitializeComponent();
            InitValue();
        }

        private void GetPlan()
        {
            try
            {
                string postData = "ZY={0}&NJ={1}&Submit=" + HttpUtility.UrlEncode("查询", Encoding.GetEncoding("GB2312"));
                //string[] department = File.ReadAllLines(Department_Path);
                string[] data = sm.ParsePlanID(
                    web.getPage("http://xscj.hit.edu.cn/hitjwgl/xs/kcxx/ZXJH.asp",
                    Encoding.GetEncoding("GB2312"),
                    null,
                    true)
                    );

                if (data == null)
                    throw new Exception("提取执行计划页面个人信息出错！！！");

                postData = String.Format(postData, data[0], data[1]);

                string ret = web.postData("http://xscj.hit.edu.cn/hitjwgl/xs/kcxx/ZXJH.asp", 
                    postData, 
                    Encoding.GetEncoding("GB2312"), 
                    null, 
                    true
                    );
                sm.ParsePlan(ret);
            }
            catch (Exception excp)
            {
                Error.RecordLog(excp, "Opps,出现错误了！建议将相同目录下的helper.ini删除再试试！");
                return;
            }
        }

        private void BindData()
        {
            while (dataGridView1.RowCount > 0)
                dataGridView1.Rows.RemoveAt(0);

            int i = 0;
            if (CreditToolStripMenuItem.Checked == true)
            {
                foreach (Score tmp in sm.score)
                {
                    if ( sm.is_important.ContainsKey(tmp.cid) )
                    {
                        dataGridView1.Rows.Add();
                        dataGridView1.Rows[i].Cells[0].Value = !(bool)sm.ignore[tmp.name];
                        dataGridView1.Rows[i].Cells[1].Value = i + 1;
                        dataGridView1.Rows[i].Cells[2].Value = tmp.name;
                        dataGridView1.Rows[i].Cells[3].Value = tmp.credit;
                        dataGridView1.Rows[i].Cells[4].Value = tmp.score;
                        i++;
                    }
                }
            }
            else if (GPAToolStripMenuItem.Checked == true)
            {
                foreach (Score tmp in sm.score)
                {
                    if (tmp.score < 60) continue;
                    dataGridView1.Rows.Add();
                    dataGridView1.Rows[i].Cells[0].Value = !(bool)sm.ignore[tmp.name];
                    dataGridView1.Rows[i].Cells[1].Value = i + 1;
                    dataGridView1.Rows[i].Cells[2].Value = tmp.name;
                    dataGridView1.Rows[i].Cells[3].Value = tmp.credit;
                    dataGridView1.Rows[i].Cells[4].Value = tmp.score;
                    i++;
                }
            }
        }

        private void start_Click(object sender, EventArgs e)
        {
            if (term.Text.ToString() == "" || pro_class.Text.ToString() == "")
            {
                MessageBox.Show("请输入！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            sm.Clear();

            string t = term.Text.ToString() == "全部学期" ? "all" : term.Text.ToString();
            string lb = pro_class.Text.ToString() == "第一学位" ? "1" : "2";
            string postData = "selectXQ={0}&LB={1}&Submit=%B2%E9%D1%AF%B3%C9%BC%A8";
            postData = String.Format(postData, HttpUtility.UrlEncode(t, Encoding.GetEncoding("GB2312")), lb);
            string ret = web.postData("http://xscj.hit.edu.cn/hitjwgl/xs/cjcx/cx_1.asp", postData, Encoding.GetEncoding("GB2312"), null, true);

            if (sm.ParseScore(ret))
            {
                GetPlan();

                outcome.Text = sm.Calculate(CreditToolStripMenuItem.Checked);

                BindData();

                //MessageBox.Show("OK!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                toolStripStatusLabel1.Text = "获取数据成功，计算完毕！";

                this.CreditToolStripMenuItem.Enabled = true;
                this.GPAToolStripMenuItem.Enabled = true;
            }
            else
                outcome.Text = "没有查到相关信息！";

        }

        private void LoginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoginForm lf = new LoginForm(ref web);
            DialogResult dr = lf.ShowDialog();
            if (dr == DialogResult.OK)
            {
                term.Enabled = true;
                pro_class.Enabled = true;
                start.Enabled = true;
                toolStripStatusLabel1.Text = "登录成功!";
                string[] tmp = lf.username.Split('|');
                welcomeText.Text = "欢迎：" + tmp[0] + tmp[1];
            }
            lf.Close();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GPAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GPAToolStripMenuItem.Checked = true;
            CreditToolStripMenuItem.Checked = false;

            outcome.Text = sm.Calculate(CreditToolStripMenuItem.Checked);

            BindData();
            if(DisplayImageToolStripMenuItem.Checked)
                DisplayImage();
        }

        private void CreditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GPAToolStripMenuItem.Checked = false;
            CreditToolStripMenuItem.Checked = true;

            outcome.Text = sm.Calculate(CreditToolStripMenuItem.Checked);

            BindData();
            if (DisplayImageToolStripMenuItem.Checked)
                DisplayImage();
        }

        private void ExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ( sm.Empty() )
            {
                MessageBox.Show("亲，还没数据哦！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string pwd = null;
            bool need_encrypt;
            GetEncryptPwd gp = new GetEncryptPwd();
            DialogResult dr = gp.ShowDialog();
            if (dr == DialogResult.OK)
            {
                pwd = gp.pwd;
                need_encrypt = true;
            }
            else
                need_encrypt = false;
            gp.Close();

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            dr = sfd.ShowDialog();
            if (dr == DialogResult.OK && sfd.CheckPathExists)
            {
                string path = sfd.FileName;

                sm.SaveToFile(sfd.FileName, need_encrypt, pwd);
                //MessageBox.Show("导出完毕!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                toolStripStatusLabel1.Text = "导出完毕!";
            }
        }

        private void ImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK && ofd.CheckFileExists)
            {
                try
                {
                    string[] data = File.ReadAllLines(ofd.FileName);
                    if (data[0] == "encrypted")
                    {
                        GetDecryptPwd gp = new GetDecryptPwd();
                        dr = gp.ShowDialog();
                        if (dr != DialogResult.OK)
                            return;
                        string newdata = Crypt.Decrypt(data[1], gp.pwd);
                        if (newdata == "")
                        {
                            MessageBox.Show("亲，貌似密码不对哦！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        data = newdata.Split('\n');
                        gp.Close();
                    }
                    sm.Load(data);
                    //MessageBox.Show("导入完毕!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    toolStripStatusLabel1.Text = "导入完毕!";
                }
                catch (Exception excp)
                {
                    Error.RecordLog(excp, "亲，貌似数据不对哦，你确定是从我这里导出的吗？");
                    return;
                }

                outcome.Text = sm.Calculate(CreditToolStripMenuItem.Checked);

                BindData();

                this.CreditToolStripMenuItem.Enabled = true;
                this.GPAToolStripMenuItem.Enabled = true;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                string key = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                if (sm.ignore.Contains(key))
                {
                    if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.Equals(true))
                    {
                        dataGridView1.Rows[e.RowIndex].Cells[0].Value = false;
                        sm.ignore[key] = true;
                    }
                    else
                    {
                        dataGridView1.Rows[e.RowIndex].Cells[0].Value = true;
                        sm.ignore[key] = false;
                    }

                    outcome.Text = sm.Calculate(CreditToolStripMenuItem.Checked);

                    if (DisplayImageToolStripMenuItem.Checked)
                        DisplayImage();
                }
            }
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox ab = new AboutBox();
            ab.ShowDialog();
        }

        //private void InnovationToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    ClassSelect cs = new ClassSelect();
        //    cs.Show();
        //}

        private Bitmap DrawOutcome(int width, int height)
        {
            Bitmap img = new Bitmap(width, height);
            int border = 25;
            Graphics gfc = Graphics.FromImage(img);           //产生Graphics对象，进行画图
            gfc.Clear(Color.White);
            Point p11 = new Point(border, border);
            Point p12 = new Point(border, height - border);
            gfc.DrawLine(new Pen(Color.Black), p11, p12);
            Point p22 = new Point(width - border, height - border);
            //gfc.DrawLine(new Pen(Color.Black), p12,p22);

            Font font = new Font("微软雅黑", 10, FontStyle.Bold);
            LinearGradientBrush brush =
                new LinearGradientBrush(new Rectangle(0, border, border, height - 2 * border), Color.Blue, Color.Blue, 1.5f, true);
            int step = (height - 2 * border) / 10;
            int scoreStep = 100 / 10;
            float rate = (float)step / scoreStep;
            for (int i = 0; i <= 10; i++)
            {
                int cur_y = step * i + border;
                gfc.DrawString((100 - i * scoreStep).ToString(), font, brush, 0, cur_y);
                gfc.DrawLine(new Pen(Color.Black), new Point(border, cur_y), new Point(width - border, cur_y));
            }

            int term_num = sm.term_avg_score.Count;
            step = (width - 2 * border) / term_num;
            //draw term
            ArrayList list = new ArrayList(sm.term_avg_score.Keys);
            list.Sort();
            Point cur1;
            Point cur2 = new Point(-1, -1);
            for (int i = 0; i < list.Count; i++)
            {
                string key = list[i].ToString();
                gfc.DrawString(key, font, brush, border + i * step, height - border);
                cur1 = new Point(border + i * step, height - border - (int)(Convert.ToSingle(sm.term_avg_score[key].ToString()) * rate));
                gfc.DrawString(sm.term_avg_score[key].ToString(), font, brush, cur1.X, cur1.Y);
                gfc.DrawRectangle(new Pen(Color.Black, 2), cur1.X - 4, cur1.Y - 4, 8, 8);
                if (cur2.X != -1)
                    gfc.DrawLine(new Pen(Color.Red, 2), cur1, cur2);
                cur2 = cur1;
            }

            return img;
        }

        private void DisplayImage()
        {
            pictureBox1.Width = 800;
            pictureBox1.Height = 500;
            pictureBox1.Image = DrawOutcome(800, 500);
        }

        private void DisplayImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sm.term_avg_score.Count == 0)
            {
                MessageBox.Show("亲，还没数据哦！", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!DisplayImageToolStripMenuItem.Checked)
            {
                //WindowState = FormWindowState.Maximized;
                tabControl1.SelectedIndex = 1;
                DisplayImageToolStripMenuItem.Checked = true;
            }
            else
            {
                //WindowState = FormWindowState.Normal;
                DisplayImageToolStripMenuItem.Checked = false;
            }
            DisplayImage();
        }
    }
}
