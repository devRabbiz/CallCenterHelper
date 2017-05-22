using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftPhoneService.Models
{
    /// <summary>
    /// 热备
    /// </summary>
    public class HASupport
    {
        /// <summary>
        /// 键
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 是否热备
        /// </summary>
        public bool HA { get; set; }

        /// <summary>
        /// 备份URI
        /// </summary>
        public string BacupURI { get; set; }
    }
}