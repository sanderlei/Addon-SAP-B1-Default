using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Common.Controllers
{
    public class GridController
    {
        public List<T> FillModelFromTable<T>(DataTable table)
        {
            return this.FillModelFromTable<T>(table, false);
        }

        public List<T> FillModelFromTable<T>(DataTable table, bool showProgressBar)
        {
            List<T> modelList = new List<T>();
            // Cria nova instância do model
            T model;

            ProgressBar pgb = null;
            if (showProgressBar)
            {
                pgb = SBOApp.Application.StatusBar.CreateProgressBar("Carregando dados da tabela", table.Rows.Count, false);
            }
            ModelControllerAttribute modelController;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (showProgressBar)
                {
                    pgb.Value++;
                }
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
                            if (String.IsNullOrEmpty(modelController.UIFieldName))
                            {
                                modelController.UIFieldName = modelController.Description;
                            }

                            if (String.IsNullOrEmpty(modelController.UIFieldName))
                            {
                                break;
                            }
                            else
                            {
                                property.SetValue(model, table.Columns.Item(modelController.UIFieldName).Cells.Item(i).Value, null);
                            }
                        }
                    }
                }
                modelList.Add(model);
            }
            if (pgb != null)
            {
                pgb.Stop();
                Marshal.ReleaseComObject(pgb);
                pgb = null;
            }

            return modelList;
        }

        public List<T> FillModelFromGrid<T>(Grid grid)
        {
            DataTable table = grid.DataTable;
            return this.FillModelFromTable<T>(table);
        }

        /// <summary>
        /// Preenche lista de acordo com valor em determinada coluna
        /// </summary>
        /// <param name="columnName">Nome da coluna que irá retornar na lista</param>
        /// <param name="clmToCheck">Nome da coluna para verificar o valor</param>
        /// <param name="valueToCheck">Valor a ser verificado</param>
        /// <param name="table">Tabela</param>
        /// <returns></returns>
        public List<string> FillListAccordingToValue(string columnName, string clmToCheck, string valueToCheck, DataTable table)
        {
            List<string> list = new List<string>();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (table.GetValue(clmToCheck, i).ToString() == valueToCheck)
                {
                    list.Add(table.GetValue(columnName, i).ToString());
                }
            }

            return list;
        }

        public List<T> FillModelFromGrid<T>(Grid grid, List<int> rows)
        {
            List<T> modelList = new List<T>();
            // Cria nova instância do model
            T model;
            DataTable table = grid.DataTable;
            ModelControllerAttribute modelController;
            for (int i = 0; i < rows.Count; i++)
            {
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
                            if (String.IsNullOrEmpty(modelController.UIFieldName))
                            {
                                modelController.UIFieldName = modelController.Description;
                            }

                            if (String.IsNullOrEmpty(modelController.UIFieldName))
                            {
                                break;
                            }
                            else
                            {
                                property.SetValue(model, table.GetValue(modelController.UIFieldName, i), null);
                            }
                        }
                    }
                }
                modelList.Add(model);
            }

            return modelList;
        }
    }
}
