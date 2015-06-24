using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbobsCOM;
using Common.Controllers;

namespace Common.Util
{
    public class FormattedSearchUtil
    {
        /// <summary>
        /// Seta o FormattedSearch no campo desejado
        /// </summary>
        /// <param name="QueryName">Nome da Query</param>
        /// <param name="TheQuery">Query</param>
        /// <param name="FormID">ID do form</param>
        /// <param name="ItemID">ID do item</param>
        /// <param name="ColID">ID da coluna (Default -1)</param>
        /// <returns></returns>
        public bool AssignFormattedSearch(string QueryName, string TheQuery, string FormID, string ItemID, string ColID = "-1")
        {
            bool functionReturnValue = false;
            functionReturnValue = false;

            SAPbobsCOM.Recordset oRS = (Recordset)SBOApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            SAPbobsCOM.FormattedSearches oFS = (FormattedSearches)SBOApp.Company.GetBusinessObject(BoObjectTypes.oFormattedSearches);

            try
            {
                string sql = @"SELECT * FROM CSHS T0
	                        INNER JOIN OUQR T1
		                        ON T0.QueryId = T1.IntrnalKey
	                        WHERE T0.FormID = '{0}' 
	                        AND T0.ItemId	= '{1}' 
	                        AND T0.ColID	= '{2}' ";

                sql = SBOApp.TranslateToHana(sql);
                oRS.DoQuery(String.Format(sql, FormID, ItemID, ColID));
                if (oRS.RecordCount == 0)
                {
                    int QueryID;
                    QueryID = CreateQuery(QueryName, TheQuery);
                    oFS.Action = BoFormattedSearchActionEnum.bofsaQuery;
                    oFS.FormID = FormID;
                    oFS.ItemID = ItemID;
                    oFS.ColumnID = ColID;
                    oFS.QueryID = QueryID;
                    oFS.FieldID = ItemID;
                    if (ColID == "-1")
                    {
                        oFS.ByField = BoYesNoEnum.tYES;
                    }
                    else
                    {
                        oFS.ByField = BoYesNoEnum.tNO;
                    }
                    
                    long lRetCode = oFS.Add();
                    if (lRetCode == -2035)
                    {
                        sql = SBOApp.TranslateToHana(sql);
                        oRS.DoQuery("SELECT TOP 1 T0.IndexID FROM [dbo].[CSHS] T0 WHERE T0.FormID='" + FormID + "' AND T0.ItemId='" + ItemID + "' AND T0.ColID='" + ColID + "'");
                        
                        if (oRS.RecordCount > 0)
                        {
                            oFS.GetByKey((int)oRS.Fields.Item(0).Value);
                            oFS.Action = BoFormattedSearchActionEnum.bofsaQuery;
                            oFS.FormID = FormID;
                            oFS.ItemID = ItemID;
                            oFS.ColumnID = ColID;
                            oFS.QueryID = QueryID;
                            oFS.FieldID = ItemID;
                            if (ColID == "-1")
                            {
                                oFS.ByField = BoYesNoEnum.tYES;
                            }
                            else
                            {
                                oFS.ByField = BoYesNoEnum.tNO;
                            }
                            lRetCode = oFS.Update();
                        }
                    }
                    if (lRetCode != 0)
                    {
                        throw new Exception(String.Format("Erro ao criar query {0}: {1}", QueryName, SBOApp.Company.GetLastErrorDescription()));
                    }
                }

                functionReturnValue = true;
            }
            catch
            {
                throw new Exception(String.Format("Erro ao criar query {0}: {1}", QueryName, SBOApp.Company.GetLastErrorDescription()));
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oRS);
                oRS = null;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oFS);
                oFS = null;
                GC.Collect();
            }
            return functionReturnValue;
        }

        public void RemoveFormattedSearch(string queryName, string itemId, string formId)
        {
            SAPbobsCOM.Recordset oRS = (Recordset)SBOApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

            SAPbobsCOM.UserQueries oQuery = (UserQueries)SBOApp.Company.GetBusinessObject(BoObjectTypes.oUserQueries);
            SAPbobsCOM.FormattedSearches oFS = (FormattedSearches)SBOApp.Company.GetBusinessObject(BoObjectTypes.oFormattedSearches);

            string sSql = "SELECT IndexId FROM CSHS WHERE ItemId = '{0}' AND FormId = '{1}'";
            sSql = string.Format(sSql, itemId, formId);

            sSql = SBOApp.TranslateToHana(sSql);
            oRS.DoQuery(sSql);

            if (oRS.RecordCount > 0)
            {
                oFS.GetByKey(Convert.ToInt32(oRS.Fields.Item(0).Value));
                oFS.Remove();
            }
            string sql = "SELECT IntrnalKey, QCategory FROM OUQR WHERE QName = '{0}' and QCategory = {1}";
            sql = String.Format(sql, queryName, this.GetSysCatID());
            sSql = SBOApp.TranslateToHana(sSql);
            oRS.DoQuery(sql);
            if (oRS.RecordCount > 0)
            {
                oQuery.GetByKey(Convert.ToInt32(oRS.Fields.Item(0).Value), Convert.ToInt32(oRS.Fields.Item(1).Value));
                oQuery.Remove();
            }

            System.Runtime.InteropServices.Marshal.ReleaseComObject(oRS);
            oRS = null;
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oFS);
            oFS = null;
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oQuery);
            oQuery = null;
            GC.Collect();
        }

        public bool ExistsQuery(string query)
        {
            query = query.Replace("'", "''");
            bool exists = false;
            string sql = "SELECT TOP 1 1 FROM OUQR WHERE CAST(QString AS NVARCHAR(MAX)) = '{0}'";
            sql = String.Format(sql, query);

            SAPbobsCOM.Recordset oRS = (Recordset)SBOApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            sql = SBOApp.TranslateToHana(sql);
            oRS.DoQuery(sql);
         
            if (oRS.RecordCount > 0)
            {
                exists = true;
            }

            System.Runtime.InteropServices.Marshal.ReleaseComObject(oRS);
            oRS = null;
            GC.Collect();
            return exists;
        }

        public int CreateQuery(string QueryName, string TheQuery)
        {
            int functionReturnValue = 0;
            functionReturnValue = -1;
            SAPbobsCOM.Recordset oRS = (Recordset)SBOApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            SAPbobsCOM.UserQueries oQuery = (UserQueries)SBOApp.Company.GetBusinessObject(BoObjectTypes.oUserQueries);

            try
            {
                SBOApp.TranslateToHana("SELECT TOP 1 INTRNALKEY FROM OUQR WHERE QCATEGORY=" + GetSysCatID() + " AND QNAME='" + QueryName + "'");
                oRS.DoQuery("SELECT TOP 1 INTRNALKEY FROM OUQR WHERE QCATEGORY=" + GetSysCatID() + " AND QNAME='" + QueryName + "'");
                if (oRS.RecordCount > 0)
                {
                    functionReturnValue = (int)oRS.Fields.Item(0).Value;
                }
                else
                {
                    oQuery.QueryCategory = GetSysCatID();
                    oQuery.QueryDescription = QueryName;
                    oQuery.Query = TheQuery;
                    if (oQuery.Add() != 0)
                    {
                        throw new Exception(String.Format("Erro ao criar query {0}: {1}", QueryName, SBOApp.Company.GetLastErrorDescription()));
                    }
                    string newKey = SBOApp.Company.GetNewObjectKey();
                    if (newKey.Contains('\t'))
                    {
                        newKey = newKey.Split('\t')[0];
                    }
                    functionReturnValue = Convert.ToInt32(newKey);
                }
            }
            catch
            {
                throw new Exception(String.Format("Erro ao criar query {0}: {1}", QueryName, SBOApp.Company.GetLastErrorDescription()));
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oRS);
                oRS = null;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oQuery);
                oQuery = null;
                GC.Collect();
            }
            return functionReturnValue;
        }

        public int GetSysCatID()
        {
            int functionReturnValue = 0;
            functionReturnValue = -3;
            SAPbobsCOM.Recordset oRS = (Recordset)SBOApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset);

            try
            {
                SBOApp.TranslateToHana("SELECT TOP 1 CATEGORYID FROM OQCN WHERE CATNAME = 'Geral'");
                oRS.DoQuery("SELECT TOP 1 CATEGORYID FROM OQCN WHERE CATNAME = 'Geral'");
                if (oRS.RecordCount > 0)
                    functionReturnValue = Convert.ToInt32(oRS.Fields.Item(0).Value);
            }
            catch
            {
                throw new Exception(String.Format("Erro: {0}", SBOApp.Company.GetLastErrorDescription()));
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(oRS);
                oRS = null;
                GC.Collect();
            }
            return functionReturnValue;
        }
    }
}
