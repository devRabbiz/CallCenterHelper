﻿//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tele.DataLibrary
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using SoftPhone.Entity;
    
    public partial class CTICFGEntities : DbContext
    {
        public CTICFGEntities()
            : base("name=CTICFGEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<cfg_dn> cfg_dn { get; set; }
        public DbSet<cfg_flex_prop> cfg_flex_prop { get; set; }
        public DbSet<cfg_script> cfg_script { get; set; }
        public DbSet<cfg_skill> cfg_skill { get; set; }
        public DbSet<cfg_skill_level> cfg_skill_level { get; set; }
        public DbSet<cfg_person> cfg_person { get; set; }
        public DbSet<cfg_login_info> cfg_login_info { get; set; }
        public DbSet<cfg_agent_group> cfg_agent_group { get; set; }
        public DbSet<cfg_agent_login> cfg_agent_login { get; set; }
    }
}
