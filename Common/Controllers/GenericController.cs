using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SAPbobsCOM;
using Common.Models;

namespace Common.Controllers
{
    public class GenericController
    {
        /// <summary>
        /// Preenche model generico
        /// </summary>
        /// <param name="sql">Comando SELECT</param>
        /// <param name="forceNoLock">Força comando NOLOCK</param>
        /// <returns>Model generico preenchido</returns>
        public GenericModel FillGenericModel(string sql, bool forceNoLock)
        {
            GenericModel genericModel = null;
            // SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED tem o mesmo efeito do WITH (NOLOCK)
            if (forceNoLock)
            {
                sql = " SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED " + sql;
            }
            // Lê os dados em um Recordset
            Recordset rs = (Recordset)SBOApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            sql = SBOApp.TranslateToHana(sql);
            rs.DoQuery(sql);

            // Lê os dados e insere no model
            if (rs.RecordCount > 0)
            {
                genericModel = new GenericModel();
                genericModel.Fields = new Dictionary<string, object>();
                for (int i = 0; i < rs.Fields.Count; i++)
                {
                    genericModel.Fields.Add(rs.Fields.Item(i).Name, rs.Fields.Item(i).Value);
                }
            }
            Marshal.ReleaseComObject(rs);
            rs = null;
            GC.Collect();

            return genericModel;
        }

        /// <summary>
        /// Preenche lista de model generico
        /// </summary>
        /// <param name="sql">Comando SELECT</param>
        /// <param name="forceNoLock">Força comando NOLOCK</param>
        /// <returns>Lista de model generico preenchido</returns>
        public List<GenericModel> FillGenericModelList(string sql, bool forceNoLock)
        {
            List<GenericModel> genericModelList = new List<GenericModel>();
            GenericModel genericModel;
            // SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED tem o mesmo efeito do WITH (NOLOCK)
            if (forceNoLock)
            {
                sql = " SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED " + sql;
            }
            // Lê os dados em um Recordset
            Recordset rs = (Recordset)SBOApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            sql = SBOApp.TranslateToHana(sql);
            rs.DoQuery(sql);

            // Lê os dados e insere no model
            if (rs.RecordCount > 0)
            {
                while (!rs.EoF)
                {
                    genericModel = new GenericModel();
                    genericModel.Fields = new Dictionary<string, object>();
                    for (int i = 0; i < rs.Fields.Count; i++)
                    {
                        genericModel.Fields.Add(rs.Fields.Item(i).Name, rs.Fields.Item(i).Value);
                    }
                    genericModelList.Add(genericModel);
                    rs.MoveNext();
                }
            }
            Marshal.ReleaseComObject(rs);
            rs = null;
            GC.Collect();

            return genericModelList;
        }
    }
}
