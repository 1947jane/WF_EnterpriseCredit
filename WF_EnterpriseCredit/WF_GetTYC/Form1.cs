using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ThreadMessage_HZH;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.Web;
using System.Diagnostics;
using System.Threading;

namespace WF_GetTYC
{
    public partial class Form1 : Form, IClientProcessMessage
    {
        public void SetMainID(string strValue)
        {
            this.SetSTRCLIENTNAME(strValue);
        }
        public void SetOnMsgEventEx(ProcessMessage.DelegateMsg del)
        {
            this.SetOnMsgEvent(del);
        }
        public void ReadMsgFromMainClientEx()
        {
            this.ReadMsgFromMainClient();
        }
        public void SendMsgToProcessEx(string strProcessID, object objMsg)
        {
            this.SendMsgToProcess(strProcessID, objMsg);
        }
        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            SetOnMsgEventEx(new ProcessMessage.DelegateMsg(Message_OnEvent));
            ReadMsgFromMainClientEx();
            InitializeComponent();
        }
        bool blnNext = false;
        int intMaxCount = 50;
        Process p = null;
        bool blnStop = false;
        string strUrl = "http://www.tianyancha.com/search/p{0}?key={1}";
        private void Message_OnEvent(object objmsg, object objfrom)
        {
            string strValue = objmsg.ToString();
            if (strValue == "-1")
            {
                blnNext = true;
            }
            else if (strValue == "+")
            {
                lblCount.Text = (int.Parse(lblCount.Text) + 1).ToString();
            }
            else if (strValue.StartsWith("count:"))
            {
                intMaxCount = int.Parse(strValue.Replace("count:", ""));
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            string strKey = txtKey.Text;
            if (string.IsNullOrEmpty(strKey))
            {
                MessageBox.Show("关键字不能为空");
                return;
            }
            string strSavePath = txtSavePath.Text;
            if (string.IsNullOrEmpty(strSavePath))
            {
                MessageBox.Show("保存位置不能为空");
                return;
            }
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            Thread th = new Thread(delegate()
            {
                try
                {
                    lblState.Text = "就绪";
                    DataTable dt = GetDataTable();
                    DataTableToExcel(strSavePath, dt, false);
                    string[] keys = strKey.Split(new char[] { ',', '，' });
                    lblState.Text = "抓取中";
                    int intBegin = 1;
                    foreach (var key in keys)
                    {
                        lblKey.Text = key;
                        lblCount.Text = "0";
                        for (int i = 1; i <= intMaxCount; i++)
                        {
                            if (blnStop)
                            {
                                return;
                            }
                            string _url = string.Format(strUrl, i.ToString(), HttpUtility.UrlEncode(key));
                            p = new Process();
                            p.StartInfo.FileName = "WF_GetMSG.exe";
                            p.StartInfo.WorkingDirectory = Application.StartupPath;
                            p.StartInfo.UseShellExecute = false;
                            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            p.StartInfo.Arguments = "\"" + strSavePath + "\" " + _url + " " + intBegin;
                            intBegin = 0;
                            p.StartInfo.CreateNoWindow = false;
                            p.Start();
                            if (p != null)
                            {
                                p.WaitForExit(1000 * 60 * 30);
                            }
                            if (blnStop)
                            {
                                return;
                            }
                        }
                    }
                }
                catch
                { }
                finally
                {
                    lblState.Text = "完成";
                    MessageBox.Show("抓取完成");
                    btnStop.Enabled = false;
                    btnStart.Enabled = true;
                }
            });
            th.IsBackground = true;
            th.Start();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            this.saveFileDialog1.FileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            if (this.saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtSavePath.Text = this.saveFileDialog1.FileName;
            }
        }

        /// <summary>
        /// 将DataTable数据导入到excel中
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="data">要导入的数据</param>
        /// <param name="sheetName">要导入的excel的sheet的名称</param>
        /// <param name="blnAppled">是否是追加模式</param>
        /// <param name="isColumnWritten">DataTable的列名是否要导入</param>
        /// <returns>导入数据行数(包含列名那一行)</returns>
        private int DataTableToExcel(string fileName, System.Data.DataTable data, bool blnAppled = false, string sheetName = "sheet1", bool isColumnWritten = true)
        {
            int i = 0;
            int j = 0;
            int count = 0;
            ISheet sheet = null;
            IWorkbook workbook = null;
            FileStream fs = null;
            try
            {
                if (!blnAppled)
                {
                    fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
                    if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                        workbook = new XSSFWorkbook();
                    else if (fileName.IndexOf(".xls") > 0) // 2003版本
                        workbook = new HSSFWorkbook();
                }
                else
                {
                    fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                        workbook = new XSSFWorkbook(fs);
                    else if (fileName.IndexOf(".xls") > 0) // 2003版本
                        workbook = new HSSFWorkbook(fs);
                }
                if (!blnAppled && !string.IsNullOrEmpty(sheetName))
                {
                    if (workbook != null)
                    {
                        sheet = workbook.CreateSheet(sheetName);
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    sheet = workbook.GetSheetAt(0);
                }

                if (!blnAppled)
                {
                    if (isColumnWritten == true) //写入DataTable的列名
                    {
                        IRow row = sheet.CreateRow(0);
                        for (j = 0; j < data.Columns.Count; ++j)
                        {
                            row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                        }
                        count = 1;
                    }
                    else
                    {
                        count = 0;
                    }
                }

                count = sheet.LastRowNum + 1;

                for (i = 0; i < data.Rows.Count; ++i)
                {
                    IRow row = sheet.CreateRow(count);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                    }
                    ++count;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return -1;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }


            FileStream outFs = new FileStream(fileName, FileMode.Open, FileAccess.Write);
            workbook.Write(outFs);
            outFs.Close();
            return count;
        }

        private DataTable GetDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Tel", typeof(string));
            dt.Columns.Add("Emali", typeof(string));
            dt.Columns.Add("Contact", typeof(string));
            return dt;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                lblState.Text = "正在停止";
                btnStop.Enabled = false;
                blnStop = true;
                p.Kill();
            }
            catch
            {

            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (p != null)
            {
                try
                {
                    p.Kill();
                }
                catch { }
            }
        }
    }
}
