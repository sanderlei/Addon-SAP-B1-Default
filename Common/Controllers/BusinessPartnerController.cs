using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbobsCOM;

namespace SZS.Common.Controllers
{
    public class BusinessPartnerController
    {
        public string GetBPUF(string cardCode)
        {
            BusinessPartners bp = (BusinessPartners)SBOApp.Company.GetBusinessObject(BoObjectTypes.oBusinessPartners);
            string uf = String.Empty;

            if (bp.GetByKey(cardCode))
            {
                for (int i = 0; i < bp.Addresses.Count; i++)
                {
                    bp.Addresses.SetCurrentLine(i);
                    if (bp.Addresses.AddressName == bp.ShipToDefault)
                    {
                        uf = bp.Addresses.State;
                    }
                }
            }
            return uf;
        }
    }
}
