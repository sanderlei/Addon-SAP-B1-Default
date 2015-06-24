using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SAPbobsCOM;
using Common.Enums;
using Common.Models;

namespace Common.Controllers
{
    public class ItemController : SAPObjectBaseController
    {
        public ItemController()
            : base("OITM")
        { }


        /// <summary>
        /// Busca preço de acordo com lista de preços padrão do PN
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="cardCode"></param>
        /// <returns></returns>
        public static double GetPriceByBP(string itemCode, string cardCode)
        {
            double price = 0;
            var recordSet = (Recordset)SBOApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            var query = @"SELECT Price FROM ITM1
                            INNER JOIN OCRD
	                            ON OCRD.ListNum = ITM1.PriceList
                            WHERE
	                            ITM1.ItemCode = '{0}'
	                            AND OCRD.CardCode = '{1}'";

            recordSet.DoQuery(String.Format(query, itemCode, cardCode));

            if (recordSet.RecordCount > 0)
            {
                price = double.Parse(recordSet.Fields.Item("Price").Value.ToString());
            }

            Marshal.ReleaseComObject(recordSet);
            recordSet = null;

            return price;
        }

        public static ItemManagTypeEnum GetItemManagement(string itemCode)
        {
            ItemManagTypeEnum itemManagement;

            Recordset rst = (Recordset)SBOApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

            try
            {
                string sql = String.Format("SELECT ManBtchNum, ManSerNum from OITM WHERE ItemCode = '{0}'", itemCode);

                rst.DoQuery(sql);

                if (rst.RecordCount == 0)
                {
                    throw new Exception("Item não encontrado!");
                }

                if (rst.Fields.Item(0).Value.ToString() == "Y")
                {
                    itemManagement = ItemManagTypeEnum.Batch;
                }
                else if (rst.Fields.Item(1).Value.ToString() == "Y")
                {
                    itemManagement = ItemManagTypeEnum.Serial;
                }
                else
                {
                    itemManagement = ItemManagTypeEnum.None;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                Marshal.ReleaseComObject(rst);
                rst = null;
            }
            return itemManagement;
        }

        public static string GetItemFromBPCatalog(string cardCode, string itemCode)
        {
            Recordset rs = (Recordset)SBOApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            string sql = @"SELECT ItemName 
                            FROM OITM 
                            INNER JOIN OSCN
                                ON OSCN.ItemCode = OITM.ItemCode
                            WHERE OSCN.CardCode = '{0}'
                            AND OSCN.ItemCode = '{1}'";

            rs.DoQuery(String.Format(sql, cardCode, itemCode));

            string itemName = String.Empty;

            if (rs.RecordCount > 0)
            {
                itemName = rs.Fields.Item(0).Value.ToString();
            }

            Marshal.ReleaseComObject(rs);
            rs = null;

            return itemName;
        }

        public ItemModel GetItemModel(string itemCode)
        {
            ItemModel model = this.RetrieveModel<ItemModel>(String.Format("ItemCode = '{0}'", itemCode));
            return model;
        }

        public static int GetItemColumnInt(string columnName, string itemCode)
        {
            Recordset rs = (Recordset)SBOApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            rs.DoQuery(String.Format("SELECT {0} FROM OITM WHERE ItemCode = '{1}'", columnName, itemCode));

            int column = 0;

            if (rs.RecordCount > 0)
            {
                Int32.TryParse(rs.Fields.Item(0).Value.ToString(), out column);
            }

            Marshal.ReleaseComObject(rs);
            rs = null;

            return column;
        }

        public static double GetItemColumnDouble(string columnName, string itemCode)
        {
            Recordset rs = (Recordset)SBOApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            rs.DoQuery(String.Format("SELECT {0} FROM OITM WHERE ItemCode = '{1}'", columnName, itemCode));

            double column = 0;

            if (rs.RecordCount > 0)
            {
                double.TryParse(rs.Fields.Item(0).Value.ToString(), out column);
            }

            Marshal.ReleaseComObject(rs);
            rs = null;

            return column;
        }

        public static string GetItemColumnString(string columnName, string itemCode)
        {
            Recordset rs = (Recordset)SBOApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            rs.DoQuery(String.Format("SELECT {0} FROM OITM WHERE ItemCode = '{1}'", columnName, itemCode));

            string column = String.Empty;

            if (rs.RecordCount > 0)
            {
                column = rs.Fields.Item(0).Value.ToString();
            }

            Marshal.ReleaseComObject(rs);
            rs = null;

            return column;
        }

        public static string GetItemName(string itemCode)
        {
            string itemName = GetItemColumnString("ItemName", itemCode);
            return itemName;
        }

        public string GetSuppCatnum(string itemcode)
        {
            Items oItem = (Items)SBOApp.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems);
            try
            {
                if (oItem.GetByKey(itemcode))
                    return oItem.SupplierCatalogNo;
                else
                    return "";
            }
            finally
            {
                Marshal.ReleaseComObject(oItem);
                oItem = null;
            } 
        }

        public static void GetDefaultItemLocale(string itemCode, ref string whsCode, ref int locId)
        {
            string sql = @"SELECT
                               OITM.DfltWH WhsCode, OITW.DftBinAbs LocId
                            FROM OITM
								INNER JOIN OITW
									ON OITW.ItemCode = OITM.ItemCode
									AND OITW.WhsCode = OITM.DfltWH
                            WHERE OITM.ItemCode = '{0}'";

            sql = String.Format(sql, itemCode);

            Recordset rst = (Recordset)SBOApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

            rst.DoQuery(sql);
            if (rst.RecordCount > 0)
            {
                whsCode = rst.Fields.Item("WhsCode").Value.ToString();
                locId = (int)rst.Fields.Item("LocId").Value;
            }
            Marshal.ReleaseComObject(rst);
            rst = null;
        }
    }
}
