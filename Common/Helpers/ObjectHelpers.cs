using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace Common.Helpers
{
	public class ObjectHelpers
	{
		public static readonly Type TypeOfBoolean = typeof(bool);
		public static readonly Type TypeOfDateTime = typeof(DateTime);
		public static readonly Type TypeOfDouble = typeof(double);
		public static readonly Type TypeOfInt = typeof(int);
		public static readonly Type TypeOfNullable = typeof(Nullable<>);
		public static readonly Type TypeOfObject = typeof(object);
		public static readonly Type TypeOfString = typeof(string);

		public static Dictionary<string, PropertyInfo> LoadProperties(Type type, Func<PropertyInfo, bool> where,
			bool useXmlAttributes = true)
		{
			var result = type.GetProperties().Where(where).ToDictionary(p => p.Name, p => p);

			if (useXmlAttributes)
			{
				ReplaceWithXmlInfo(result);
			}

			return result;
		}

		public static Type GetSpecificType(Type type, out bool nullable)
		{
			if (type.IsClass)
			{
				nullable = true;
			}
			else if (type.IsGenericType && type.GetGenericTypeDefinition() == TypeOfNullable)
			{
				nullable = true;

				type = type.GetGenericArguments().First();
			}
			else
			{
				nullable = false;
			}

			return type;
		}

		internal static void ReplaceWithXmlInfo(Dictionary<string, PropertyInfo> propertyInfos)
		{
			var typeOfXmlAttribute = typeof(XmlElementAttribute);

			foreach (var pair in propertyInfos.Values.ToDictionary(p => p,
				p => (XmlElementAttribute)p.GetCustomAttributes(typeOfXmlAttribute, true).FirstOrDefault())
				.Where(kv => kv.Value != null && !String.IsNullOrWhiteSpace(kv.Value.ElementName) && kv.Value.ElementName != kv.Key.Name))
			{
				propertyInfos.Remove(pair.Key.Name);
				propertyInfos.Add(pair.Value.ElementName, pair.Key);
			}
		}
	}
}