using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SAPbobsCOM;
using Common.Controllers;
using System.Text.RegularExpressions;

namespace Common.DataBase
{
    public class StoredProcedureController
    {
        private string Server;
        private string Database;
        private string User;
        private string Password;
        public StoredProcedureController(string server, string db, string user, string password)
        {
            this.Server = server;
            this.Database = db;
            this.User = user;
            this.Password = password;
        }

        public void CreateProcedure(string file)
        {
            SqlController sqlController = new SqlController(this.Server, this.Database, this.User, this.Password);
            
            StreamReader reader;
            Regex r = new Regex(@"^(\s|\t)*go(\s\t)?.*", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            try
            {
                reader = new StreamReader(file);

                string sproc = reader.ReadToEnd();

                foreach (string s in r.Split(sproc))
                {
                    //Skip empty statements, in case of a GO and trailing blanks or something
                    string thisStatement = s.Trim();
                    if (String.IsNullOrEmpty(thisStatement))
                    {
                        continue;
                    }

                    sqlController.ExecuteNonQuery(s);
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void CreateProcedureList(string path)
        {
            SqlController sqlController = new SqlController(this.Server, this.Database, this.User, this.Password);
            string[] filePaths = Directory.GetFiles(path);

            StreamReader reader;
            Regex r = new Regex(@"^(\s|\t)*go(\s\t)?.*", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            foreach (string file in filePaths)
            {
                try
                {
                    reader = new StreamReader(file);

                    string sproc = reader.ReadToEnd();

                    foreach (string s in r.Split(sproc))
                    {
                        //Skip empty statements, in case of a GO and trailing blanks or something
                        string thisStatement = s.Trim();
                        if (String.IsNullOrEmpty(thisStatement))
                        {
                            continue;
                        }

                        sqlController.ExecuteNonQuery(s);
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
