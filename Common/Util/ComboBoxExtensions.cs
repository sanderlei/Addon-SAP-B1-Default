using System;
using SAPbouiCOM;
using SAPbobsCOM;

namespace Common.Util
{
    public static class ComboBoxExtensions
    {
        /// <summary>
        /// Preenche um ComboBox com os dados de um Recordset.
        /// </summary>
        /// <param name="comboBox">O ComboBox a ser preenchido.</param>
        /// <param name="recordset">O Recordset com os dados.</param>
        /// <param name="fieldValue">O nome do Field que contém o valor para ComboBox.</param>
        /// <param name="fieldDescription">O nome do Field que contém a descrição para o ComboBox.</param>
        public static Recordset AddValuesFromRecordset(this ComboBox comboBox, Recordset recordset, string fieldValue = null,
                                                string fieldDescription = null)
        {
            if (comboBox == null) throw new ArgumentNullException("comboBox");
            if (recordset == null) throw new ArgumentNullException("recordset");

            recordset.MoveFirst();

            if (fieldDescription == null)
            {
                if (fieldValue == null)
                {
                    var fields = recordset.Fields;

                    if (fields.Count > 1)
                    {
                        fieldValue = fields.Item(0).Name;
                        fieldDescription = fields.Item(1).Name;
                    }
                    else
                    {
                        fieldValue = fieldDescription = fields.Item(0).Name;
                    }
                }
                else
                {
                    var fields = recordset.Fields;

                    if (fields.Count > 1)
                    {
                        fieldDescription = fields.Item(0).Name == fieldValue ? fields.Item(1).Name : fields.Item(0).Name;
                    }
                    else
                    {
                        fieldDescription = fieldValue;
                    }
                }
            }

            var validValues = comboBox.ValidValues;

            while (!recordset.EoF)
            {
                var value = recordset.Fields.Item(fieldValue);
                var description = recordset.Fields.Item(fieldDescription);

                validValues.Add(value.Value.ToString(), description.Value.ToString());

                recordset.MoveNext();
            }

            return recordset;
        }

        /// <summary>
        /// Preenche um ComboBox com os dados de uma query.
        /// </summary>
        /// <param name="comboBox"></param>
        /// <param name="recordset">O Recordset a ser utilizado para obter os dados.</param>
        /// <param name="query">A query a ser utilizada para obter os dados.</param>
        /// <param name="fieldValue">O nome do Field que contém o valor para ComboBox.</param>
        /// <param name="fieldDescription">O nome do Field que contém a descrição para o ComboBox.</param>
        /// <param name="noLock">Indica se a query deve ser executada na forma de apenas leiura, isto é, se houver alguma escrita no momento não vai esperar a conclusão desta para continuar.</param>
        public static Recordset AddValuesFromQuery(this ComboBox comboBox, Recordset recordset, string query,
                                            string fieldValue = null, string fieldDescription = null, bool noLock = true)
        {
            if (recordset == null) throw new ArgumentNullException("recordset");
            if (query == null) throw new ArgumentNullException("query");

            recordset.DoQuery(noLock ? " SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED " + query : query);

            return AddValuesFromRecordset(comboBox, recordset, fieldValue, fieldDescription);
        }

        /// <summary>
        /// Preenche um ComboBox com os dados de uma query.
        /// </summary>
        /// <param name="comboBox"></param>
        /// <param name="company">O Company a ser utilizado para obter dos dados.</param>
        /// <param name="query">A query a ser utilizada para obter os dados.</param>
        /// <param name="fieldValue">O nome do Field que contém o valor para ComboBox.</param>
        /// <param name="fieldDescription">O nome do Field que contém a descrição para o ComboBox.</param>
        /// <param name="noLock">Indica se a query deve ser executada na forma de apenas leiura, isto é, se houver alguma escrita no momento não vai esperar a conclusão desta para continuar.</param>
        public static Recordset AddValuesFromQuery(this ComboBox comboBox, SAPbobsCOM.Company company, string query,
                                            string fieldValue = null, string fieldDescription = null, bool noLock = true)
        {
            if (company == null) throw new ArgumentNullException("company");

            var recordset = (Recordset)company.GetBusinessObject(BoObjectTypes.BoRecordset);

            return AddValuesFromQuery(comboBox, recordset, query, fieldValue, fieldDescription, noLock);
        }
    }
}
