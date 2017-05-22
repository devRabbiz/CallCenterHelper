using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VoiceOCX.Models
{
    [Serializable]
    [DataContract]
    public class ChatFlow
    {
        public ChatFlow()
        {
            Interactions = new List<Interaction>();
        }

        [DataMember]
        public List<Interaction> Interactions { get; set; }
    }
}
