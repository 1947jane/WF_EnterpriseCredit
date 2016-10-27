// 版权所有 ZhuoYue Co.,Ltd 卓越一通秘密信息
// 文件名称：IToolProcessMessage.cs
// 作　　者：huangzh
// 创建日期：2015-09-01 17:25:24
// 功能描述：工具消息接口
// 任务编号：
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadMessage_HZH
{
    /// <summary>
    /// 功能描述:工具消息接口
    /// 作　　者:huangzh
    /// 创建日期:2015-09-01 17:25:17
    /// 任务编号:
    /// 仅需要有以下代码即可：
    ///  public void SetOnMsgEventEx(ProcessMessage.DelegateMsg del){this.SetOnMsgEvent(del); }
    ///  public void SendMsgToMainClientEx(object objMsg) { this.SendMsgToMainClient(objMsg); }
    ///  public void ReadMsgFromMiEx() { this.ReadMsgFromMi(); }
    /// </summary>
    public interface IToolProcessMessage : IProcessMessage
    {

        /// <summary>
        /// 设置消息事件，需要程序初始化时完成
        /// </summary>
        /// <param name="del">事件</param>
        void SetOnMsgEventEx(ThreadMessage_HZH.ProcessMessage.DelegateMsg del);
        /// <summary>
        /// 向客户端发送消息
        /// 实现代码：this.SendMsgToMainClient(objMsg)
        /// </summary>
        /// <param name="objMsg">消息</param>
        void SendMsgToMainClientEx(object objMsg);
        /// <summary>
        /// 读取当前程序消息，需要程序初始化时完成
        /// 实现代码：this.ReadMsgFromMi（）
        /// 异常：当没有调用SetSetOnMsgEvent初始化事件，抛出异常“请初始化OnMsg事件”
        /// </summary>
        void ReadMsgFromMiEx();
        /// <summary>
        /// 设置主程序
        /// </summary>
        /// <param name="strValue">值</param>
        void SetMainID(string strValue);
    }
}
