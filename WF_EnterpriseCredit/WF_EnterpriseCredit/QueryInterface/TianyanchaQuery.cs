using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Web;
using System.CodeDom.Compiler;
using System.Reflection;
using WF_EnterpriseCredit.LocalEntity;
using HtmlAgilityPack;
using WF_EnterpriseCredit.ExcuteProcesser;
using System.Data.Common;
using System.Threading;
using System.Data;
using System.Diagnostics;

namespace WF_EnterpriseCredit.QueryInterface
{
    public class TianyanchaQuery : Query
    {
        /// <summary>
        /// 记录数量
        /// </summary>
        public int m_intcount = 0;
        /// <summary>
        /// timer控件
        /// </summary>
        System.Windows.Forms.Timer m_timer = new System.Windows.Forms.Timer();
        /// <summary>
        /// POI信息
        /// </summary>
        LinkedList<T_Web_TianYanCha_POI> m_pois;
        /// <summary>
        /// 判断是否加载完成
        /// </summary>
        bool m_blnUpLoad = true;
        /// <summary>
        /// 关键词
        /// </summary>
        public string m_strkeyword;
        /// <summary>
        /// 动态生成webbrowser对象
        /// </summary>
        private WebBrowser m_browser2;
               /// <summary>
        /// 动态生成webbrowser对象
        /// </summary>
        private WebBrowser m_browser1;
        /// <summary>
        /// 链接
        /// </summary>
        string strUrl = null;
        /// <summary>
        /// TaskProcesser
        /// </summary>
        MainTaskProcesser TaskProcessor;
        string savepath;
        /// <summary>
        /// 构造函数
        /// </summary>
        public TianyanchaQuery()
        {   
            m_timer.Tick += new EventHandler(m_timer_Tick);
        }
        public LinkedList<LocalEntity.T_Web_TianYanCha_POI> GetJson(MainTaskProcesser taskProcesser, string keyword)
        {
            m_browser1 = new WebBrowser();
            m_browser2 = new WebBrowser();
            m_browser1.StatusTextChanged += new EventHandler(browser1_StatusTextChanged);
            m_pois = new LinkedList<T_Web_TianYanCha_POI>();
            TaskProcessor=taskProcesser;
            savepath = taskProcesser.ExcelPath;
            m_browser1.ScriptErrorsSuppressed = true;
            //m_browser2.ScriptErrorsSuppressed = true;
            //m_browser2.StatusTextChanged += new EventHandler(browser2_StatusTextChanged);      
            m_strkeyword = keyword;

            LinkedList<T_Web_TianYanCha_POI> results = new LinkedList<T_Web_TianYanCha_POI>();
            string strkeyword = HttpUtility.UrlEncode(m_strkeyword).ToUpper();
            strUrl = "http://www.tianyancha.com/search/p{1}?key={0}";
            string strurl = string.Format(strUrl, strkeyword, 1);
            GetInfo(m_browser1, strurl);
            m_browser1.Dispose();
            m_browser1 = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return results;
        }

        public DataTable GetDataTable(LinkedList<T_Web_TianYanCha_POI> m_pois)
        {
            #region 将数据保存到DataTable
            DataTable dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Tel", typeof(string));
            dt.Columns.Add("Emali", typeof(string));
            dt.Columns.Add("Contact", typeof(string));
            lock (m_pois)
            {
                foreach (T_Web_TianYanCha_POI tw in m_pois)
                {
                    DataRow dr = dt.NewRow();
                    dr["Name"] = tw.Name;
                    dr["Tel"] = tw.Tel;
                    dr["Emali"] = tw.Email;
                    dr["Contact"] = tw.LegalPersonName;
                    //dr["Address"] = tw.RegLocation;
                    dt.Rows.Add(dr);
                }
                m_pois.Clear();
            }
            return dt;
            #endregion
        }
        /// <summary>
        /// 功能描述:得到数据
        /// 作　　者:huangsp
        /// 创建日期:2016-08-23 17:11:55
        /// 任务编号:信息收集子系统开发
        /// </summary>
        /// <param name="m_browser1">m_browser1</param>
        /// <param name="strurl">strurl</param>
        /// <returns>返回值</returns>
        private bool GetInfo(WebBrowser m_browser1, string strurl)
        {

            try
            {
            NavateAgain:
                m_intcount = 0;
                m_browser1.Navigate(strurl);
                while (m_browser1.ReadyState != WebBrowserReadyState.Complete)
                {
                    Delay(3);
                }
                if (m_browser1.Document != null && m_browser1.Document.Body != null)
                {
                    //为确认本次访问为正常用户行为，请您协助验证。
                    if (Regex.IsMatch(m_browser1.Document.Body.InnerText,"为确认本次访问为正常用户行为，请您协助验证"))
                    {
                        VerifyCode();
                        goto NavateAgain;
                    }
                    if (m_browser1.Document.Body.InnerText.Contains("没有找到相关结果"))
                    {
                        return false;
                    }
                }
                int m_intPageSum = 0;
                LinkedList<T_Web_TianYanCha_POI> Listty = new LinkedList<T_Web_TianYanCha_POI>();
                System.Windows.Forms.HtmlDocument doc = m_browser1.Document;
                if (doc != null)
                {
                    HtmlElementCollection elems = doc.GetElementsByTagName("div");
                    if (elems.Count == 8)
                    {
                        Delay(5);
                    }
                    foreach (HtmlElement el in elems)
                    {
                        if (el.GetAttribute("className").Equals("total ng-binding"))
                        {
                            string str = el.InnerText;
                            m_intPageSum = int.Parse(Regex.Match(str, "\\d+").Value);
                            break;
                        }
                    }
                    for (int i = 1; i < m_intPageSum + 1; i++)
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();

                        GetUrlList(elems, ref dic);
                        #region 详情页
                        foreach (var item in dic)
                        {
                        Contect:
                            m_browser2.Navigate(item.Key);
                            Delay(2);
                            System.Windows.Forms.HtmlDocument docIn = m_browser2.Document;
                            string str = docIn.Body.InnerText;
                            HtmlElementCollection elemsIn = docIn.GetElementsByTagName("div");
                            if (elemsIn.Count == 8)
                            {
                                goto Contect;
                            }
                            if (elemsIn != null) 
                            {
                                string strtyTel = null;
                                string strtyUrl = null;
                                string strtyEmail = null;
                                foreach (HtmlElement elIn in elemsIn)
                                {
                                    if (elIn.GetAttribute("className").Equals("company_info_text"))
                                    {
                                        if (elIn.Children.Count == 10)
                                        {
                                            strtyTel = elIn.Children[2].InnerText.Trim().Replace("电话: ", "");
                                            strtyEmail = elIn.Children[3].InnerText.Trim().Replace("邮箱: ", "");
                                            strtyUrl = elIn.Children[5].InnerText.Trim().Replace("网址: ", "");
                                        }
                                        if (elIn.Children.Count == 11)
                                        {
                                            strtyTel = elIn.Children[3].InnerText.Trim().Replace("电话: ", "");
                                            strtyEmail = elIn.Children[3].InnerText.Trim().Replace("邮箱: ", "");
                                            strtyUrl = elIn.Children[6].InnerText.Trim().Replace("网址: ", "");
                                        }

                                    }
                                    T_Web_TianYanCha_POI ty = new T_Web_TianYanCha_POI();
                                    if (elIn.GetAttribute("className").Equals("row b-c-white company-content"))
                                    {
                                        HtmlElementCollection els = elIn.Children[1].Children[0].Children;
                                        ty.Name = item.Value;
                                        ty.LegalPersonName = elIn.Children[0].Children[0].Children[1].Children[0].Children[0].Children[0].InnerText;
                                        ty.Tel = strtyTel;
                                        ty.Email = strtyEmail;
                                        if (els[4].Children.Count != 0)
                                        {
                                            ty.RegLocation = els[4].Children[0].Children[0].Children[0].InnerText;
                                        }
                                        HtmlElementCollection elss = elIn.Children[0].Children[0].Children;
                                        if (!string.IsNullOrEmpty(ty.Email))
                                        {
                                            m_pois.AddLast(ty);
                                        }
                                        m_intcount++;
                                        TaskProcessor.Progress = string.Format("关键字:{0};收集数量：{1}", m_strkeyword, m_intcount);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                int sh = 0;
                            }
                        }                       
                        #endregion
                        string strkeyword = HttpUtility.UrlEncode(m_strkeyword).ToUpper();
                        string strurls = string.Format(strUrl, strkeyword, i + 1);
                        m_browser1.Navigate(strurls);
                        while (m_browser1.ReadyState != WebBrowserReadyState.Complete)
                        {
                            Delay(3);
                        }
                        doc = m_browser1.Document;
                        if (doc != null)
                        {
                            elems = doc.GetElementsByTagName("div");
                            if (elems != null)
                            {
                                Application.DoEvents();
                                if (elems.Count == 26 || elems.Count == 27)
                                {
                                    m_browser1.Navigate(strurls);
                                    Delay(3);
                                }
                            }
                        }
                    }
                    if (m_pois.Count > 0)
                    {
                        DataTable dt = GetDataTable(m_pois);
                        Excel.TableToExcelForXLSX(dt, savepath, m_strkeyword, m_intcount);
                        //ExcelSavePOI();
                    }
                }
                return true;
                //1、快速获取天眼查网站数据，尽可能确保全面；
                //2、获取信息要包含 名称、地址、电话、网址、注册时间、核准时间、状态（开业、注销）、组织机构代码、工商注册号、统一信用代码、登记机关。
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString() + (exc.InnerException != null ? exc.InnerException.ToString() : ""), "错误");
                return false;
            }
            finally
            {
            }
        }
        /// <summary>
        /// 功能描述:调用验证码exe
        /// 作　　者:huangsp
        /// 创建日期:2016-08-23 15:41:52
        /// 任务编号:信息收集子系统开发
        /// </summary>
        /// <param name="process">process</param>
        public void ExcuteExe(ref System.Diagnostics.Process process)
        {
            //这个path就是你要调用的exe程序的绝对路径
            string path = Application.StartupPath + @"\TYC_VirfyCode";
            process.StartInfo.FileName = "验证码识别测试.exe";
            process.StartInfo.WorkingDirectory = path;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            Delay(10);
            process.Kill();
            process.WaitForExit();
        }
        /// <summary>
        /// 下一页暂停
        /// </summary>
        bool m_NextPage = true;
        /// <summary>
        /// 功能描述:延时
        /// 作　　者:huangsp
        /// 创建日期:2016-08-18 17:26:22
        /// 任务编号:信息收集子系统开发
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void m_timer_Tick(object sender, EventArgs e)
        {
            m_NextPage = true;
            m_timer.Enabled = false;
        }

        /// <summary>
        /// 功能描述:延迟
        /// 作　　者:huangsp
        /// 创建日期:2016-08-19 15:01:01
        /// 任务编号:信息收集子系统开发
        /// </summary>
        /// <param name="intSec">intSec</param>
        public void Delay(int intSec)
        {
            try
            {
                m_NextPage = false;
                m_blnUpLoad = false;
                m_timer.Interval = intSec * 1000;
                m_timer.Enabled = true;
                while (!m_NextPage)
                {
                    Application.DoEvents();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString(), "错误");
            }
        }
        /// <summary>
        /// 功能描述:得到链接列表和名字表
        /// 作　　者:huangsp
        /// 创建日期:2016-08-19 08:56:54
        /// 任务编号:信息收集子系统开发
        /// </summary>
        /// <param name="elems">elems</param>
        /// <param name="dic">dic</param>
        public void GetUrlList(
            HtmlElementCollection elems,
            ref Dictionary<string, string> dic)
        {

            if (elems == null)
                return;
            string strUrl = null;
            string strname = null;
            if (elems.Count == 8)
            {
                Delay(3);
            }

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            foreach (HtmlElement el in elems)
            {
                if (el.GetAttribute("className").Equals("col-xs-10 search_name search_repadding2"))
                {
                    if (el != null)
                    {
                        stopwatch.Start();
                    ReadAgain:
                        if (el.Children.Count > 0 && el.Children[0].Children.Count > 0)
                        {
                            strUrl = el.Children[0].GetAttribute("href");
                            strname = el.Children[0].Children[0].InnerText;
                            dic.Add(strUrl, strname);
                            stopwatch.Stop();
                            stopwatch.Reset();
                        }
                        else
                        {
                            if (stopwatch.ElapsedMilliseconds > 6000)
                            {
                                //MessageBox.Show(m_browser1.Document.Body.InnerHtml);
                                continue;
                            }
                            Delay(3);
                            goto ReadAgain;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 功能描述:webbrowser状态改变事件
        /// 作　　者:huangsp
        /// 创建日期:2015-12-10 15:59:19
        /// 任务编号:信息收集子系统开发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void browser1_StatusTextChanged(object sender, EventArgs e)
        {
            try
            {
                //Thread.Sleep(500);
                System.Windows.Forms.HtmlDocument doc = (sender as WebBrowser).Document;
                if (doc != null)
                {
                    if (doc.Body != null)
                    {
                        string strDoc = doc.Body.InnerText;
                        if (strDoc != null)
                        {
                            if (strDoc.Contains("相关企业"))
                            {
                                m_blnUpLoad = true;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool VerifyCode()
        {

            try
            {
                if (Process.GetProcessesByName("验证码识别测试.exe").ToList().Count > 0)
                {
                    foreach (var item in Process.GetProcessesByName("验证码识别测试.exe"))
                    {
                        item.Kill();
                        Thread.Sleep(500);
                    }
                }

                Process pp = Process.Start(string.Format(@"{0}\TYC_VirfyCode\验证码识别测试.exe", Application.StartupPath));
                int intTime = 0;
                while (true)
                {
                    if (intTime > 30)
                    {
                        break;
                    }
                    intTime++;
                    Thread.Sleep(1000);
                }
                pp.Kill();
                pp.Dispose();
            }
            catch (Exception ex)
            {
            }
            return true;
        }
    }

}
