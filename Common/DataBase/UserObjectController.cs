using System;
using System.Windows.Forms;
using SAPbobsCOM;
using Common;
using System.Runtime.InteropServices;
using System.Text;
using Common.Models;
using System.Collections;
using System.Collections.Generic;

namespace DataBase
{
    public class UserObjectController
    {
        public StringBuilder Log { get; set; }
        int CodErro;
        string MsgErro;
        private GenericModel FindColumns;

        public UserObjectController()
        {
            Log = new StringBuilder();
        }

        public void CreateUserTable(string UserTableName, string UserTableDesc, SAPbobsCOM.BoUTBTableType UserTableType)
        {
            FindColumns = new GenericModel();
            FindColumns.Fields = new Dictionary<string, object>();
            Log.AppendLine();
            Log.AppendLine(UserTableName);
            Log.AppendFormat("Criação/Atualização da tabela de usuário {0}: ", UserTableName);

            SAPbobsCOM.UserTablesMD oUserTableMD = (SAPbobsCOM.UserTablesMD)SBOApp.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables);
            // Remove a arroba do usertable Name
            UserTableName = UserTableName.Replace("@", "");

            bool bUpdate = oUserTableMD.GetByKey(UserTableName);

            oUserTableMD.TableName = UserTableName;
            oUserTableMD.TableDescription = UserTableDesc;
            oUserTableMD.TableType = UserTableType;

            if (bUpdate)
                //CodErro = oUserTableMD.Update();
                CodErro = 0;
            else
                CodErro = oUserTableMD.Add();
            this.ValidateAction();

            Marshal.ReleaseComObject(oUserTableMD);
            oUserTableMD = null;
        }

        public void RemoveUserTable(string UserTableName)
        {
            Log.AppendFormat("Remoção da tabela de usuário {0}: ", UserTableName);
            SAPbobsCOM.UserTablesMD oUserTableMD = (SAPbobsCOM.UserTablesMD)SBOApp.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables);

            // Remove a arroba do usertable Name
            UserTableName = UserTableName.Replace("@", "");

            if (oUserTableMD.GetByKey(UserTableName))
            {
                CodErro = oUserTableMD.Remove();
                this.ValidateAction();
            }
            else
            {
                CodErro = 0;
                MsgErro = "";
                Log.AppendLine("OK");
            }
            Marshal.ReleaseComObject(oUserTableMD);
            oUserTableMD = null;
        }

        public void CreateUserField(string TableName, string FieldName, string FieldDescription, SAPbobsCOM.BoFieldTypes oType, SAPbobsCOM.BoFldSubTypes oSubType, int FieldSize, bool MandatoryYN = false, string DefaultValue = "")
        {
            if (FieldDescription.Length > 30)
            {
                FieldDescription = FieldDescription.Substring(0, 30);
            }
            if (FindColumns != null)
            {
                FindColumns.Fields.Add(FieldName, FieldDescription);
            }

            Log.AppendFormat(Environment.NewLine + "Criação/Atualização do Campo {0}.{1}: ", TableName, FieldName);
            SAPbobsCOM.UserFieldsMD oUserFieldsMD = ((SAPbobsCOM.UserFieldsMD)(SBOApp.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields)));
            bool bUpdate;

            string Sql = " SELECT FieldId FROM CUFD WHERE TableID = '{0}' AND AliasID = '{1}' ";
            Sql = String.Format(Sql, TableName, FieldName);
            string FieldId = QueryForValue(Sql);

            if (FieldId != null)
            {
                bUpdate = oUserFieldsMD.GetByKey(TableName, Convert.ToInt32(FieldId));
            }
            else
                bUpdate = false;

            oUserFieldsMD.TableName = TableName;
            oUserFieldsMD.Name = FieldName;
            oUserFieldsMD.Description = FieldDescription;
            oUserFieldsMD.Type = oType;
            oUserFieldsMD.SubType = oSubType;
            oUserFieldsMD.Mandatory = GetSapBoolean(MandatoryYN);
            if (!String.IsNullOrEmpty(DefaultValue))
            {
                oUserFieldsMD.DefaultValue = DefaultValue;
            }

            if (FieldSize > 0)
                oUserFieldsMD.EditSize = FieldSize;

            if (bUpdate)
                //CodErro = oUserFieldsMD.Update();
                CodErro = 0;
            else
                CodErro = oUserFieldsMD.Add();
            this.ValidateAction();

            Marshal.ReleaseComObject(oUserFieldsMD);
            oUserFieldsMD = null;
        }

        public void RemoveUserField(string TableName, string FieldName)
        {
            Log.AppendFormat("Remoção do Campo {0}.{1}: ", TableName, FieldName);
            SAPbobsCOM.UserFieldsMD oUserFieldsMD = ((SAPbobsCOM.UserFieldsMD)(SBOApp.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields)));

            string Sql = " SELECT FieldId FROM CUFD WHERE TableID = '{0}' AND AliasID = '{1}' ";
            Sql = String.Format(Sql, TableName, FieldName);

            string FieldId = QueryForValue(Sql);

            if (FieldId != null)
            {
                if (oUserFieldsMD.GetByKey(TableName, Convert.ToInt32(FieldId)))
                {
                    CodErro = oUserFieldsMD.Remove();
                    this.ValidateAction();
                }
            }
            else
            {
                MsgErro = "";
                CodErro = 0;
                Log.AppendLine(" Tabela/Campo não encontrado ");
            }

            Marshal.ReleaseComObject(oUserFieldsMD);
            oUserFieldsMD = null;
        }

        public void AddValidValueToUserField(string TableName, string FieldName, string Value, string Description)
        {
            // se não foi passado o parâmetro de "É Valor Padrão" trata como não
            // chamando a função que realmente insere o valor como "false" a variável IsDefault
            AddValidValueToUserField(TableName, FieldName, Value, Description, false);
        }

        public void AddValidValueToUserField(string TableName, string FieldName, string Value, string Description, bool IsDefault)
        {
            SAPbobsCOM.UserFieldsMD oUserFieldsMD = ((SAPbobsCOM.UserFieldsMD)(SBOApp.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields)));
            bool bUpdate;

            Log.AppendFormat("Criação de Valor válido {0}.{1}: ", TableName, FieldName);

            string Sql = @" SELECT FieldId FROM CUFD WHERE TableID = '{0}' AND AliasID = '{1}' ";
            Sql = String.Format(Sql, TableName, FieldName.Replace("U_", ""));
            string FieldId = QueryForValue(Sql);

            bUpdate = oUserFieldsMD.GetByKey(TableName, Convert.ToInt32(FieldId));

            Sql = @" SELECT COUNT(1) FROM CUFD
                            INNER JOIN UFD1 
                                ON CUFD.TableID = UFD1.TableID 
                                AND CUFD.FieldID = UFD1.FieldID
                        WHERE CUFD.TableID = '{0}' 
                        AND CUFD.AliasID = '{1}' 
                        AND LEN(UFD1.FldValue) > 0 "; //AND UFD1.FldValue = '" + Value + "'";
            Sql = String.Format(Sql, TableName, FieldName.Replace("U_", ""));

            string ContaValoresValidos = QueryForValue(Sql);

            if (Convert.ToInt32(ContaValoresValidos) > 0)
            {
                Sql = @" SELECT UFD1.IndexID FROM CUFD
                            INNER JOIN UFD1 
                                ON CUFD.TableID = UFD1.TableID 
                                AND CUFD.FieldID = UFD1.FieldID
                         WHERE CUFD.TableID = '{0}' 
                         AND CUFD.AliasID = '{1}' 
                         AND UFD1.FldValue = '{2}'";
                Sql = String.Format(Sql, TableName, FieldName.Replace("U_", ""), Value);

                string IndexId = QueryForValue(Sql);

                if (IndexId == null)
                    oUserFieldsMD.ValidValues.Add();

                if (IndexId != null)
                    oUserFieldsMD.ValidValues.SetCurrentLine(Convert.ToInt32(IndexId));
            }

            oUserFieldsMD.ValidValues.Value = Value;
            oUserFieldsMD.ValidValues.Description = Description;

            if (IsDefault)
                oUserFieldsMD.DefaultValue = Value;

            CodErro = oUserFieldsMD.Update();

            this.ValidateAction();

            Marshal.ReleaseComObject(oUserFieldsMD);
            oUserFieldsMD = null;
        }

        public void CreateUserObject(string ObjectName, string ObjectDesc, string TableName, SAPbobsCOM.BoUDOObjType ObjectType, bool CanLog, bool CanYearTransfer)
        {
            this.CreateUserObject(ObjectName, ObjectDesc, TableName, ObjectType, CanLog, CanYearTransfer, false, false, false, true, true, 0, 0, null);
        }

        public void CreateUserObject(string ObjectName, string ObjectDesc, string TableName, SAPbobsCOM.BoUDOObjType ObjectType, bool CanLog, bool CanYearTransfer, GenericModel findColumns)
        {
            this.CreateUserObject(ObjectName, ObjectDesc, TableName, ObjectType, CanLog, CanYearTransfer, false, false, false, true, true, 0, 0, findColumns);
        }


        public void CreateUserObject(string ObjectName, string ObjectDesc, string TableName, SAPbobsCOM.BoUDOObjType ObjectType, bool CanLog, bool CanYearTransfer, bool CanCancel, bool CanClose, bool CanCreateDefaultForm, bool CanDelete, bool CanFind, int FatherMenuId, int menuPosition, GenericModel findColumns)
        {
            // se não preenchido um table name separado, usa o mesmo do objeto
            if (String.IsNullOrEmpty(TableName))
                TableName = ObjectName;

            Log.AppendFormat("Criação/Atualização do Objeto de usuário {0}", ObjectName);

            SAPbobsCOM.UserObjectsMD UserObjectsMD = (SAPbobsCOM.UserObjectsMD)SBOApp.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD);

            // Remove a arroba do usertable Name
            TableName = TableName.Replace("@", "");

            bool bUpdate = UserObjectsMD.GetByKey(ObjectName);

            UserObjectsMD.Code = ObjectName;
            UserObjectsMD.Name = ObjectDesc;
            UserObjectsMD.ObjectType = ObjectType;
            UserObjectsMD.TableName = TableName;

            //UserObjectsMD.CanArchive = GetSapBoolean(CanArchive);
            UserObjectsMD.CanCancel = GetSapBoolean(CanCancel);
            UserObjectsMD.CanClose = GetSapBoolean(CanClose);
            UserObjectsMD.CanCreateDefaultForm = GetSapBoolean(CanCreateDefaultForm);
            UserObjectsMD.CanDelete = GetSapBoolean(CanDelete);
            UserObjectsMD.CanFind = GetSapBoolean(CanFind);
            UserObjectsMD.CanLog = GetSapBoolean(CanLog);
            UserObjectsMD.CanYearTransfer = GetSapBoolean(CanYearTransfer);

            if (CanCreateDefaultForm)
            {
                UserObjectsMD.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tYES;
                UserObjectsMD.CanCancel = GetSapBoolean(CanCancel);
                UserObjectsMD.CanClose = GetSapBoolean(CanClose);
                UserObjectsMD.CanDelete = GetSapBoolean(CanDelete);
                UserObjectsMD.CanFind = GetSapBoolean(CanFind);
                UserObjectsMD.ExtensionName = "";
                UserObjectsMD.OverwriteDllfile = SAPbobsCOM.BoYesNoEnum.tYES;
                UserObjectsMD.UseUniqueFormType = SAPbobsCOM.BoYesNoEnum.tYES;
                UserObjectsMD.ManageSeries = SAPbobsCOM.BoYesNoEnum.tYES;

                UserObjectsMD.FormColumns.FormColumnAlias = "Code";
                UserObjectsMD.FormColumns.FormColumnDescription = "Código";
                UserObjectsMD.FormColumns.Add();

                UserObjectsMD.FormColumns.FormColumnAlias = "Name";
                UserObjectsMD.FormColumns.FormColumnDescription = "Descrição";
                UserObjectsMD.FormColumns.Editable = SAPbobsCOM.BoYesNoEnum.tYES;
                UserObjectsMD.FormColumns.Add();

                UserObjectsMD.FindColumns.ColumnAlias = "Code";
                UserObjectsMD.FindColumns.ColumnDescription = "Código";
                UserObjectsMD.FindColumns.Add();

                UserObjectsMD.FindColumns.ColumnAlias = "Name";
                UserObjectsMD.FindColumns.ColumnDescription = "Descrição";
                UserObjectsMD.FindColumns.Add();

                if (findColumns != null)
                {
                    FindColumns = findColumns;
                }

                if (FindColumns != null && FindColumns.Fields != null)
                {
                    foreach (KeyValuePair<string, object> pair in FindColumns.Fields)
                    {
                        UserObjectsMD.FindColumns.ColumnAlias = pair.Key;
                        UserObjectsMD.FindColumns.ColumnDescription = pair.Value.ToString();
                        UserObjectsMD.FindColumns.Add();
                    }
                }

                UserObjectsMD.FatherMenuID = FatherMenuId;
                UserObjectsMD.Position = menuPosition;
                UserObjectsMD.MenuItem = SAPbobsCOM.BoYesNoEnum.tYES;
                UserObjectsMD.MenuUID = ObjectName;
                UserObjectsMD.MenuCaption = ObjectDesc;
            }

            if (bUpdate)
            {
                //CodErro = UserObjectsMD.Update();
            }
            else
                CodErro = UserObjectsMD.Add();

            this.ValidateAction();

            Marshal.ReleaseComObject(UserObjectsMD);
            UserObjectsMD = null;
            FindColumns = new GenericModel();
            FindColumns.Fields = new Dictionary<string, object>();
        }

        public void RemoveUserObject(string ObjectName)
        {
            Log.AppendFormat("Remoção do Objeto de usuário {0}", ObjectName);

            SAPbobsCOM.UserObjectsMD UserObjectsMD = (SAPbobsCOM.UserObjectsMD)SBOApp.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD);

            bool bUpdate = UserObjectsMD.GetByKey(ObjectName);

            CodErro = 0;
            if (bUpdate)
                CodErro = UserObjectsMD.Remove();
            this.ValidateAction();

            Marshal.ReleaseComObject(UserObjectsMD);
            UserObjectsMD = null;
        }

        public void AddChildTableToUserObject(string ObjectName, string ChildTableName)
        {
            // se não preenchido um table name separado, usa o mesmo do objeto

            Log.AppendFormat("Inserção de tabela filha ({0}) ao objeto {1}: ", ChildTableName, ObjectName);

            SAPbobsCOM.UserObjectsMD UserObjectsMD = (SAPbobsCOM.UserObjectsMD)SBOApp.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD);

            // Remove a arroba do usertable Name
            ChildTableName = ChildTableName.Replace("@", "");

            bool bUpdate = UserObjectsMD.GetByKey(ObjectName);

            bool JaAdicionada = false;
            for (int i = 0; i < UserObjectsMD.ChildTables.Count; i++)
            {
                UserObjectsMD.ChildTables.SetCurrentLine(i);
                if (ChildTableName == UserObjectsMD.ChildTables.TableName)
                {
                    JaAdicionada = true;
                    break;
                }
            }

            if (!JaAdicionada)
            {
                UserObjectsMD.ChildTables.Add();
                UserObjectsMD.ChildTables.TableName = ChildTableName;
            }

            CodErro = UserObjectsMD.Update();
            this.ValidateAction();

            Marshal.ReleaseComObject(UserObjectsMD);
            UserObjectsMD = null;
        }

        public static string QueryForValue(string Sql)
        {
            SAPbobsCOM.Recordset oRecordset = (SAPbobsCOM.Recordset)(SBOApp.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset));
            string Retorno = null;
            try
            {
                Sql = SBOApp.TranslateToHana(Sql);
                oRecordset.DoQuery(Sql);

                // Executa e, caso exista ao menos um registro, devolve o mesmo.
                // retorna sempre o primeiro campo da consulta (SEMPRE)
                if (!oRecordset.EoF)
                {
                    Retorno = oRecordset.Fields.Item(0).Value.ToString();
                }

            }
            catch
            {

            }
            finally
            {
                Marshal.ReleaseComObject(oRecordset);
                oRecordset = null;

            }

            return Retorno;
        }

        public static bool FieldExists(string tableName, string fieldName)
        {
            string sql = @" SELECT TOP 1 1 FROM CUFD WHERE TableID = '{0}' AND AliasID = '{1}' ";
            sql = String.Format(sql, tableName, fieldName.Replace("U_", ""));
            Recordset rst = (Recordset)SBOApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            sql = SBOApp.TranslateToHana(sql);

            rst.DoQuery(sql);
            bool exists = false;
            if (rst.RecordCount > 0)
            {
                exists = true;
            }

            Marshal.ReleaseComObject(rst);
            rst = null;

            return exists;
        }

        public string CreateUserKey(string KeyName, string TableName, string Fields, bool isUnique)
        {
            Log.AppendFormat("Criação de chave {0} na tabela {1}: ", KeyName, TableName);

            SAPbobsCOM.UserKeysMD oUserKeysMD = (SAPbobsCOM.UserKeysMD)SBOApp.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserKeys);

            oUserKeysMD.TableName = TableName;
            oUserKeysMD.KeyName = KeyName;

            string[] arrAux = Fields.Split(Convert.ToChar(","));
            for (int i = 0; i < arrAux.Length; i++)
            {
                if (i > 0)
                    oUserKeysMD.Elements.Add();

                oUserKeysMD.Elements.ColumnAlias = arrAux[i].Trim();

            }

            oUserKeysMD.Unique = GetSapBoolean(isUnique);

            string Retorno = "";

            CodErro = oUserKeysMD.Add();
            this.ValidateAction();

            Marshal.ReleaseComObject(oUserKeysMD);
            oUserKeysMD = null;

            Log.AppendLine(KeyName + " " + Retorno);
            return Retorno;
        }

        public void ValidateAction()
        {
            if (CodErro != 0)
            {
                SBOApp.Company.GetLastError(out CodErro, out MsgErro);
                Log.AppendFormat("FALHA ({0}){1}", MsgErro, Environment.NewLine);
            }
            else
            {
                MsgErro = "";
                Log.AppendLine(" OK ");
            }
        }

        public void MakeFieldsSearchable(string tableInitial)
        {
            Log.AppendFormat("Liberando campos para pesquisa");

            Recordset rs = (Recordset)(SBOApp.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset));
            string sql = "SELECT * FROM CUFD WHERE TableID LIKE '{0}%'";

            rs.DoQuery(String.Format(sql, tableInitial));
            string tableName = String.Empty;
            UserObjectsMD userObjectsMD = (UserObjectsMD)SBOApp.Company.GetBusinessObject(BoObjectTypes.oUserObjectsMD);

            while (!rs.EoF)
            {
                if (tableName != rs.Fields.Item("TableID").Value.ToString())
                {
                    tableName = rs.Fields.Item("TableID").Value.ToString();
                }

                // Remove a arroba do usertable Name
                tableName = tableName.Replace("@", "");

                bool bUpdate = userObjectsMD.GetByKey(tableName);

                userObjectsMD.FindColumns.ColumnAlias = rs.Fields.Item("AliasID").Value.ToString();
                userObjectsMD.FindColumns.ColumnDescription = rs.Fields.Item("Descr").Value.ToString();
                userObjectsMD.FindColumns.Add();

                if (bUpdate)
                {
                    CodErro = userObjectsMD.Update();
                }
                else
                {
                    CodErro = userObjectsMD.Add();
                }

                this.ValidateAction();

                Marshal.ReleaseComObject(userObjectsMD);
                userObjectsMD = null;
            }
        }

        public static SAPbobsCOM.BoYesNoEnum GetSapBoolean(bool Variavel)
        {
            if (Variavel)
                return SAPbobsCOM.BoYesNoEnum.tYES;
            else
                return SAPbobsCOM.BoYesNoEnum.tNO;

        }
    }
}
