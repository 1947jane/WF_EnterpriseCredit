using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WF_EnterpriseCredit.LocalEntity;

namespace WF_EnterpriseCredit.ExcuteProcesser
{
    public class CollRunLog
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 运行状态
        /// </summary>
        public string RuningState { get; set; }
        /// <summary>
        /// 执行Log
        /// </summary>
        public string RuningLog { get; set; }

        public int CollCount { get; set; }
        public int ErrorCount { get; set; }
        public int DealCount { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool Complete { get; set; }
    }



    /// <summary>
    /// 功能描述:任务状态
    /// 作　　者:
    /// 创建日期:2016-09-09 14:31:38
    /// 任务编号:
    /// </summary>
    public class MainTaskProcesser
    {
        /// <summary>
        /// 请求数
        /// </summary>
        public int ParamCount { get; set; }
        /// <summary>
        /// 线程数
        /// </summary>
        public int ThreadCount { get; set; }
        /// <summary>
        /// 宽带拨号参数
        /// </summary>
        public string PPPOEParam { get; set; }
        /// <summary>
        /// 线程等待时间
        /// </summary>
        public int CollSleepSpace { get; set; }
        /// <summary>
        /// 执行进度
        /// </summary>
        public string Progress { get; set; }
        /// <summary>
        /// 收集类型
        /// </summary>
        public int Mode { get; set; }

        /// <summary>
        /// 外部终止
        /// </summary>
        public bool GetNeedStop { get; set; }

        /// <summary>
        /// 完成
        /// </summary>
        public bool Complete { get; set; }

        /// <summary>
        /// 错误次数
        /// </summary>
        public int Error { get; set; }

        /// <summary>
        /// Sqlit保存路径
        /// </summary>
        public string ExcelPath { get; set; }

        /// <summary>
        /// 空闲线程
        /// </summary>
        public int SubThreadFreeCount { get; set; }

        public int CollSleepMax { get; set; }

        public int CollSleepMin { get; set; }

    }

    public class IDParamInfo
    {


        /// <summary>
        /// 错误数
        /// </summary>
        public int ErrorCount { get; set; }

        /// <summary>
        /// 参数类型，0: 原始参数  1:重复参数
        /// </summary>
        public int ParamMode { get; set; }
        /// <summary>
        /// 参数完成
        /// </summary>
        public bool ParamFinish { get; set; }
    }
}
