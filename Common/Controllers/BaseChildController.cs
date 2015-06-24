using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Controllers
{
    public class BaseChildController
    {
        #region Properties
        private CrudController CrudController;
        private CrudChildController CrudChildController;
        private string tableName;
        #endregion Properties

        #region Constructor
        public BaseChildController(string parentTable, string tableName)
        {
            CrudController = new CrudController(tableName);
            CrudChildController = new CrudChildController(parentTable, tableName);
            this.tableName = tableName;
        }
        #endregion Constructor

        #region CRUD
        public virtual string RetrieveModelSql(Type modelType, string where, string orderBy, bool getValidValues)
        {
            return CrudController.RetrieveModelSql(modelType, where, orderBy, getValidValues);
        }

        public virtual string RetrieveSqlModel(Type modelType, string where, bool getValidValues)
        {
            return this.RetrieveModelSql(modelType, where, String.Empty, getValidValues);
        }

        public virtual string RetrieveSqlModel(Type modelType, bool getValidValues)
        {
            return this.RetrieveModelSql(modelType, String.Empty, String.Empty, getValidValues);
        }

        public virtual void CreateModel(object model, object parentCode)
        {
            CrudChildController.Model = model;
            CrudChildController.CreateModel(parentCode);
        }

        public virtual void CreateModelList(object[] modelList, object parentCode)
        {
            CrudChildController.ModelList = new List<object>();
            CrudChildController.ModelList.AddRange(modelList);
            CrudChildController.CreateModelList(parentCode);
        }

        public virtual void UpdateModel(object model, object parentCode)
        {
            CrudChildController.Model = model;
            CrudChildController.UpdateModel(parentCode);
        }

        public virtual void UpdateModelList(object[] modelList, object parentCode)
        {
            CrudChildController.ModelList = new List<object>();
            CrudChildController.ModelList.AddRange(modelList);
            CrudChildController.UpdateModelList(parentCode);
        }

        public virtual T RetrieveModel<T>(string where)
        {
            return CrudController.RetrieveModel<T>(where);
        }

        public virtual List<T> RetrieveModelList<T>(string where)
        {
            return CrudController.RetrieveModelList<T>(where);
        }

        public virtual List<T> RetrieveModelList<T>(string where, string orderBy)
        {
            return CrudController.RetrieveModelList<T>(where, orderBy);
        }

        //public virtual void UpdateModel(object model)
        //{
        //    CrudController.Model = model;
        //    CrudController.UpdateModel();
        //}

        //public virtual void UpdateModel(object model, string where)
        //{
        //    CrudController.Model = model;
        //    CrudController.UpdateModel(where);
        //}

        //public virtual void DeleteModel(string where)
        //{
        //    CrudController.DeleteModel(tableName, where);
        //}

        #endregion

        #region Util
        public virtual string Exists(string where)
        {
            return CrudController.Exists(where);
        }

        public virtual string Exists(string returnColumn, string where)
        {
            return CrudController.Exists(returnColumn, where);
        }

        public virtual List<T> FillModel<T>(string sql)
        {
            return CrudController.FillModel<T>(sql);
        }

        public string GetNextCode()
        {
            return CrudController.GetNextCode(tableName);
        }

        public string GetNextCode(string fieldName)
        {
            return CrudController.GetNextCode(tableName, fieldName);
        }
        #endregion
    }
}
