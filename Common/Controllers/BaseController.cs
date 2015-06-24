using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Controllers;
using SAPbobsCOM;

namespace Common.Controllers
{
    public class BaseController
    {
        #region Properties
        private CrudController crudController;
        private string tableName;
        #endregion Properties

        #region Constructor
        public BaseController(string tableName)
        {
            crudController = new CrudController(tableName);
            this.tableName = tableName;
        }

        public BaseController(string tableName, BoUTBTableType userTableType)
        {
            crudController = new CrudController(tableName);
            this.tableName = tableName;
        }
        #endregion Constructor

        #region CRUD
        public virtual string RetrieveModelSql(Type modelType, string where, string orderBy, bool getValidValues)
        {
            return crudController.RetrieveModelSql(modelType, where, orderBy, getValidValues);
        }

        public virtual string RetrieveSqlModel(Type modelType, string where, bool getValidValues)
        {
            return this.RetrieveModelSql(modelType, where, String.Empty, getValidValues);
        }

        public virtual string RetrieveSqlModel(Type modelType, bool getValidValues)
        {
            return this.RetrieveModelSql(modelType, String.Empty, String.Empty, getValidValues);
        }

        public virtual void CreateModel(object model)
        {
            crudController.Model = model;
            crudController.CreateModel();
        }

        public virtual T RetrieveModel<T>(string where)
        {
            return crudController.RetrieveModel<T>(where);
        }

        public virtual List<T> RetrieveModelList<T>(string where)
        {
            return crudController.RetrieveModelList<T>(where);
        }

        public virtual List<T> RetrieveModelList<T>(string where, string orderBy)
        {
            return crudController.RetrieveModelList<T>(where, orderBy);
        }

        public virtual void UpdateModel(object model)
        {
            crudController.Model = model;
            crudController.UpdateModel();
        }

        public virtual void UpdateModel(object model, string where)
        {
            crudController.Model = model;
            crudController.UpdateModel(where);
        }

        public virtual void DeleteModel(string where)
        {
            crudController.DeleteModel(tableName, where);
        }

        #endregion

        #region Util
        public virtual string Exists(string where)
        {
            return crudController.Exists(where);
        }

        public virtual string Exists(string returnColumn, string where)
        {
            return crudController.Exists(returnColumn, where);
        }

        public virtual List<T> FillModel<T>(string sql)
        {
            return crudController.FillModel<T>(sql);
        }

        public string GetNextCode()
        {
            return CrudController.GetNextCode(tableName);
        }

        public string GetNextCode(string fieldName)
        {
            return CrudController.GetNextCode(tableName, fieldName);
        }

        public string GetNextCode(string fieldName, string where)
        {
            return CrudController.GetNextCode(tableName, fieldName, where);
        }
        #endregion
    }
}
