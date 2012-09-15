using System;
using System.Collections.Generic;
using System.Web;
using System.Net;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO.Compression;
/*
 * Author:  Xu Zhipei
 * Email:   xuzhipei@gmail.com
 * Licence: MIT
 * */
namespace HIT_JWC_Helper
{
    public class Web
    {
        CookieContainer cookie;
        /// <summary>
        /// 初始化HttpWebRequest
        /// </summary>
        /// <param name="url"></param>
        /// <param name="referer"></param>
        /// <returns></returns>
        public HttpWebRequest initRequest(string url,string referer = null,bool withCookie = false)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            if (withCookie)
            {
                if (cookie == null)
                {
                    request.CookieContainer = new CookieContainer();
                    cookie = request.CookieContainer;
                }
                else
                {
                    request.CookieContainer = cookie;
                }
            }
            request.Referer = referer;
            request.Accept = "Accept:text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Headers["Accept-Language"] = "zh-CN,zh;q=0.";
            request.Headers["Accept-Charset"] = "GBK,utf-8;q=0.7,*;q=0.3";
            request.UserAgent = "User-Agent:Mozilla/5.0 (Windows NT 5.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.835.202 Safari/535.1";
            request.KeepAlive = true;
            return request;
        }
        /// <summary>
        /// 从相应的网址下载图片，用做样本
        /// </summary>
        /// <param name="n">个数</param>
        /// <param name="url">地址</param>
        public void saveCodeImg(int n, string url,string path,string referer = null)
        {
            for (int i = 0; i < n; i++)
            {
                WebClient webClient = new WebClient();
                webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                webClient.Headers.Add("Referer", referer);
                byte[] responseData = webClient.DownloadData(url);
                File.WriteAllBytes(path + "\\" + (i + 1).ToString() + ".bmp", responseData);
            }
        }
        /// <summary>
        /// 得到单个图片以及cookies
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Bitmap getCodeImg(string url,string referer = null)
        {
            HttpWebRequest request = initRequest(url,referer,true);

            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            //request.KeepAlive = true;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            cookie.Add(response.Cookies);
            Stream s = response.GetResponseStream();

            //string c = saveCookies(cookie);

            Bitmap tmp = new Bitmap(s);
            return tmp;
        }
        /// <summary>
        /// 得到单个页面，发送cookies
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string getPage(string url, Encoding encoding, string referer = null, bool withCookie = false, HttpWebRequest req = null)
        {
            HttpWebRequest request = initRequest(url, referer, withCookie);
            request.ContentType = "text/html;charset=UTF-8";
            request.Method = "GET";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if(withCookie)
                cookie.Add(response.Cookies);
            //string c = saveCookies(cookie);
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, encoding);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        public string postData(string postDataStr, Encoding encoding,HttpWebRequest request)
        {
            //saveCookies(cookie);
            byte[] postData = encoding.GetBytes(postDataStr);
            request.ContentLength = postData.Length;
            Stream myRequestStream = request.GetRequestStream();
            myRequestStream.Write(postData, 0, postData.Length);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream myResponseStream = response.GetResponseStream();

            if (response.Headers["Content-Encoding"] != null && response.Headers["Content-Encoding"].ToLower().Contains("gzip"))
            {
                myResponseStream = new GZipStream(myResponseStream, CompressionMode.Decompress);
            }

            StreamReader myStreamReader = new StreamReader(myResponseStream, encoding);
            string retString = myStreamReader.ReadToEnd();

            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
         
        /// <summary>
        /// POST数据，带cookies
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postDataStr"></param>
        /// <returns>返回页面</returns>
        public string postData(string url, string postDataStr,Encoding encoding,string referer = null,bool withCookie = false)
        {
            HttpWebRequest request= initRequest(url, referer, withCookie);
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";

            //saveCookies(cookie);

            return postData(postDataStr, encoding, request);
        }
        /// <summary>
        /// 打印cookies内容
        /// </summary>
        /// <param name="cc"></param>
        public string saveCookies(CookieContainer cc)
        {
            List<Cookie> lstCookies = new List<Cookie>();

            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, cc, new object[] { });

            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies) lstCookies.Add(c);
            }


            StringBuilder sbc = new StringBuilder();
            List<Cookie> cooklist = lstCookies;
            foreach (Cookie cookie in cooklist)
            {
                sbc.AppendFormat("{0};{1};{2};{3};{4};{5}\r\n",
                    cookie.Domain, cookie.Name, cookie.Path, cookie.Port,
                    cookie.Secure.ToString(), cookie.Value);
            }

            return sbc.ToString();
        }

        public void getRuleCode(string path)
        {
            string page = getPage("http://www.ku114.cn/Help/IllegalCodeP_Bejing.html", Encoding.UTF8);
            MatchCollection provinces = Regex.Matches(page, "<a href=\"/Help/IllegalCodeP_([a-zA-Z]+).html\">([^<]+)</a>", 
                RegexOptions.Multiline | RegexOptions.IgnoreCase);
            foreach (Match pro in provinces)
            {
                string province = pro.Groups[1].ToString();
                page = getPage("http://www.ku114.cn/Help/IllegalCodeP_" + province + ".html", Encoding.UTF8);
                MatchCollection cityes= Regex.Matches(page, "<a href=\"/Help/IllegalCode_([a-zA-Z]+).html\">([^<]+)</a>", 
                    RegexOptions.Multiline | RegexOptions.IgnoreCase);
                foreach (Match city in cityes)
                {
                    MatchCollection codes = Regex.Matches(page,
                        "<a href=\"/Help/IllegalCodeP_" + province + "_([\\w]+).html\">([^<]+)</a>",
                        RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    string data = "";
                    foreach (Match code in codes)
                    {
                        page = getPage("http://www.ku114.cn/Help/IllegalCodeP_" + province + "_" +code.Groups[1] + ".html", Encoding.UTF8);
                        MatchCollection info = Regex.Matches(page, "<span class='f40'>([^<]+)</span>",
                            RegexOptions.Multiline | RegexOptions.IgnoreCase);
                        data += (info[0].Groups[1] + "\t" + info[1].Groups[1] + "\t" + info[2].Groups[1] + "\n");
                    }
                    File.AppendAllText(path + "IllegalCode\\" + province + "_" + city.Groups[1] + ".txt",data);
                }
            }
        }

        public string getCookieValue(string key)
        {
            List<Cookie> lstCookies = new List<Cookie>();

            Hashtable table = (Hashtable)cookie.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, cookie, new object[] { });

            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies) lstCookies.Add(c);
            }


            List<Cookie> cooklist = lstCookies;
            foreach (Cookie c in cooklist)
            {
                //sbc.AppendFormat("{0};{1};{2};{3};{4};{5}\r\n",
                //    cookie.Domain, cookie.Name, cookie.Path, cookie.Port,
                //    cookie.Secure.ToString(), cookie.Value);
                if (c.Name.Equals(key))
                    return c.Value;
            }

            return "";
        }
    }
}