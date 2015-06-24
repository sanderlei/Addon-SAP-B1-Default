using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.DataBase
{
    public class Date
    {
        /// <summary>
        /// Formato em que está a data
        /// b = barra
        /// t = traço
        /// </summary>
        public enum StringFormat
        {
            yyyyMMdd,
            ddMMyyyy,
            ddbMMbyyyy
        };

        public static string ConvertToDate(string dateValue, StringFormat stringFormat)
        {
            switch (stringFormat)
            {
                case StringFormat.yyyyMMdd:
                    return String.Format(" CONVERT(datetime, '{0}', 112) ", dateValue);
                case StringFormat.ddMMyyyy:
                    break;
                default:
                    break;
            }
            return "";
        }
    }
}
