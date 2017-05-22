using SoftPhone.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftPhone.Business
{
    public class IPDNBLL
    {
        public static Entity.Model.DNPlace GetPlaceDN(string placeIP)
        {
            using (var db = Tele.DataLibrary.DCHelper.SPhoneContext())
            {
                IPDNRepository IPDNR = new IPDNRepository(db);
                return IPDNR.GetPlaceDN(placeIP);
            }
        }
    }
}
