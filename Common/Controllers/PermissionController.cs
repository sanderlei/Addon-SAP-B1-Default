using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Common.Models;

namespace Common.Controllers
{
    public class PermissionController
    {
        public string CreateUserPermission(PermissionModel model, bool recreate = false, bool AddPermissionsToUsers = true)
        {
            UserPermissionTree oUserPermissionTree = (UserPermissionTree)SBOApp.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserPermissionTree);
            string msg = String.Empty;
            try
            {
                bool update = oUserPermissionTree.GetByKey(model.PermissionID);
                if (update && recreate)
                {
                    int permission = oUserPermissionTree.Remove();
                }

                oUserPermissionTree.PermissionID = model.PermissionID;
                oUserPermissionTree.Name = model.Name;
                oUserPermissionTree.Options = model.Options;
                oUserPermissionTree.IsItem = model.IsItem;

                if (!String.IsNullOrEmpty(model.ParentId))
                {
                    oUserPermissionTree.ParentID = model.ParentId;
                }

                if (model.IsItem == SAPbobsCOM.BoYesNoEnum.tYES && !String.IsNullOrEmpty(model.FormType))
                {
                    bool formExists = false;
                    int i;
                    for (i = 0; i < oUserPermissionTree.UserPermissionForms.Count; i++)
                    {
                        oUserPermissionTree.UserPermissionForms.SetCurrentLine(i);
                        if (oUserPermissionTree.UserPermissionForms.FormType == model.FormType)
                        {
                            formExists = true;
                            msg = String.Format("Permissão {0} - {1} já existe", model.PermissionID, model.Name);
                        }
                    }

                    if (!formExists)
                    {
                        if (!String.IsNullOrEmpty(oUserPermissionTree.UserPermissionForms.FormType))
                        {
                            oUserPermissionTree.UserPermissionForms.Add();
                        }

                        oUserPermissionTree.UserPermissionForms.FormType = model.FormType;
                    }
                }

                int error = 0;
                error = oUserPermissionTree.Add();
                if (update && !recreate)
                {
                    error = oUserPermissionTree.Update();
                }
                else
                {
                    error = oUserPermissionTree.Add();
                }

                if (error != 0)
                {
                    msg = ErrorController.GetLastErrorDescription();
                }
                else if (AddPermissionsToUsers)
                {
                    msg = this.SetPermissionForAllUsers(model.PermissionID);
                }
            }
            catch (Exception e)
            {
                msg = "Erro geral ao criar permissão: " + e.Message;
            }
            finally
            {
                Marshal.ReleaseComObject(oUserPermissionTree);
                oUserPermissionTree = null;
            }

            return msg;
        }

        public string SetPermissionForAllUsers(string permissionId)
        {
            string msg = String.Empty;

            Recordset rst = (Recordset)SBOApp.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
            try
            {
                // Busca apenas usuários que ainda não possuem a permissão e que não foram deletados (diferente de 99)
                string sql = @" SELECT UserId FROM OUSR 
                                WHERE UserId NOT IN (SELECT UserLink FROM USR3 WHERE PermId = '{0}')
                                AND Groups <> 99";

                sql = String.Format(sql, permissionId);
                rst.DoQuery(sql);
                List<int> usersList = new List<int>();

                while (!rst.EoF)
                {
                    usersList.Add((int)rst.Fields.Item("UserId").Value);
                    rst.MoveNext();
                }

                return this.SetPermissionToUserList(permissionId, usersList);
            }
            catch (Exception e)
            {
                msg = "Erro geral ao criar permissão: " + e.Message;
            }
            finally
            {
                Marshal.ReleaseComObject(rst);
                rst = null;
            }

            return msg;
        }

        public string SetPermissionToUserList(string permissionId, List<int> usersList)
        {
            Users oUser = (Users)(SBOApp.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUsers));
            string msg = String.Empty;
            try
            {
                foreach (int userId in usersList)
                {
                    if (oUser.GetByKey(userId))
                    {
                        oUser.UserPermission.Add();
                        oUser.UserPermission.SetCurrentLine(0);
                        oUser.UserPermission.PermissionID = permissionId;
                        oUser.UserPermission.Permission = SAPbobsCOM.BoPermission.boper_Full;

                        if (oUser.Update() != 0)
                        {
                            msg += ErrorController.GetLastErrorDescription();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                msg = "Erro geral ao criar permissão: " + e.Message;
            }
            finally
            {
                Marshal.ReleaseComObject(oUser);
                oUser = null;
            }

            return msg;
        }
    }
}
