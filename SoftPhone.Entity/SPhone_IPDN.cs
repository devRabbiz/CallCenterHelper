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
    
    public partial class SPhone_IPDN
    {
        public System.Guid ID { get; set; }
        public string Place { get; set; }
        public string DNNo { get; set; }
        public string PlaceIP { get; set; }
        public string PhoneIP { get; set; }
        public Nullable<short> IsValid { get; set; }
        public string BindType { get; set; }
        public string PhoneType { get; set; }
        public string CreateBy { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public short IsSayEmpNO { get; set; }
        public short IsRealDN { get; set; }
        public string Ext1 { get; set; }
        public string Ext2 { get; set; }
        public string Ext3 { get; set; }
    }
}