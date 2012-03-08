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

namespace HIT_JWC_Helper
{
    public partial class MainForm : Form
    {
        struct Score
        {
            public string cid;
            public string name;
            public string teacher;
            public string term;
            public string kind;
            public float credit;
            public int hours;
            public float score;
            public string other;
        };

        Web web;
        List<Score> score;
        Hashtable is_important;//必修
        Hashtable ignore;
        Hashtable term_avg_score;//每学期的分数
        Hashtable term_avg_score_tmp;//每学期的分数，加上一些临时分数和以及学分和
        string Departments = "011-测控技术与仪器,012-光电信息工程,021-热能与动力工程,022-飞行器动力工程,023-核反应堆工程,031-计算机科学与技术 ,032-信息安全,033-生物信息技术,041-自动化,042-探测制导与控制技术,051-通信工程,052-电子信息工程,053-信息对抗技术,054-遥感科学与技术,055-电磁场与无线技术,059-通信工程(微波技术),061-电气工程及其自动化,073-材料化学,074-应用化学,075-核化工与核燃料工程,081-机械设计制造及其自动化,082-工业设计,083-飞行器制造工程,084-工业工程,091-材料成型及控制工程,101-信息管理与信息系统,111-应用物理学,112-光信息科学与技术,113-核物理,121-数学与应用数学,122-信息与计算科学,131-工程管理,141-高分子材料与工程,142-化学工程与工艺,143-能源化学工程,151-英语,152-俄语,153-日语,157-英语（理）,158-俄语（理）,159-俄语_国际经济与贸易,161-社会学,163-汉语言文学,168-社会学_英语,169-社会学体育特长班,181-工程力学,182-飞行器设计与工程,183-飞行器环境与生命保障工程,184-复合材料与工程,185-空间科学与技术,190-材料科学与工程,201-工商管理,202-市场营销,203-会计学,204-财务管理,211-电子科学与技术,212-电子信息科学与技术,213-光信息科学与技术,221-金融学,222-国际经济与贸易,231-国际经济与贸易,232-经济学,238-国际经济与贸易（太平洋项目）,239-国际经济与贸易体育特长班,242-法学,251-给排水科学与工程,261-建筑环境与设备工程,271-环境工程,272-环境科学,281-生物技术,282-生物工程,291-焊接技术与工程,292-电子封装技术,301-广播电视编导,302-广告学,321-道路桥梁与渡河工程,322-交通工程,323-交通运输,324-交通信息与控制工程,331-土木工程,332-理论与应用力学,339-土木工程_理论与应用力学,341-建筑学,342-城市规划,343-艺术设计,344-景观学,351-材料物理,360-英才学院,361-实验学院,371-软件工程,372-软件工程(联合培养班),373-物联网工程,379-软件工程P2,381-临床医学,382-基础医学,391-中医班,400-英才班,411-食品科学与工程,421-电气工程及其自动化（中外合作）,441-光电子材料与器件,451-化学类（本硕博连读）,461-林大班,501-车辆工程";
        //string Department_Path = "department.txt";

        private void InitValue()
        {
            score = new List<Score>();
            is_important = new Hashtable();
            ignore = new Hashtable();
            term_avg_score = new Hashtable();
            term_avg_score_tmp = new Hashtable();
            web = new Web();

            if (!File.Exists("helper.ini"))
            {
                //INI.WriteIniData("basic", "DEPARTMENT_PATH", "department.txt", @".\helper.ini");
                INI.WriteIniData("user", "uid", "", @".\helper.ini");
                INI.WriteIniData("user", "pwd", "", @".\helper.ini");
            }
            //else
            //{
            //    Department_Path = INI.ReadIniData("basic", "DEPARTMENT_PATH", "department.txt", @".\helper.ini");
            //}

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
                string[] department = Departments.Split(',');
                string uid = web.getCookieValue("UserID");
                string udep = uid.Substring(3, 3);
                foreach (string tmp in department)
                {
                    if (tmp.Substring(0, 3) == udep)
                    {
                        postData = String.Format(postData, HttpUtility.UrlEncode(tmp, Encoding.GetEncoding("GB2312")), "20" + uid.Substring(1, 2));
                        break;
                    }
                }

                string ret = web.postData("http://xscj.hit.edu.cn/hitjwgl/xs/kcxx/ZXJH.asp", postData, Encoding.GetEncoding("GB2312"), null, true);
                ret = ret.Replace("&nbsp;", "");
                MatchCollection m = Regex.Matches(ret,
                    "<div align=\"center\"><span class=\"style2\">([^<]+)</span></div>",
                    RegexOptions.Multiline | RegexOptions.IgnoreCase);
                int count = m.Count;

                for (int i = 0; i < count; i += 8)
                {
                    string key = m[i + 1].Groups[1].ToString().Trim();
                    if (!is_important.ContainsKey(key))
                        is_important.Add(key, m[i + 6].Groups[1].ToString().Trim());
                }
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
                foreach (Score tmp in score)
                {
                    if (is_important.ContainsKey(tmp.cid) && is_important[tmp.cid].ToString() == "√")
                    {
                        dataGridView1.Rows.Add();
                        dataGridView1.Rows[i].Cells[0].Value = !(bool)ignore[tmp.name];
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
                foreach (Score tmp in score)
                {
                    dataGridView1.Rows.Add();
                    dataGridView1.Rows[i].Cells[0].Value = !(bool)ignore[tmp.name];
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

            score.Clear();
            is_important.Clear();
            ignore.Clear();

            string t = term.Text.ToString() == "全部学期" ? "all" : term.Text.ToString();
            string lb = pro_class.Text.ToString() == "第一学位" ? "1" : "2";
            string postData = "selectXQ={0}&LB={1}&Submit=%B2%E9%D1%AF%B3%C9%BC%A8";
            postData = String.Format(postData, HttpUtility.UrlEncode(t, Encoding.GetEncoding("GB2312")), lb);
            string ret = web.postData("http://xscj.hit.edu.cn/hitjwgl/xs/cjcx/cx_1.asp", postData, Encoding.GetEncoding("GB2312"), null, true);
            ret = ret.Replace("&nbsp;", "");
            MatchCollection m = Regex.Matches(ret,
                "<td[^>]*><div[^>]+>([^>]*<a[^>]*>)*([^<]+)(</a>[^>]*)*</div></td>",
                RegexOptions.Multiline | RegexOptions.IgnoreCase);
            int count = m.Count;
            if (count > 0)
            {
                Score tmp = new Score();
                for (int i = 0; i < count; i += 10)
                {
                    tmp.cid = m[i].Groups[2].ToString().Trim();
                    tmp.name = m[i + 1].Groups[2].ToString().Trim();
                    tmp.teacher = m[i + 2].Groups[2].ToString().Trim();
                    tmp.term = m[i + 3].Groups[2].ToString().Trim();
                    tmp.kind = m[i + 4].Groups[2].ToString().Trim();
                    tmp.credit = Convert.ToSingle(m[i + 5].Groups[2].ToString().Trim());
                    string hours = m[i + 6].Groups[2].ToString().Trim();
                    if (hours.EndsWith("周"))
                    {
                        hours = hours.Remove(hours.Length - 1, 1);
                        tmp.hours = Convert.ToInt32(hours) * 5;
                    }
                    else if (hours.EndsWith("次"))
                    {
                        hours = hours.Remove(hours.Length - 1, 1);
                        tmp.hours = Convert.ToInt32(hours);
                    }
                    else if (hours == "")
                        tmp.hours = 0;
                    else
                        tmp.hours = Convert.ToInt32(hours);
                    tmp.score = Convert.ToSingle(m[i + 7].Groups[2].ToString().Trim());
                    tmp.other = m[i + 8].Groups[2].ToString().Trim();
                    if (tmp.other == "补考")
                    {
                        tmp.name += "【补考】";
                    }
                    //if (ignore.ContainsKey(tmp.name))
                    //{
                    //    tmp.name += "【" +tmp.term+ "】";
                    //}
                    while (ignore.ContainsKey(tmp.name))
                    {
                        tmp.name += "【" + tmp.term + "】";
                    }
                    score.Add(tmp);
                    if (tmp.score == 0)
                        ignore.Add(tmp.name, true);
                    else
                        ignore.Add(tmp.name, false);
                }

                GetPlan();

                Cal_Credit_Performance();

                BindData();

                //MessageBox.Show("OK!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                toolStripStatusLabel1.Text = "获取数据成功，计算完毕！";
            }
            else
                outcome.Text = "没有查到相关信息！";

        }

        private void Cal_Credit_Performance()
        {
            double sum = 0.0;
            double num = 0;

            term_avg_score.Clear();
            term_avg_score_tmp.Clear();

            if (CreditToolStripMenuItem.Checked)
            {
                foreach (Score tmp in score)
                {
                    if (is_important.ContainsKey(tmp.cid) && is_important[tmp.cid].ToString() == "√" && ignore[tmp.name].Equals(false))
                    {
                        if (!term_avg_score_tmp.ContainsKey(tmp.term))
                        {
                            term_avg_score_tmp.Add(tmp.term, 0.0);
                            term_avg_score_tmp.Add(tmp.term + "s", 0.0);
                            term_avg_score_tmp.Add(tmp.term + "n", 0.0);
                        }
                        term_avg_score_tmp[tmp.term + "s"] = Convert.ToSingle(term_avg_score_tmp[tmp.term + "s"]) + tmp.credit * tmp.score;
                        term_avg_score_tmp[tmp.term + "n"] = Convert.ToSingle(term_avg_score_tmp[tmp.term + "n"]) + tmp.credit;
                        sum += tmp.credit * tmp.score;
                        num += tmp.credit;
                    }
                }
                double result = sum / num;
                outcome.Text = "总共学分：" + num.ToString("0.00") + "\n平均学分绩：" + result.ToString("0.00");
            }
            else
            {
                double peking_sum = 0.0;
                double zj_sum = 0.0;
                double std_sum = 0.0;
                double std_4_3_sum = 0.0;
                double std_improved_sum = 0.0;

                foreach (Score tmp in score)
                {
                    if (ignore[tmp.name].Equals(false))
                    {
                        if (!term_avg_score_tmp.ContainsKey(tmp.term))
                        {
                            term_avg_score_tmp.Add(tmp.term, 0.0);
                            term_avg_score_tmp.Add(tmp.term + "s", 0.0);
                            term_avg_score_tmp.Add(tmp.term + "n", 0.0);
                        }
                        term_avg_score_tmp[tmp.term + "s"] = Convert.ToSingle(term_avg_score_tmp[tmp.term + "s"]) + tmp.credit * tmp.score;
                        //term_avg_score_tmp[tmp.term + "s"] = Convert.ToSingle(term_avg_score_tmp[tmp.term + "s"]) + tmp.credit * tmp.score;
                        term_avg_score_tmp[tmp.term + "n"] = Convert.ToSingle(term_avg_score_tmp[tmp.term + "n"]) + tmp.credit;

                        sum += tmp.credit * tmp.score;
                        peking_sum += (tmp.credit * GPA_Algorithms.Peking(tmp.score));
                        std_sum += (tmp.credit * GPA_Algorithms.Standard(tmp.score));
                        std_4_3_sum += (tmp.credit * GPA_Algorithms.Standard_4_3(tmp.score));
                        std_improved_sum += (tmp.credit * GPA_Algorithms.Standard_Improved(tmp.score));
                        zj_sum += (tmp.credit * GPA_Algorithms.Zhejiang(tmp.score));
                        num += tmp.credit;
                    }
                }
                double result = sum / num;
                outcome.Text = "总共学分：" + num.ToString("0.00") +
                    "\n平均分：" + result.ToString("0.00") +
                    "\nGPA_Standard\n(Avg / 100 * 4.0)：" + (result / 100 * 4.0).ToString("0.00") +
                    "\nGPA Classification:" +
                    "\nStandard                ：" + (std_sum / num).ToString("0.00") +
                    "\nStandard 4.3           ：" + (std_4_3_sum / num).ToString("0.00") +
                    "\nStandard Improved ：" + (std_improved_sum / num).ToString("0.00") +
                    "\nUniversity of Peking：" + (peking_sum / num).ToString("0.00") +
                    "\nUniversity of Zhejiang：" + (zj_sum / num).ToString("0.00");
            }
            foreach (DictionaryEntry tmp in term_avg_score_tmp)
            {
                string key = tmp.Key.ToString();
                if (key.EndsWith("s") || key.EndsWith("n"))
                    continue;
                float s = Convert.ToSingle(term_avg_score_tmp[key + "s"]) / Convert.ToSingle(term_avg_score_tmp[key + "n"]);
                term_avg_score.Add(key, s);
            }
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
            Cal_Credit_Performance();
            BindData();
            DisplayImage();
        }

        private void CreditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GPAToolStripMenuItem.Checked = false;
            CreditToolStripMenuItem.Checked = true;
            Cal_Credit_Performance();
            BindData();
            DisplayImage();
        }

        private void ExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (score.Count == 0 || is_important.Count == 0)
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
                string data = "";
                foreach (Score tmp in score)
                {
                    data += tmp.cid + "#";
                    data += tmp.name + "#";
                    data += tmp.teacher + "#";
                    data += tmp.term + "#";
                    data += tmp.kind + "#";
                    data += tmp.credit + "#";
                    data += tmp.hours + "#";
                    data += tmp.score + "#";
                    data += tmp.other + "\n";
                }
                data += "--------\n";
                foreach (DictionaryEntry tmp in is_important)
                {
                    data += tmp.Key + "#";
                    data += tmp.Value + "\n";
                }
                if (need_encrypt)
                {
                    data = Crypt.Encrypt(data, pwd);
                    if (data == "")
                    {
                        MessageBox.Show("亲，出错了。。。~~>_<~~", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    data = "encrypted\n" + data;
                }
                File.WriteAllText(path, data);
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
                    score.Clear();
                    is_important.Clear();
                    ignore.Clear();

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
                    bool other = false;
                    foreach (string tmp in data)
                    {
                        if (tmp == "--------" || tmp == "")
                        {
                            other = true;
                            continue;
                        }
                        string[] vals = tmp.Split('#');
                        if (other)
                        {
                            is_important.Add(vals[0], vals[1]);
                        }
                        else
                        {
                            Score s = new Score();
                            s.cid = vals[0];
                            s.name = vals[1];
                            s.teacher = vals[2];
                            s.term = vals[3];
                            s.kind = vals[4];
                            s.credit = Convert.ToSingle(vals[5]);
                            s.hours = Convert.ToInt32(vals[6]);
                            s.score = Convert.ToSingle(vals[7]);
                            s.other = vals[8];
                            score.Add(s);
                            //ignore.Add(s.name, false);
                            if (s.score == 0)
                                ignore.Add(s.name, true);
                            else
                                ignore.Add(s.name, false);
                        }
                    }
                    //MessageBox.Show("导入完毕!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    toolStripStatusLabel1.Text = "导入完毕!";
                }
                catch (Exception excp)
                {
                    Error.RecordLog(excp, "亲，貌似数据不对哦，你确定是从我这里导出的吗？");
                    return;
                }
                Cal_Credit_Performance();

                BindData();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                string key = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                if (ignore.Contains(key))
                {
                    if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.Equals(true))
                    {
                        dataGridView1.Rows[e.RowIndex].Cells[0].Value = false;
                        ignore[key] = true;
                    }
                    else
                    {
                        dataGridView1.Rows[e.RowIndex].Cells[0].Value = true;
                        ignore[key] = false;
                    }
                    Cal_Credit_Performance();
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

        private void InnovationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassSelect cs = new ClassSelect();
            cs.Show();
        }

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

            int term_num = term_avg_score.Count;
            step = (width - 2 * border) / term_num;
            //draw term
            ArrayList list = new ArrayList(term_avg_score.Keys);
            list.Sort();
            Point cur1;
            Point cur2 = new Point(-1, -1);
            for (int i = 0; i < list.Count; i++)
            {
                string key = list[i].ToString();
                gfc.DrawString(key, font, brush, border + i * step, height - border);
                cur1 = new Point(border + i * step, height - border - (int)(Convert.ToSingle(term_avg_score[key].ToString()) * rate));
                gfc.DrawString(term_avg_score[key].ToString(), font, brush, cur1.X, cur1.Y);
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
            if (term_avg_score.Count == 0)
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
