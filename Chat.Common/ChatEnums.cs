using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chat.Common
{
    #region ChatRoomStatus

    /// <summary>
    /// 聊天室状态
    /// </summary>
    public enum ChatRoomStatus
    {
        /// <summary>
        /// 等待接入
        /// </summary>
        WaitService = 1,

        /// <summary>
        /// 聊天中
        /// </summary>
        Chatting = 2,

        /// <summary>
        /// 已关闭
        /// </summary>
        Closed = 4,

        /// <summary>
        /// 初始化中
        /// </summary>
        Initting = 8,
    }

    #endregion

    #region MessageType

    /// <summary>
    /// 消息类别
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// 系统提示
        /// </summary>
        Alert = 1,

        /// <summary>
        /// 坐席消息
        /// </summary>
        AgentMessage = 2,

        /// <summary>
        /// 访客消息
        /// </summary>
        ClientMessage = 4,

        /// <summary>
        /// 系统通知
        /// </summary>
        Notice = 8,
    }

    #endregion




}
