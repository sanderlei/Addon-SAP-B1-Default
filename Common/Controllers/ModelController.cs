using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Enums;

namespace Common.Controllers
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ModelControllerAttribute : Attribute
    {
        /// <summary>
        /// Nome da Coluna na tabela
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Name da Tabela
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Nome do campo na tela
        /// </summary>
        public string UIFieldName { get; set; }

        /// <summary>
        /// Descricao do campo
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Campo obrigatório
        /// </summary>
        public bool MandatoryYN { get; set; }

        /// <summary>
        /// Campo para ser tratado no banco de dados?
        /// </summary>
        public bool DataBaseFieldYN { get; set; }

        public ModelControllerAttribute()
        {
            DataBaseFieldYN = true;
        }
    }
}
