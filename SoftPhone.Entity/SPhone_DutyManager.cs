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
    
    public partial class SPhone_DutyManager
    {
        public System.Guid DutyManagerID { get; set; }
        public string ManagerName { get; set; }
        public string PhoneNo { get; set; }
        public string CreateBy { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public Nullable<System.DateTime> DutyDate { get; set; }
    }
}