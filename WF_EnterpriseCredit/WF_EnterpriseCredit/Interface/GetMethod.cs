using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using System.Drawing;

namespace WF_EnterpriseCredit.Interface
{

    public class GetMethod : GetDataMethod
    {

        /// <summary>
        /// 获取Cookie的值
        /// </summary>
        /// <param name="cookieName">Cookie名称</param>
        /// <param name="cc">Cookie集合对象</param>
        /// <returns>返回Cookie名称对应值</returns>
        public string GetCookie(string cookieName, CookieContainer cc, ref  Dictionary<string, string> dic)
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
                    foreach (Cookie c1 in colCookies) lstCookies.Add(c1);
            }
            dic = new Dictionary<string, string>();
            foreach (Cookie ck in lstCookies)
            {
                dic.Add(ck.Name, ck.Value);
            }
            var model = lstCookies.Find(p => p.Name == cookieName);
            if (model != null)
            {
                return model.Value;
            }
            return string.Empty;
        }
        /// <summary>
        /// 功能描述:获取时间戳
        /// 作　　者:
        /// 创建日期:2016-04-21 11:51:56
        /// 任务编号:
        /// </summary>
        /// <param name="dtEnd">dtEnd</param>
        /// <returns>返回值</returns>
        public string GetTimeStamp(DateTime dtEnd,string strC=null)
        {
            if (dtEnd == null)
                return string.Empty;
            DateTime dteTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            //除10000调整为13位
            int a = 10000;
            if (strC != null) 
            {
                a = 10000 * Convert.ToInt32(strC);
            }
            long lngt = (dtEnd.Ticks - dteTime.Ticks) / a;
            return lngt.ToString();
        }
        /// <summary>
        /// 功能描述:post
        /// 作　　者:黄斯平
        /// 创建日期:2016-09-08 12:26:09
        /// 任务编号:
        /// </summary>
        /// <param name="strUrl">strUrl</param>
        /// <param name="cookie">cookie</param>
        /// <returns>返回值</returns>
        public string GetDataByPost(string strUrl, ref CookieContainer cookie, RequestConent RequestConent=null,string strpostdata=null)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
                request.Method = "Post";
                if (cookie.Count == 0)
                {
                    request.CookieContainer = new CookieContainer();
                    cookie = request.CookieContainer;
                }
                else
                {
                    request.CookieContainer = cookie;
                }
                if (RequestConent != null) 
                {
                    if (!string.IsNullOrEmpty(RequestConent.StrHost))
                    {
                        request.Host = RequestConent.StrHost;
                    }
                    if (!string.IsNullOrEmpty(RequestConent.StrAccept))
                    {
                        request.Accept = RequestConent.StrAccept;
                    }
                    if (RequestConent.Timeout != 0)
                    {
                        request.Timeout = RequestConent.Timeout;
                    }
                    if (!string.IsNullOrEmpty(RequestConent.StrReferer))
                    {
                        request.Referer = RequestConent.StrReferer;
                    }
                    if (!string.IsNullOrEmpty(RequestConent.ContentType))
                    {
                        request.ContentType = RequestConent.ContentType;
                    }
                    if (!string.IsNullOrEmpty(RequestConent.UserAgent))
                    {
                        request.UserAgent = RequestConent.UserAgent;
                    }
                }
              
                if (!string.IsNullOrEmpty(strpostdata)) 
                {
                    byte[] postdatabytes = Encoding.UTF8.GetBytes(strpostdata);
                    request.ContentLength = postdatabytes.Length;
                    Stream stream = request.GetRequestStream();
                    stream.Write(postdatabytes, 0, postdatabytes.Length);
                    stream.Close();
                }
          

                //request.Timeout = 30000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = null;
                myStreamReader = new System.IO.StreamReader(myResponseStream, Encoding.UTF8);
                if (response.CharacterSet.Contains("GB") || response.CharacterSet.Contains("gb"))
                {
                    myStreamReader = new System.IO.StreamReader(myResponseStream, Encoding.GetEncoding("gb2312"));
                }
                //读出html内容
                var t_gbk = myStreamReader.ReadToEnd();
                myResponseStream.Close();
                return t_gbk;
            }
            catch (Exception ex)
            {
              
                return null;
            }
        }
        public Bitmap GetStreamByGet(string strUrl, ref CookieContainer cookie)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
            request.Method = "GET";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:42.0) Gecko/20100101 Firefox/42.0";

            if (cookie.Count == 0)
            {
                request.CookieContainer = new CookieContainer();
                cookie = request.CookieContainer;
            }
            else
            {
                request.CookieContainer = cookie;
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            System.Drawing.Bitmap bm = new System.Drawing.Bitmap(myResponseStream);
            myResponseStream.Close();
            return bm;

        }
        /// <summary>
        /// 功能描述:GET方式获取数据
        /// 作　　者:huangsp
        /// 创建日期:2015-11-20 14:15:39
        /// 任务编号:信息收集子系统开发
        /// </summary>
        /// <param name="strUrl">strUrl</param>
        /// <param name="cookie">cookie</param>
        /// <returns>返回值</returns>
        public string GetDataByGet(string strUrl, ref CookieContainer cookie, RequestConent RequestConent=null)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
                request.Method = "GET";
                if (cookie.Count == 0)
                {
                    request.CookieContainer = new CookieContainer();
                    cookie = request.CookieContainer;
                }
                else
                {
                    request.CookieContainer = cookie;
                }
                if (RequestConent != null) 
                {
                    if (!string.IsNullOrEmpty(RequestConent.StrHost))
                    {
                        request.Host = RequestConent.StrHost;
                    }
                    if (!string.IsNullOrEmpty(RequestConent.StrAccept))
                    {
                        request.Accept = RequestConent.StrAccept;
                    }
                    if (RequestConent.Timeout != 0)
                    {
                        request.Timeout = RequestConent.Timeout;
                    }
                    if (!string.IsNullOrEmpty(RequestConent.StrReferer))
                    {
                        request.Referer = RequestConent.StrReferer;
                    }
                    if (!string.IsNullOrEmpty(RequestConent.ContentType))
                    {
                        request.ContentType = RequestConent.ContentType;
                    }
                }
             
                //request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:42.0) Gecko/20100101 Firefox/42.0";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = null;
                myStreamReader = new System.IO.StreamReader(myResponseStream, Encoding.UTF8);
                if (response.CharacterSet != null) 
                {
                    if (response.CharacterSet.Contains("GB") || response.CharacterSet.Contains("gb"))
                    {
                        myStreamReader = new System.IO.StreamReader(myResponseStream, Encoding.GetEncoding("gb2312"));
                    }
                    if (response.CharacterSet.Contains("ISO-8859-1"))
                    {
                        myStreamReader = new System.IO.StreamReader(myResponseStream, Encoding.GetEncoding("gb2312"));
                    }
                    if (strUrl.Contains("gsxt.gzgs.gov.cn") && response.CharacterSet.Contains("ISO-8859-1")) 
                    {
                        myStreamReader = new System.IO.StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                    }
                }
       
                //读出html内容
                var t_gbk = myStreamReader.ReadToEnd();

                string htm = t_gbk;
                myResponseStream.Close();
                return htm;
            }
            catch (Exception ex)
            {
               
                return null;
            }
        }
        /// <summary>
        /// 判断是否有乱码
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        static bool isLuan(string txt)
        {
            var bytes = Encoding.UTF8.GetBytes(txt);
            //239 191 189
            for (var i = 0; i < bytes.Length; i++)
            {
                if (i < bytes.Length - 3)
                    if (bytes[i] == 239 && bytes[i + 1] == 191 && bytes[i + 2] == 189)
                    {
                        return true;
                    }
            }

            return false;
        }

        /// <summary>
        /// 功能描述:执行js
        /// 作　　者:huangsp
        /// 创建日期:2016-09-29 10:38:37
        /// 任务编号:
        /// </summary>
        /// <param name="sExpression">sExpression</param>
        /// <param name="sCode">sCode</param>
        /// <returns>返回值</returns>
        public string ExecuteScript(string sExpression, string sCode)
        {
            MSScriptControl.ScriptControl scriptControl = new MSScriptControl.ScriptControl();
            scriptControl.UseSafeSubset = true;
            scriptControl.Language = "JScript";
            scriptControl.AddCode(sCode);
            try
            {
                string str = scriptControl.Eval(sExpression).ToString();
                return str;
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
            return null;
        }

        /// <summary>
        /// 功能描述:解密js
        /// 作　　者:huangsp
        /// 创建日期:2016-09-29 10:38:26
        /// 任务编号:
        /// </summary>
        /// <param name="stringjs">stringjs</param>
        /// <returns>返回值</returns>
        public string Eval(string stringjs)
        {
            string MyJs = "function Eval(code){code2=code.replace(/^eval/,'');return eval(code2);}";
            object[] _params = new object[1];
            _params[0] = stringjs;
            MSScriptControl.ScriptControl js = new MSScriptControl.ScriptControl();
            js.Language = "javascript";
            js.AddCode(MyJs);
            string result = js.Run("Eval", _params);//NET4.0以下为js.Run("Eval",ref _params).ToString();
            return result;
        }
    }
}
