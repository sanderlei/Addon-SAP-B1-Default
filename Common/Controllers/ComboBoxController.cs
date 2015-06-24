using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Common.Models;

namespace Common.Controllers
{
    public class ComboBoxController
    {
        /// <summary>
        /// Adiciona valores no combo box através de uma consulta no banco de dados
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">ValidValues</param>
        /// <param name="sql">Query</param>
        /// <param name="valueField">Campo que contém o valor</param>
        /// <param name="descriptionField">Campo que contém a descrição</param>
        public static void AddValidValues<T>(T obj, String sql, String valueField, String descriptionField)
        {
            List<GenericModel> genericModelList = new GenericController().FillGenericModelList(sql, true);

            foreach (GenericModel genericModel in genericModelList)
            {
                obj.GetType().GetMethod("Add").Invoke(obj, new object[] { genericModel.GetFieldValue(valueField).ToString(), genericModel.GetFieldValue(descriptionField).ToString() });
            }
        }
    }
}
