using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftPhone.Entity.Model
{
    public class DNPlace
    {
        public string DN { get; set; }
        public string Place { get; set; }

        public short IsSayEmpNO { get; set; }
        public short IsRealDN { get; set; }
        public string Ext1 { get; set; }
        public string Ext2 { get; set; }
        public string Ext3 { get; set; }
    }
}
