using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tele.DataLibrary
{
    public class DCHelper
    {
        public static SPhoneEntities SPhoneContext()
        {
            var db = new SPhoneEntities();
            return db;
        }

        public static genesys_cfgEntities GenesysCfgContext()
        {
            var db = new genesys_cfgEntities();
            return db;
        }

        public static genesys_ersEntities GenesysErsContext()
        {
            var db = new genesys_ersEntities();
            return db;
        }
    }
}
