//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace SoftPhone.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class SPhone_DNIS
    {
        public SPhone_DNIS()
        {
            this.SPhone_TFNDNISRel = new HashSet<SPhone_TFNDNISRel>();
        }
    
        public long DNISID { get; set; }
        public string DNISName { get; set; }
        public string CreateBy { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public string Ext1 { get; set; }
        public string Ext2 { get; set; }
    
        public virtual ICollection<SPhone_TFNDNISRel> SPhone_TFNDNISRel { get; set; }
    }
}