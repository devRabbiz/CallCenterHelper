using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tele.DataLibrary;

namespace SoftPhone.Repository
{
    public class IPDNRepository : BaseTransRepository
    {
        SPhoneEntities db;
        public IPDNRepository(SPhoneEntities db)
            : base(db)
        {
            this.db = db;
        }

        public Entity.Model.DNPlace GetPlaceDN(string placeIP)
        {
            using (var db = DCHelper.SPhoneContext())
            {
                return db.SPhone_IPDN
                    .Where(x => x.PlaceIP == placeIP && x.IsValid == 1)
                    .Select(x => new Entity.Model.DNPlace() { DN = x.DNNo, Place = x.Place, Ext1 = x.Ext1, Ext2 = x.Ext2, Ext3 = x.Ext3, IsRealDN = x.IsRealDN, IsSayEmpNO = x.IsSayEmpNO }).FirstOrDefault();
            }
        }
    }
}
