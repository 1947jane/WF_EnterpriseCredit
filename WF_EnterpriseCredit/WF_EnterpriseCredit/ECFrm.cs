using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WF_EnterpriseCredit.QueryInterface;
using System.IO;
using System.Data.Common;
using System.Threading;
using WF_EnterpriseCredit.ExcuteProcesser;
using System.Diagnostics;
using System.Collections;

namespace WF_EnterpriseCredit
{
    public partial class ECFrm : Form
    {
        /// <summary>
        /// 收集POI的Keyword
        /// </summary>
        public List<string> POIKeywords;
        /// <summary>
        /// 执行文件夹
        /// </summary>
        private string ExcuteFile { get; set; }
        /// <summary>
        /// 完成文件
        /// </summary>
        private string CompleteFile { get; set; }
        /// <summary>
        /// 任务文件
        /// </summary>
        private string TaskFile { get; set; }
        /// <summary>
        /// 任务完成
        /// </summary>
        bool blnOneTaskComlete;
        /// <summary>
        /// 完成次数
        /// </summary>
        int intComplete;
        /// <summary>
        /// 任务状态类
        /// </summary>
        MainTaskProcesser taskProcesserLog;
        /// <summary>
        /// 功能描述:构造函数
        /// 作　　者:
        /// 创建日期:2016-09-08 16:01:13
        /// 任务编号:
        /// </summary>
        public ECFrm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 功能描述:开始
        /// 作　　者:
        /// 创建日期:2016-09-09 14:32:54
        /// 任务编号:
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void btnExcute_Click(object sender, EventArgs e)
        {
            try
            {
                taskProcesserLog = new MainTaskProcesser();
                taskProcesserLog.GetNeedStop = false;
                POIKeywords = new List<string>();                
                if (string.IsNullOrEmpty(txtTask.Text))
                {
                    MessageBox.Show("请输入关键字");
                    return;
                }
                if (string.IsNullOrEmpty(txtSavepath.Text))
                {
                    MessageBox.Show("请输入保存位置");
                    return;
                }
                else
                {
                    HashSet<string> hsKeywords = new HashSet<string>();
                    if (!string.IsNullOrEmpty(txtTask.Text))
                    {
                        char[] ch = { ',', '，', '\n', '\r', '\t', '/' };
                        hsKeywords.UnionWith(txtTask.Text.Split(ch, StringSplitOptions.RemoveEmptyEntries)
                            .Select(t => t.Trim())
                            .Where(t => t.Length > 0));
                    }
                    foreach (var hs in hsKeywords)
                    {
                        POIKeywords.Add(hs);
                    }
                    blnOneTaskComlete = true;
                    intComplete = 0;
                    btnExcute.Enabled = false;
                    Thread th = new Thread(AllTaskExcute);
                    th.Name = "循环检查文件夹";
                    th.IsBackground = true;
                    th.Start();
                    btnStop.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "系统错误,请截图并与开发人员联系");
                btnExcute.Enabled = true;
            }
        }
        /// <summary>
        /// 功能描述:执行所有文件任务
        /// 作　　者:
        /// 创建日期:2016-09-09 14:35:15
        /// 任务编号:
        /// </summary>
        private void AllTaskExcute()
        {
            bool il = true;
            while (true)
            {
                if (taskProcesserLog != null && taskProcesserLog.GetNeedStop)
                {
                    break;
                }
                if (il) 
                {
                    OneTaskExcute();
                }
                il = false;
            }
            this.BeginInvoke(new EventHandler(delegate
            {
                btnExcute.Enabled = true;
            }));
        }
        /// <summary>
        /// 功能描述:执行单个文件任务
        /// 作　　者:
        /// 创建日期:2016-09-09 14:36:16
        /// 任务编号:
        /// </summary>
        private void OneTaskExcute()
        {
            try
            {
                taskProcesserLog.ParamCount = POIKeywords.Count;
                taskProcesserLog.ThreadCount = 1;
                taskProcesserLog.CollSleepSpace = 500;
                Thread th = new Thread(delegate() { LogRunning(); });
                th.Name = "LogRunning";
                th.IsBackground = true;
                th.Start();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 功能描述:开始收集
        /// 作　　者:
        /// 创建日期:2016-09-09 14:39:26
        /// 任务编号:
        /// </summary>
        private void LogRunning()
        {
            try
            {
                taskProcesserLog = new MainTaskProcesser();
                taskProcesserLog.ExcelPath = txtSavepath.Text;
                taskProcesserLog.Mode = 1;
                taskProcesserLog.ThreadCount = 1;
                TaskProcesser obj =new TaskProcesserEC();
                obj.Prepare(taskProcesserLog);
                string strResult = string.Empty;

                #region 线程处理
                Thread mainThread = new Thread(delegate()
                {
                    strResult = obj.MainRunning();
                });
                mainThread.Name = "执行主线程";
                mainThread.IsBackground = true;
                mainThread.Start();

                Thread[] collThreads = new Thread[taskProcesserLog.ThreadCount];
                CollRunLog[] collLogs = new CollRunLog[taskProcesserLog.ThreadCount];
                Thread.Sleep(100);
                for (int i = 0; i < taskProcesserLog.ThreadCount; i++)
                {
                    collLogs[i] = new CollRunLog();
                    CollRunLog s = collLogs[i];
                    Thread.Sleep(100);
                    int intS = i + 1;
                    collLogs[i].Name = "收集线程" + intS;
                    collThreads[i] = new Thread(delegate()
                    {
                        obj.CollRunning(POIKeywords);
                    });
                    blnOneTaskComlete = false;
                    collThreads[i].SetApartmentState(ApartmentState.STA);
                    collThreads[i].Name = "收集线程" + intS;
                    collThreads[i].IsBackground = true;
                    collThreads[i].Start();
                }
                #endregion

                #region 线程Log
                string strExcuteLog = "======任务执行情况======";
                ShowToListState(0, strExcuteLog);
                //strExcuteLog = string.Format("文件:{0}", taskProcesserLog.SqlitPath);
                //ShowToListState(1, strExcuteLog);
                System.Diagnostics.Stopwatch sw = new Stopwatch();
                while (true) 
                {
                    sw.Start();
                    int intIndex = 1;
                    if (taskProcesserLog.Complete || taskProcesserLog.GetNeedStop)
                    {
                        strExcuteLog = "执行完成";
                        ShowToListState(intIndex++, strExcuteLog);
                        Thread.Sleep(30000);
                        break;
                    }
                    strExcuteLog = "执行中...";
                    ShowToListState(intIndex++, strExcuteLog);
                    strExcuteLog = string.Format("进度:{0}", taskProcesserLog.Progress);
                    ShowToListState(intIndex++, strExcuteLog);

                    Thread.Sleep(1000);
                }         
                blnOneTaskComlete = true;
                intComplete++;
                //intComplete++;
                mainThread.Abort();

                #endregion
            }
            catch (Exception ex)
            {
                this.BeginInvoke(new EventHandler(delegate
                {
                    MessageBox.Show(ex.ToString(), "系统错误,请截图并与开发人员联系");
                    btnExcute.Enabled = true;
                }));
            }
        }

        /// <summary>
        /// 功能描述:显示状态
        /// 作　　者:黄斯平
        /// 创建日期:2016-09-09 14:44:05
        /// 任务编号:
        /// </summary>
        /// <param name="intIndex">intIndex</param>
        /// <param name="strItem">strItem</param>
        private void ShowToListState(int intIndex, string strItem)
        {
            this.BeginInvoke(new EventHandler(delegate
            {
                if (lstState.Items.Count > intIndex)
                {
                    lstState.Items[intIndex] = strItem;
                }
                else
                {
                    lstState.Items.Add(strItem);
                }
            }));
        }

        /// <summary>
        /// 停止执行任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStop_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否终止收集?", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                taskProcesserLog.GetNeedStop = true;
                btnStop.Enabled = false;
                btnExcute.Enabled = true;
            }
        }

        private void RemoveCompleteTask()
        {
            try
            {
                if (!string.IsNullOrEmpty(TaskFile))
                {
                    //string strNewFile = TaskFile.Replace("执行", "完成");
                    string strNewFile = TaskFile.Replace("Excute", "Complete");
                    File.Move(TaskFile, strNewFile);

                }
            }
            catch
            {
                throw;
            }
        }

        private void ECFrm_Load(object sender, EventArgs e)
        {
            btnStop.Enabled = false;
        }

        private void btnsave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.DefaultExt = "xlsx";
            saveDialog.Filter = "Excel文件|*.xlsx";
            saveDialog.FileName = DateTime.Now.ToString("yyyy-MM-dd");
            DialogResult a = saveDialog.ShowDialog();
            txtSavepath.Text = saveDialog.FileName;
        }
    }

}
