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
    
    public partial class SPhone_LoginLog
    {
        public System.Guid ID { get; set; }
        public string EmployeeID { get; set; }
        public Nullable<System.DateTime> LoginDate { get; set; }
        public Nullable<System.DateTime> FirstloginTime { get; set; }
        public Nullable<System.DateTime> FirstlogoutTime { get; set; }
        public Nullable<System.DateTime> Lastlogout { get; set; }
        public string CreateBy { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string UpdateBy { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
    }
}
