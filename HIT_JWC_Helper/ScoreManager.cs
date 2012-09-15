using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;
/*
 * Author:  Xu Zhipei
 * Email:   xuzhipei@gmail.com
 * Licence: MIT
 * */
namespace HIT_JWC_Helper
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
    class ScoreManager
    {       
        public List<Score> score;    
        public Hashtable term_avg_score;//每学期的分数
        public Hashtable is_important;//是否为考试课，算学分绩
        public Hashtable ignore;
        Hashtable term_avg_score_tmp;//每学期的分数，加上一些临时分数和以及学分和


        public ScoreManager()
        {
            score = new List<Score>();
            is_important = new Hashtable();
            ignore = new Hashtable();
            term_avg_score = new Hashtable();
            term_avg_score_tmp = new Hashtable();
        }

        public void Clear()
        {
            score.Clear();
            is_important.Clear();
            ignore.Clear();
        }

        public bool Empty()
        {
            return (score.Count == 0 || is_important.Count == 0);
        }

        public bool SaveToFile(String path,bool encrypt = false,String pwd = null)
        {
            if (encrypt && pwd == null)
                return false;

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
                data += tmp.Key + "\n";
            }
            if (encrypt)
            {
                data = Crypt.Encrypt(data, pwd);
                if (data == "")
                {
                    return false;
                }
                data = "encrypted\n" + data;
            }
            File.WriteAllText(path, data);
            return true;
        }

        public bool Load( String[] data )
        {
            Clear();
            bool other = false;
            foreach (string tmp in data)
            {
                if (tmp == "--------" || tmp == "")
                {
                    other = true;
                    continue;
                }
                
                if (other)
                {
                    is_important.Add(tmp, true);
                }
                else
                {
                    string[] vals = tmp.Split('#');
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
            return true;
        }

        public bool ParseScore(String htmlData)
        {
            htmlData = htmlData.Replace("&nbsp;", "");
            MatchCollection m = Regex.Matches(htmlData,
                "<td[^>]*><div[^>]+>([^>]*<a[^>]*>)*([^<]+)(</a>[^>]*)*</div></td>",
                RegexOptions.Multiline | RegexOptions.IgnoreCase);
            int count = m.Count;
            if (count > 0)
            {
                Score tmp = new Score();
                for (int i = 0; i < count; i += 10)
                {
                    tmp.term = m[i].Groups[2].ToString().Trim();
                    tmp.cid = m[i + 1].Groups[2].ToString().Trim();
                    tmp.name = m[i + 2].Groups[2].ToString().Trim();
                    tmp.teacher = m[i + 3].Groups[2].ToString().Trim();
                    tmp.kind = m[i + 4].Groups[2].ToString().Trim();
                    tmp.credit = Convert.ToSingle(m[i + 5].Groups[2].ToString().Trim());
                    string hours = m[i + 6].Groups[2].ToString().Trim();
                    tmp.score = Convert.ToSingle(m[i + 7].Groups[2].ToString().Trim());
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
                return true;
            }
            else
                return false;
        }

        public bool ParsePlan(String htmlData)
        {
            htmlData = htmlData.Replace("&nbsp;", "");
            MatchCollection m = Regex.Matches(htmlData,
                "<div align=\"center\"><span class=\"style2\">([^<]+)</span></div>",
                RegexOptions.Multiline | RegexOptions.IgnoreCase);
            int count = m.Count;
            if (count <= 0)
                return false;
            for (int i = 0; i < count; i += 8)
            {
                string key = m[i + 1].Groups[1].ToString().Trim();
                string method = m[i + 6].Groups[1].ToString().Trim();
                if (method == "√" && !is_important.ContainsKey(key))
                    is_important.Add(key, true);
            }
            return true;
        }

        public String[] ParsePlanID(String htmlData)
        {
            MatchCollection m = Regex.Matches(htmlData,
                @"<option value=([\w-]+) selected >([\w-]+)</option>",
                RegexOptions.Multiline | RegexOptions.IgnoreCase);
            int count = m.Count;

            if (count != 2)
                return null;

            return new String[] { m[0].Groups[1].ToString(), m[1].Groups[1].ToString() };
        }

        public String Calculate(bool credit = false)
        {
            double sum = 0.0;
            double num = 0.0;

            term_avg_score.Clear();
            term_avg_score_tmp.Clear();

            String outcome;

            if ( credit)
            {
                foreach ( Score tmp in score )
                {
                    if ( is_important.ContainsKey(tmp.cid) && ignore[ tmp.name ].Equals( false ) )
                    {
                        if ( !term_avg_score_tmp.ContainsKey( tmp.term ) )
                        {
                            term_avg_score_tmp.Add(tmp.term, 0.0);
                            term_avg_score_tmp.Add(tmp.term + "s", 0.0);//学期学分乘以分数
                            term_avg_score_tmp.Add(tmp.term + "n", 0.0);//学期学分
                        }
                        term_avg_score_tmp[tmp.term + "s"] = Convert.ToSingle(term_avg_score_tmp[tmp.term + "s"]) + tmp.credit * tmp.score;
                        term_avg_score_tmp[tmp.term + "n"] = Convert.ToSingle(term_avg_score_tmp[tmp.term + "n"]) + tmp.credit;
                        sum += tmp.credit * tmp.score;
                        num += tmp.credit;
                    }
                }
                double result = sum / num;
                outcome = "总共学分：" + num.ToString("0.00") + "\n平均学分绩：" + result.ToString("0.00");
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
                    if (tmp.score < 60) 
                        continue; //去掉不及格的课程
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
                outcome = "总共学分：" + num.ToString("0.00") +
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
            return outcome;
        }
    }
}
