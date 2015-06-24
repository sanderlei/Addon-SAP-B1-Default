using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common.Controllers
{
    public class SqlController
    {
        private SqlConnection Connection = new SqlConnection();
        private SqlDataAdapter DataAdapter = new SqlDataAdapter();
        private SqlDataReader DataReader;
        private SqlCommand Command;
        private SqlTransaction Transaction;

        private string ConnectionString;

        public SqlController(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public SqlController(string serverName, string dataBaseName, string userName, string userPassword)
        {
            this.ConnectionString = this.GetConnectionString(serverName, dataBaseName, userName, userPassword);
        }

        public void Connect()
        {
            if (Connection.State == ConnectionState.Broken || Connection.State == ConnectionState.Closed)
            {
                try
                {
                    this.Connection.ConnectionString = this.ConnectionString;
                    this.Connection.Open();
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao conectar SQL: " + ex.Message);
                }
            }
        }

        public void Close()
        {
            if (Connection.State == ConnectionState.Open || Connection.State == ConnectionState.Executing || Connection.State == ConnectionState.Fetching)
            {
                Connection.Close();
                Connection.Dispose();
                Connection = null;
            }
        }

        public SqlDataReader ExecuteReader(string sql)
        {
            try
            {
                this.Connect();
                this.Command = new SqlCommand(sql, Connection);
                this.DataReader = Command.ExecuteReader();
                return this.DataReader;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao executar SqlDataReader: " + ex.Message);
            }
        }

        public object ExecuteScalar(string sql)
        {
            try
            {
                this.Connect();
                this.Command = new SqlCommand(sql, Connection);
                this.Command.Transaction = Transaction;
                return this.Command.ExecuteScalar();
            }
            catch (Exception ex)
            {

                throw new Exception("Erro ao executar ExecuteScalar: " + ex.Message);

            }
        }

        public void ExecuteNonQuery(string sql)
        {
            try
            {
                this.Connect();
                this.Command = new SqlCommand(sql, Connection);
                this.Command.Transaction = Transaction;
                this.Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao executar ExecuteNonQuery: " + ex.Message);
            }
        }

        public DataTable FillDataTable(string sql)
        {
            try
            {
                this.Connect();
                this.Command = new SqlCommand(sql, Connection);
                this.Command.CommandType = CommandType.Text;
                this.Command.CommandText = sql;
                DataAdapter.SelectCommand = this.Command;

                DataTable dtb = new DataTable();

                DataAdapter.Fill(dtb);
                DataAdapter.Dispose();
                return dtb;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao executar FillDataTable: " + ex.Message);
            }
        }

        public void BeginTransaction()
        {
            Transaction = this.Connection.BeginTransaction();
        }

        public void RollbackTransaction()
        {
            Transaction.Rollback();
        }

        public void CommitTransaction()
        {
            Transaction.Commit();
        }

        public string GetConnectionString(string serverName, string dataBaseName, string userName, string userPassword)
        {
            string connectionString = String.Format(@" data source={0};initial catalog={1};persist security info=True;user id={2};password={3};",
                                                serverName,
                                                dataBaseName,
                                                userName,
                                                userPassword);
            return connectionString;
        }

        public T FillModel<T>(string sql)
        {
            List<T> modelList = this.FillModelList<T>(sql);
            if (modelList.Count > 0)
            {
                return modelList[0];
            }
            else
            { 
                return Activator.CreateInstance<T>();
            }
        }

        public List<T> FillModelList<T>(string sql)
        {
            List<T> modelList = new List<T>();
            T model;
            ModelControllerAttribute modelController;

            using (SqlDataReader dr = this.ExecuteReader(sql))
            {
                while (dr.Read())
                {
                    // Cria nova instância do model
                    model = Activator.CreateInstance<T>();
                    // Seta os valores no model
                    foreach (PropertyInfo property in model.GetType().GetProperties())
                    {
                        // Busca os Custom Attributes
                        foreach (Attribute attribute in property.GetCustomAttributes(true))
                        {
                            modelController = attribute as ModelControllerAttribute;
                            if (modelController != null)
                            {
                                // Se propriedade "ColumnName" estiver vazia, pega o nome da propriedade
                                if (String.IsNullOrEmpty(modelController.ColumnName))
                                {
                                    modelController.ColumnName = property.Name;
                                }
                                if (!modelController.DataBaseFieldYN)
                                {
                                    break;
                                }

                                if (!dr.IsDBNull(dr.GetOrdinal(modelController.ColumnName)))
                                {
                                    property.SetValue(model, dr.GetValue(dr.GetOrdinal(modelController.ColumnName)), null);
                                }
                            }
                        }
                    }
                    modelList.Add(model);
                }
            }
            return modelList;
        }
    }
}
