using System;
using System.Globalization;
using System.Reflection;
using System.Xml;
using SAPbouiCOM;

namespace Common.Helpers.UserInterface
{
	public static class UserDataSourcesHelpers
	{
		public static void FromObject<TSource>(UserDataSources destiny, TSource source, bool useXmlAttributes = true)
			where TSource : class
		{
			if (destiny == null) throw new ArgumentNullException("destiny");
			if (source == null) throw new ArgumentNullException("source");

			var props = ObjectHelpers.LoadProperties(source.GetType(), p => p.CanRead, useXmlAttributes);

			for (var index = 0; index < destiny.Count; ++index)
			{
				var item = destiny.Item(index);
				PropertyInfo propertyInfo;

				if (!props.TryGetValue(item.UID, out propertyInfo))
				{
					continue;
				}

				var value = propertyInfo.GetValue(source, null);

				if (value == null)
				{
					item.Value = String.Empty;
				}
				else
				{
					bool nullable;
					var type = ObjectHelpers.GetSpecificType(propertyInfo.PropertyType, out nullable);

					if (nullable && value.Equals(null))
					{
						item.Value = String.Empty;
					}
					else if (type == ObjectHelpers.TypeOfBoolean)
					{
						item.Value = value.Equals(true) ? "Y" : "N";
					}
					else
					{
						var convertible = value as IConvertible;

						item.Value = convertible != null
							? convertible.ToString(CultureInfo.InvariantCulture)
							: value.ToString();
					}
				}
			}
		}

		public static TDestiny ToObject<TDestiny>(UserDataSources source, bool useXmlAttributes = true)
			where TDestiny : class, new()
		{
			var destiny = new TDestiny();

			ToObject(source, destiny, useXmlAttributes);

			return destiny;
		}

		public static void ToObject<TDestiny>(UserDataSources source, TDestiny destiny, bool useXmlAttributes = true)
			where TDestiny : class
		{
			if (source == null) throw new ArgumentNullException("source");
			if (destiny == null) throw new ArgumentNullException("destiny");

			var props = ObjectHelpers.LoadProperties(destiny.GetType(), p => p.CanWrite, useXmlAttributes);

			for (var index = 0; index < source.Count; ++index)
			{
				var item = source.Item(index);
				PropertyInfo propertyInfo;

				if (!props.TryGetValue(item.UID, out propertyInfo))
				{
					continue;
				}

				object value;
				bool nullable;
				var type = ObjectHelpers.GetSpecificType(propertyInfo.PropertyType, out nullable);

				if (nullable && String.IsNullOrEmpty(item.ValueEx))
				{
					value = null;
				}
				else if (type == ObjectHelpers.TypeOfString)
				{
					value = item.ValueEx;
				}
				else if (type == ObjectHelpers.TypeOfInt)
				{
					value = int.Parse(item.ValueEx);
				}
				else if (type == ObjectHelpers.TypeOfDateTime)
				{
					value = DateTime.ParseExact(item.ValueEx, item.DataType == BoDataType.dt_DATE 
						? "yyyyMMdd" 
						: "HH:mm", CultureInfo.InvariantCulture);
				}
				else if (type == ObjectHelpers.TypeOfDouble)
				{
					value = double.Parse(item.ValueEx, CultureInfo.InvariantCulture);
				}
				else if (type == ObjectHelpers.TypeOfBoolean)
				{
					value = (item.ValueEx == "Y");
				}
				else
				{
					continue;
				}

				propertyInfo.SetValue(destiny, value, null);
			}

		}

		public static XmlDocument SAPObjectToXml(this UserDataSources userDataSources, string documentName)
		{
			var result = userDataSources.ToXmlDocument(userDataSources.Count, GetItem, GetName, GetValue, documentName);

			return result;
		}

		public static UserDataSources SAPObjetFromXml(this UserDataSources userDataSources, XmlDocument xmlDocument,
			string documentName = null)
		{
			var result = xmlDocument.ToObject(userDataSources, userDataSources.Count, GetItem, GetName, SetValue, documentName);

			return result;
		}

		internal static UserDataSource GetItem(UserDataSources ds, int index)
		{
			return ds.Item(index);
		}

		internal static string GetName(UserDataSource item)
		{
			return item.UID;
		}

		internal static string GetValue(UserDataSource item)
		{
			if (item.DataType == BoDataType.dt_SHORT_TEXT && item.Length == 1)
			{
				if (item.ValueEx == "Y")
				{
					return "true";
				}

				if (item.ValueEx == "N")
				{
					return "false";
				}
			}

			return item.ValueEx.Length == 0 ? null : item.ValueEx;
		}

		internal static void SetValue(UserDataSource item, string value)
		{
			if (item.DataType == BoDataType.dt_SHORT_TEXT && item.Length == 1)
			{
				if (bool.TrueString.Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					item.ValueEx = "Y";

					return;
				}

				if (bool.FalseString.Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					item.ValueEx = "N";

					return;
				}
			}

			item.Value = value ?? String.Empty;
		} 
 
	}
}