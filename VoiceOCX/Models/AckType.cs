using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceOCX.Models
{
    public enum AckType
    {
        未知 = 0,
        登录 = 1,
        注销 = 2,

        接受 = 5,
        结束 = 6,
        转个人 = 7,
        转队列 = 8,
        更新随路 = 9,
        离开 = 10,
        邀请会议 = 11,

        未就绪 = 99,
        就绪 = 100,
        处理电话 = 101,
        案头工作 = 102,
        会议 = 103,
        开会 = 104,
        培训 = 105,
        休息 = 106,
        午餐 = 107,
        其他 = 108
    }
}
