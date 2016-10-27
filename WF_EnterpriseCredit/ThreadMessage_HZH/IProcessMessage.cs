// 版权所有 ZhuoYue Co.,Ltd 卓越一通秘密信息
// 文件名称：IProcessMessage.cs
// 作　　者：huangzh
// 创建日期：2015-08-27 16:01:34
// 功能描述：进程通信辅助
// 任务编号：
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThreadMessaging;
using System.Threading;

namespace ThreadMessage_HZH
{
    /// <summary>
    /// 功能描述:ProcessMessage的空接口，不要继承该接口，请继承IToolProcessMessage或IClientProcessMessage
    /// 使用方法:当类继承该接口后，可直接调用this.ReadMsgFromMainClient()
    /// 作　　者:huangzh
    /// 创建日期:2015-08-28 12:15:36
    /// 任务编号:
    /// </summary>
    public interface IProcessMessage
    { }
    /// <summary>
    /// 功能描述:进程通信辅助
    /// 作　　者:huangzh
    /// 创建日期:2015-08-27 16:01:20
    /// 任务编号:
    /// </summary>
    public static class ProcessMessage
    {

        /// <summary>
        /// 消息委托
        /// </summary>
        /// <param name="objmsg">消息</param>
        /// <param name="objfrom">来源</param>
        public delegate void DelegateMsg(object objmsg, object objfrom);
        /// <summary>
        /// 消息事件
        /// </summary>
        public static event DelegateMsg OnMsg;

        /// <summary>
        /// 主程序常量
        /// </summary>
        private static string STRCLIENTNAME = "MainClient";

        /// <summary>
        /// 设置主程序
        /// </summary>
        /// <param name="value">值0</param>
        /// <param name="strValue">值</param>
        public static void SetSTRCLIENTNAME<T>(this T value, string strValue)
        {
            STRCLIENTNAME = strValue;
        }

        /// <summary>
        /// 功能描述:设置消息事件OnMsg
        /// 作　　者:huangzh
        /// 创建日期:2015-08-28 11:44:32
        /// 任务编号:
        /// </summary>
        /// <param name="value">非接口调用，传null</param>
        /// <param name="del">事件委托</param>
        public static void SetOnMsgEvent<T>(this T value, DelegateMsg del) where T : IProcessMessage
        {
            OnMsg += del;
        }


        /// <summary>
        /// 功能描述:发送消息到主程序
        /// 作　　者:huangzh
        /// 创建日期:2015-08-28 16:11:23
        /// 任务编号:
        /// </summary>
        /// <param name="value">非接口调用，传null</param>
        /// <param name="objMsg">objMsg</param>
        public static void SendMsgToMainClient<T>(this T value, object objMsg) where T : IProcessMessage
        {
            try
            {
                SendMsgToProcess(value, STRCLIENTNAME, objMsg);
            }
            catch
            {
                throw;
            }
        }


        /// <summary>
        /// 功能描述:读取主程序消息
        /// 作　　者:huangzh
        /// 创建日期:2015-08-28 16:11:31
        /// 任务编号:
        /// </summary>
        /// <param name="value">非接口调用，传null</param>
        public static void ReadMsgFromMainClient<T>(this T value) where T : IProcessMessage
        {
            try
            {
                ReadMsgFromProcess(value, STRCLIENTNAME);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 功能描述:读取自己的消息
        /// 作　　者:huangzh
        /// 创建日期:2015-09-06 11:13:44
        /// 任务编号:
        /// </summary>
        /// <param name="value">非接口调用，传null</param>
        public static void ReadMsgFromMi<T>(this T value) where T : IProcessMessage
        {
            try
            {
                ReadMsgFromProcess(value, System.Diagnostics.Process.GetCurrentProcess().Id.ToString());
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// 功能描述:发送消息到指定进程ID
        /// 作　　者:huangzh
        /// 创建日期:2015-08-28 16:11:41
        /// 任务编号:
        /// </summary>
        /// <param name="value">非接口调用，传null</param>
        /// <param name="strProcessID">进程ID</param>
        /// <param name="objMsg">objMsg</param>
        public static void SendMsgToProcess<T>(
            this T value,
            string strProcessID,
            object objMsg) where T : IProcessMessage
        {
            try
            {

                ProcessMailBox mail = new ProcessMailBox(strProcessID, 1024, false);
                mail.Content = objMsg;
                //给读取消息留时间
                Thread.Sleep(10);
                //清空消息
                mail.ClearContent();
                mail.Dispose();
            }
            catch
            {
                throw;
            }
        }



        /// <summary>
        /// 功能描述:从指定进程读取消息
        /// 作　　者:huangzh
        /// 创建日期:2015-08-28 16:11:59
        /// 任务编号:
        /// </summary>
        /// <param name="value">非接口调用，传null</param>
        /// <param name="strMsgFrom">strToolProcessName</param>
        public static void ReadMsgFromProcess<T>(this T value, string strMsgFrom) where T : IProcessMessage
        {
            try
            {
                if (OnMsg == null)
                {
                    throw new Exception("请初始化OnMsg事件");
                }
                Thread th = new Thread(delegate()
                {

                    ProcessMailBox mail = new ProcessMailBox(strMsgFrom, 1024, false);
                    try
                    {
                        while (true)
                        {
                            if (OnMsg != null)
                            {
                                OnMsg(mail.Content, strMsgFrom);
                                //防止一直读取同一消息
                                Thread.Sleep(20);
                            }
                        }
                    }
                    catch
                    { }
                    finally
                    {
                        mail.Dispose();
                    }
                });
                th.Name = "读取夸进程消息";
                th.IsBackground = true;
                th.Start();

            }
            catch
            {
                throw;
            }
        }
    }
}
