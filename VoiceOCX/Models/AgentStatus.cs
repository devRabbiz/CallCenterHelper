using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceOCX.Models
{

    public enum AgentStatus
    {
        未就绪 = -1,
        就绪 = 0,
        处理电话 = 1,
        案头工作 = 2,
        后续跟进 = 3,
        开会 = 4,
        培训 = 5,
        休息 = 6,
        午餐 = 7,
        其他 = 8
    }
}
