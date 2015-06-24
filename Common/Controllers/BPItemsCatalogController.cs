using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SAPbobsCOM;

namespace Common.Controllers
{
    public class BPItemsCatalogController
    {
        public static string GetItemCode(string cardCode, string substitute)
        {
            Recordset rs = (Recordset)SBOApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            string sql = @"SELECT ItemCode FROM OSCN WHERE Substitute = '{0}' AND CardCode = '{1}'";
            sql = String.Format(sql, substitute, cardCode);
            rs.DoQuery(sql);

            string itemCode = String.Empty;
            if (rs.RecordCount > 0)
            {
                itemCode = rs.Fields.Item("ItemCode").Value.ToString();
            }

            Marshal.ReleaseComObject(rs);
            rs = null;

            return itemCode;
        }

        public static string GetSubstitute(string cardCode, string itemCode)
        {
            Recordset rs = (Recordset)SBOApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            string sql = @"SELECT Substitute FROM OSCN WHERE ItemCode = '{0}' AND CardCode = '{1}'";
            sql = String.Format(sql, itemCode, cardCode);
            rs.DoQuery(sql);

            string substitute = String.Empty;
            if (rs.RecordCount > 0)
            {
                substitute = rs.Fields.Item("Substitute").Value.ToString();
            }

            Marshal.ReleaseComObject(rs);
            rs = null;

            return substitute;
        }
    }
}
