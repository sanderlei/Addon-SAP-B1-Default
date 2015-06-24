using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml.Serialization;

namespace Common.Util
{
    public class XmlSerializationUtil
    {
        public object GetEnumValue<T>(string value)
        {
            foreach (object o in System.Enum.GetValues(typeof(T)))
            {
                T enumValue = (T)o;
                if (GetXmlAttrNameFromEnumValue<T>(enumValue) == value)
                {
                    return enumValue;
                }
            }

            return null;
            //throw new ArgumentException("No code exists for type " + typeof(T).ToString() + " corresponding to value of " + value);
        }

        public string GetCode<T>(string value)
        {
            foreach (object o in System.Enum.GetValues(typeof(T)))
            {
                T enumValue = (T)o;
                if (enumValue.ToString() == value)
                {
                    return GetXmlAttrNameFromEnumValue<T>(enumValue);
                }
            }
            return String.Empty;
            //throw new ArgumentException("No code exists for type " + typeof(T).ToString() + " corresponding to value of " + value);
        }

        public string GetXmlAttrNameFromEnumValue<T>(T pEnumVal)
        {
            Type type = pEnumVal.GetType();
            FieldInfo info = type.GetField(Enum.GetName(typeof(T), pEnumVal));
            XmlEnumAttribute xmlEnumAtt;

            foreach (Attribute attribute in info.GetCustomAttributes(false))
            {
                xmlEnumAtt = attribute as XmlEnumAttribute;
                if (xmlEnumAtt != null)
                {
                    return xmlEnumAtt.Name;
                }
            }
            return pEnumVal.ToString();
        }
    }
}
