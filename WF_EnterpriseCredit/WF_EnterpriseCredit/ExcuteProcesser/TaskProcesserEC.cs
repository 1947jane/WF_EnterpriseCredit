using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WF_EnterpriseCredit.LocalEntity;
using System.Data.Common;
using System.Windows.Forms;
using System.Threading;
using WF_EnterpriseCredit.QueryInterface;
using System.IO;

namespace WF_EnterpriseCredit.ExcuteProcesser
{
    public class TaskProcesserEC : TaskProcesser
    {

        /// <summary>
        /// 收集数据
        /// </summary>
        LinkedList<T_Web_TianYanCha_POI> m_lisPOIs;

        Queue<IDParamInfo> m_lisPageParam;

        string m_strProvince = null;
        /// <summary>
        /// 收集数据量
        /// </summary>
        int m_intDataCount;
        /// <summary>
        /// 网络限制
        /// </summary>
        int m_intWebStop;
        /// <summary>
        /// 请求数
        /// </summary>
        public int m_intCollRequest;

        public MainTaskProcesser TaskProcessor { get; private set; }
        int m_intVerifyCode;

        public string MainRunning()
        {
            string strResult = string.Empty;
            //参数总数
            int intParamCount = TaskProcessor.ParamCount;
            int intCount = 0;
            //已执行参数总数
            int intParamProcessed = 0;
            //标记是否有数据需要加载
            DateTime dteLastSave = DateTime.Now;
            //TaskProcessor.Progress = string.Format("总体进度:{0}/{1}",
                    //intCount + intParamProcessed, intParamCount);
            TaskProcessor.Error = 0;
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            return null;
        }
        Random r = new Random();

        /// <summary>
        /// 功能描述:收集主方法
        /// 作　　者:黄斯平
        /// 创建日期:2016-09-09 17:41:56
        /// 任务编号:
        /// </summary>
        /// <param name="log">log</param>
        /// <param name="paramInfo">paramInfo</param>
        /// <param name="query">query</param>
        private void CollExcute(string str,Query query)
        {
            int intSleep = r.Next(TaskProcessor.CollSleepMin, TaskProcessor.CollSleepMax);
            Thread.Sleep(intSleep);
            try
            {
                ICollection<T_Web_TianYanCha_POI> result=null;
                //收集方法
                result = query.GetJson(TaskProcessor,str);

            }
            catch (Exception ex)
            {
                m_intVerifyCode++;
                try
                {
                    if (ex.Message == "远程服务器返回错误: (403) 已禁止。")
                    {
                        m_intWebStop++;
                    }
                    if (ex.Message == "The remote server returned an error: (501) Not Implemented.")
                    {
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception exs)
                {
                  
                    //WriteLog(System.DateTime.Now + System.Environment.NewLine + exs.ToString(), Application.StartupPath, "\\DealError");
                }
            }
        }
        private void SaveCollData(ICollection<T_Web_TianYanCha_POI> result, string strKeyword)
        {
            if (result != null && result.Count > 0)
            {
                lock (m_lisPOIs)
                {
                    foreach (var item in result)
                    {
                        m_lisPOIs.AddLast(item);
                        m_intDataCount++;
                    }
                }
            }
        }

        static object m_obj = new object();


        public Query GetQuery() 
        {
            Query query =  new TianyanchaQuery();
            return query;
        }

        /// <summary>
        /// 功能描述:收集子线程
        /// 作　　者:黄斯平
        /// 创建日期:2016-09-09 16:36:02
        /// 任务编号:
        /// </summary>
        /// <param name="log">log</param>
        public void CollRunning(List<string> poiKeywords)
        {

            try
            {
                Query query = null;
                Thread.Sleep(3000);
                query = GetQuery();
                foreach (string str in poiKeywords)
                {
                    CollExcute(str, query);
                }
                TaskProcessor.GetNeedStop = true;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
        public void Prepare(MainTaskProcesser taskProcesserLog)
        {
            m_lisPOIs = new LinkedList<T_Web_TianYanCha_POI>();
            m_lisPageParam = new Queue<IDParamInfo>();
            m_intDataCount = 0;
            m_intWebStop = 0;
            m_intCollRequest = 0;
            TaskProcessor = taskProcesserLog;
            m_intVerifyCode = 0;
            TaskProcessor.Complete = false;
        }
    }
}
