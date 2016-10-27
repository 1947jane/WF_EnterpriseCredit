// 版权所有 ZhuoYue Co.,Ltd 卓越一通秘密信息
// 文件名称：IClientProcessMessage.cs
// 作　　者：huangzh
// 创建日期：2015-09-01 17:25:13
// 功能描述：客户端消息接口
// 任务编号：
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadMessage_HZH
{
    /// <summary>
    /// 功能描述:客户端消息接口
    /// 作　　者:huangzh
    /// 创建日期:2015-09-01 17:25:07
    /// 任务编号:
    /// 仅需要有以下代码即可：
    ///  public void SetOnMsgEventEx(ProcessMessage.DelegateMsg del){this.SetOnMsgEvent(del); }
    ///  public void ReadMsgFromMainClientEx(){this.ReadMsgFromMainClient();}
    ///  public void SendMsgToProcessEx(string strProcessID, object objMsg){this.SendMsgToProcess(strProcessID, objMsg);}
    /// </summary>
    public interface IClientProcessMessage : IProcessMessage
    {

        /// <summary>
        /// 设置消息事件，需要程序初始化时完成
        /// 实现代码：this.SetSetOnMsgEvent(del);
        /// </summary>
        /// <param name="del">事件</param>
         void SetOnMsgEventEx(ThreadMessage_HZH.ProcessMessage.DelegateMsg del);
        /// <summary>
        /// 读取指定文件的消息，需要程序初始化时完成
        /// 实现代码：this.ReadMsgFromMainClient()
        /// 异常：当没有调用SetSetOnMsgEvent初始化事件，抛出异常“请初始化OnMsg事件”
        /// </summary>
         void ReadMsgFromMainClientEx();

        /// <summary>
        /// 向指定进程ID发送消息，发送消息时调用
         /// 实现代码：this.SendMsgToProcess(strProcessID,objMsg);
        /// </summary>
         /// <param name="strProcessID">进程ID</param>
        /// <param name="objMsg">消息内容</param>
         void SendMsgToProcessEx(string strProcessID, object objMsg);
         /// <summary>
         /// 设置主程序
         /// </summary>
         /// <param name="strValue">值</param>
         void SetMainID(string strValue);
    }
}
