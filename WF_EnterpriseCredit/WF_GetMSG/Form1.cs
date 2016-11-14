using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Text.RegularExpressions;
using System.Diagnostics;
using ThreadMessage_HZH;

namespace WF_GetMSG
{
    public partial class Form1 : Form, IToolProcessMessage
    {
        public void SetMainID(string strValue)
        {
            this.SetSTRCLIENTNAME(strValue);
        }
        public void SetOnMsgEventEx(ProcessMessage.DelegateMsg del)
        {
            this.SetOnMsgEvent(del);
        }
        public void SendMsgToMainClientEx(object objMsg)
        {
            this.SendMsgToMainClient(objMsg);
        }
        public void ReadMsgFromMiEx()
        {
            this.ReadMsgFromMi();
        }

        private enum WebState
        {
            NONE,
            LIST,
            DEF
        }


        string m_strListUrl = "http://www.tianyancha.com/search?key=莱克电气股份有限公司";
        string m_strDefUrl = string.Empty;
        /// <summary>
        /// <summary>
        /// 判断是否加载完成
        /// </summary>
        bool m_blnLoaded = false;
        string m_savepath = @"d:\2016-10-27-15-01-07.xlsx";
        bool m_blnDefLoaded = false;
        bool m_blnGetCount = false;
        /// <summary>
        /// 详情
        /// </summary>
        private WebBrowser m_browser2;
        /// <summary>
        /// 列表
        /// </summary>
        private WebBrowser m_browser1;
        public Form1(string[] args)
        {
            if (args.Length > 0)
            {
                m_savepath = args[0];
                m_strListUrl = args[1];
                m_blnGetCount = args[2] == "1";
            }
            SetOnMsgEventEx(new ProcessMessage.DelegateMsg(Msg_OnEvent));
            ReadMsgFromMiEx();

            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            CreateWebBrowser();
            Thread th = new Thread(Start);
            th.IsBackground = true;
            th.Start();

        }

        public void Msg_OnEvent(object msg, object title)
        {
            //switch (msg.ToString())
            //{
            //    case "test":
            //        SendMsgToMainClientEx("这是一个测试消息" + msg + "," + title);
            //        break;
            //}
        }

        private void CreateWebBrowser()
        {
            m_browser1 = new WebBrowser();
            m_browser2 = new WebBrowser();
            m_browser1.ScriptErrorsSuppressed = true;
            m_browser2.ScriptErrorsSuppressed = true;
            m_browser1.Size = new Size(1024, 768);
            m_browser2.Size = new Size(1024, 768);
            m_browser2.Location = new Point(800, 0);
            this.Controls.Add(m_browser1);
            this.Controls.Add(m_browser2);
            m_browser1.StatusTextChanged += new EventHandler(m_browser1_StatusTextChanged);
            m_browser2.StatusTextChanged += new EventHandler(m_browser2_StatusTextChanged);
        }

        void m_browser2_StatusTextChanged(object sender, EventArgs e)
        {
            try
            {
                if (m_browser2.ReadyState == WebBrowserReadyState.Complete)
                {
                    System.Windows.Forms.HtmlDocument doc = m_browser2.Document;
                    if (doc != null)
                    {
                        if (doc.Body != null)
                        {
                            string strDoc = doc.Body.InnerText;

                            if (doc.Body.InnerHtml.ToLower().Contains("row b-c-white company-content") || strDoc.Contains("为确认本次访问为正常用户行为，请您协助验证"))
                            {
                                m_blnDefLoaded = true;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        void m_browser1_StatusTextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!m_blnLoaded)
                {
                    System.Windows.Forms.HtmlDocument doc = m_browser1.Document;
                    if (doc != null)
                    {
                        if (doc.Body != null)
                        {
                            string strDoc = doc.Body.InnerText;

                            if ((doc.Body.InnerHtml.ToLower().Contains("b-c-white search_result_container") && Regex.IsMatch(doc.Body.InnerText, @"共(\d+)页")) || strDoc.Contains("为确认本次访问为正常用户行为，请您协助验证"))
                            {
                                m_blnLoaded = true;
                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }


        private void Start()
        {
            try
            {
            start:
                m_blnLoaded = false;
                m_browser1.BeginInvoke(new MethodInvoker(delegate()
                {
                    m_browser1.Navigate(m_strListUrl);
                }));

                int intSleepIndex = 0;
                while (!m_blnLoaded)
                {
                    Thread.Sleep(100);

                    intSleepIndex++;
                    if (intSleepIndex >= 100)
                    {
                        break;
                        //goto start;
                    }
                }
                Thread.Sleep(1000);
                try
                {
                    //验证码判断
                    System.Windows.Forms.HtmlDocument doc = null;
                    m_browser1.BeginInvoke(new MethodInvoker(delegate()
                    {
                        doc = m_browser1.Document;
                    }));
                    while (doc == null)
                    {
                        Thread.Sleep(10);
                    }
                    if (doc != null && doc.Body != null)
                    {
                        //为确认本次访问为正常用户行为，请您协助验证。
                        if (Regex.IsMatch(doc.Body.InnerText, "为确认本次访问为正常用户行为，请您协助验证"))
                        {
                            VerifyCode();
                            goto start;
                        }
                        if (doc.Body.InnerText.Contains("没有找到相关结果"))
                        {
                            SendMsgToMainClientEx("-1");
                            Thread.Sleep(1000);
                            return;
                        }
                    }
                    if (m_blnGetCount)
                    {
                        if (doc != null)
                        {
                        goHere:
                            string strReg = @"共(\d+)页";
                            Match match = Regex.Match(doc.Body.InnerText, strReg);
                            if (match != null)
                            {

                                string strNum = Regex.Match(match.Value, @"\d+").Value;
                                int intCount = int.Parse(strNum);
                                SendMsgToMainClientEx("count:" + intCount);
                            }
                            else
                            {
                                Thread.Sleep(1000);
                                goto goHere;
                            }
                        }
                    }

                    if (doc != null)
                    {
                        HtmlElementCollection elems = doc.GetElementsByTagName("div");
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        try
                        {
                            GetUrlList(elems, ref dic);
                        }
                        catch
                        {
                            goto start;
                        }
                        if (dic.Count != 0)
                        {
                            GetInner(dic);
                        }
                        else
                        {
                            goto start;
                        }

                    }
                    else
                    {
                        goto start;
                    }
                }
                catch
                {
                    goto start;
                }
            }
            catch
            { }
            finally
            {
                Application.Exit();
                Process.GetCurrentProcess().Kill();
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
                            Thread.Sleep(100);
                            goto ReadAgain;
                        }
                    }
                }
            }
        }


        public void GetInner(Dictionary<string, string> dic)
        {
            #region 详情页
            foreach (var item in dic)
            {
            Contect:
                try
                {
                    m_blnDefLoaded = false;
                    m_browser2.Navigate(item.Key);
                    int intSleepIndex = 0;
                    while (!m_blnDefLoaded)
                    {
                        Thread.Sleep(100);
                        intSleepIndex++;
                        if (intSleepIndex > 100)
                            goto Contect;
                    }
                    System.Windows.Forms.HtmlDocument docIn = null;
                    m_browser2.BeginInvoke(new MethodInvoker(delegate()
                    {
                        docIn = m_browser2.Document;
                    }));
                    while (docIn == null)
                    {
                        Thread.Sleep(10);
                    }
                    if (docIn != null && docIn.Body != null)
                    {
                        string str = docIn.Body.InnerText;
                        //为确认本次访问为正常用户行为，请您协助验证。
                        if (Regex.IsMatch(str, "为确认本次访问为正常用户行为，请您协助验证"))
                        {
                            VerifyCode();
                            goto Contect;
                        }

                        HtmlElementCollection elemsIn = docIn.GetElementsByTagName("div");
                        if (elemsIn.Count == 8)
                        {
                            goto Contect;
                        }
                        if (elemsIn != null)
                        {
                            string strtyTel = string.Empty;
                            string strtyUrl = string.Empty;
                            string strtyEmail = string.Empty;
                            string strName = string.Empty;
                            foreach (HtmlElement elIn in elemsIn)
                            {
                                if (elIn.GetAttribute("className").Equals("company_info_text"))
                                {
                                    string strMsg = elIn.InnerText;
                                    strMsg = Regex.Replace(strMsg, @"暂无|\s", "");
                                    int int1 = strMsg.IndexOf("电话:");
                                    int int2 = strMsg.IndexOf("地址:");
                                    strMsg = strMsg.Substring(int1, int2 - int1);
                                    string[] values = Regex.Split(strMsg, @"电话:|邮箱:|网址:");
                                    if (values.Length == 4)
                                    {
                                        strtyTel = values[1];
                                        strtyEmail = values[2];
                                        strtyUrl = values[3];
                                    }
                                    strtyTel = string.IsNullOrEmpty(strtyTel) ? "暂无" : strtyTel;
                                    strtyEmail = string.IsNullOrEmpty(strtyEmail) ? "暂无" : strtyEmail;
                                    strtyUrl = string.IsNullOrEmpty(strtyUrl) ? "暂无" : strtyUrl;


                                    //if (elIn.Children.Count == 10)
                                    //{
                                    //    strtyTel = elIn.Children[2].InnerText.Trim().Replace("电话: ", "");
                                    //    strtyEmail = elIn.Children[3].InnerText.Trim().Replace("邮箱: ", "");
                                    //    strtyUrl = elIn.Children[5].InnerText.Trim().Replace("网址: ", "");
                                    //}
                                    //else if (elIn.Children.Count == 11)
                                    //{
                                    //    strtyTel = elIn.Children[3].InnerText.Trim().Replace("电话: ", "");
                                    //    strtyEmail = elIn.Children[3].InnerText.Trim().Replace("邮箱: ", "");
                                    //    strtyUrl = elIn.Children[6].InnerText.Trim().Replace("网址: ", "");
                                    //}

                                }
                                else if (elIn.GetAttribute("className").Equals("row b-c-white company-content"))
                                {

                                    HtmlElementCollection els = elIn.Children[0].Children[0].Children;

                                    if (els.Count > 1)
                                    {
                                        strName = els[1].Children[0].Children[0].Children[0].InnerText;
                                    }

                                    break;
                                }
                                else
                                {
                                    continue;
                                }
                            }

                            T_Web_TianYanCha_POI ty = new T_Web_TianYanCha_POI();
                            ty.LegalPersonName = strName;
                            ty.Tel = strtyTel;
                            ty.Email = strtyEmail;
                            ty.Name = item.Value;

                            DataTable dt = GetDataTable(new List<T_Web_TianYanCha_POI>() { ty });
                            ExcelEX.DataTableToExcel(m_savepath, dt, true);
                            SendMsgToMainClientEx("+");
                        }
                    }
                }
                catch
                {
                    goto Contect;
                }

            }
            #endregion
        }

        public DataTable GetDataTable(List<T_Web_TianYanCha_POI> m_pois)
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
