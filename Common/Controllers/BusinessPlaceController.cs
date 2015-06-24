using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM;
using SAPbobsCOM;
using System.Runtime.InteropServices;

namespace Common.Controllers
{
    public class BusinessPlaceController : BaseController
    {
        public BusinessPlaceController()
            : base("OBPL")
        { }

        /// <summary>
        /// Busca Filial Padrão
        /// </summary>
        /// <returns>Id Filial Padrão</returns>
        public static int GetCurrentBPlId()
        {
            // FH: unica solucao encontrada no forum foi buscar a string da tela para pegar a filial selecionada.
            SAPbouiCOM.Forms forms = SBOApp.Application.Forms;
            int formType = 169;
            int bplId = 0;

            for (int I = 0; I < forms.Count; I++)
            {
                if (forms.Item(I).Type == formType)
                {
                    Form form = forms.Item(I);
                    string bplName = ((StaticText)form.Items.Item(6).Specific).Caption;
                    if (bplName.Contains("Filial: "))
                    {
                        bplName = bplName.Substring(bplName.IndexOf("Filial: "));
                        bplName = bplName.Replace("Filial: ", String.Empty);
                        Recordset rsBpl = (Recordset)SBOApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                        string sql = " SELECT BPlId FROM OBPL WHERE BPLName = '{0}' ";
                        sql = String.Format(sql, bplName);
                        sql = SBOApp.TranslateToHana(sql);
                        rsBpl.DoQuery(sql);
                        if (rsBpl.RecordCount > 0)
                        {
                            bplId = Convert.ToInt32(rsBpl.Fields.Item(0).Value);
                        }
                        else
                        {
                            bplId = 1;
                        }
                        Marshal.ReleaseComObject(rsBpl);
                        rsBpl = null;
                        GC.Collect();
                    }
                    break;
                }
            }

            return bplId;
        }

        public static int GetBplIdFromWarehouse(string warehouse)
        {
            int bplId = 1;

            Recordset rsBpl = (Recordset)SBOApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            string sql = " SELECT BPlId FROM OBPL WHERE DflWhs = '{0}' ";
            sql = String.Format(sql, warehouse);
            sql = SBOApp.TranslateToHana(sql);
            rsBpl.DoQuery(sql);
            if (rsBpl.RecordCount > 0)
            {
                bplId = Convert.ToInt32(rsBpl.Fields.Item(0).Value);
            }

            Marshal.ReleaseComObject(rsBpl);
            rsBpl = null;
            GC.Collect();

            return bplId;
        }

        public static string GetMatrixCnpj()
        {
            string cnpj = String.Empty;

            Recordset rsBpl = (Recordset)SBOApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            string sql = " SELECT TaxIdNum FROM OBPL WHERE MainBPL = 'Y' ";
            sql = SBOApp.TranslateToHana(sql);

            rsBpl.DoQuery(sql);
            if (rsBpl.RecordCount > 0)
            {
                cnpj = rsBpl.Fields.Item(0).Value.ToString();
            }

            Marshal.ReleaseComObject(rsBpl);
            rsBpl = null;
            GC.Collect();

            return cnpj;
        }

        public static int? GetBplIdFromCnpj(string cnpj)
        {
            int? bplId = null;

            Recordset rsBpl = (Recordset)SBOApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            string sql = " SELECT BPlId FROM OBPL WHERE TaxIdNum = '{0}' ";
            sql = String.Format(sql, cnpj);
            sql = SBOApp.TranslateToHana(sql);
            rsBpl.DoQuery(sql);
            if (rsBpl.RecordCount > 0)
            {
                bplId = Convert.ToInt32(rsBpl.Fields.Item(0).Value);
            }

            Marshal.ReleaseComObject(rsBpl);
            rsBpl = null;
            GC.Collect();

            return bplId;
        }

        public static object GetBpProperty(string fieldName)
        {
            return GetBpProperty(GetCurrentBPlId(), fieldName);
        }

        public static object GetBpProperty(int bplId, string fieldName)
        {
            Recordset rsBpl = (Recordset)SBOApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            string sql = " SELECT {0} FROM OBPL WHERE BPlId = '{0}' ";
            sql = String.Format(sql, fieldName, bplId);
            object value = null;
            sql = SBOApp.TranslateToHana(sql);
            rsBpl.DoQuery(sql);
            if (rsBpl.RecordCount > 0)
            {
                value = Convert.ToInt32(rsBpl.Fields.Item(0).Value);
            }

            Marshal.ReleaseComObject(rsBpl);
            rsBpl = null;
            GC.Collect();

            return value;
        }
    }
}
