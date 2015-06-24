using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Common.Helpers
{
	public static class XmlHelpers
	{
		public static XmlDocument ToXmlDocument<T, TItem>(this T source, int count, Func<T, int, TItem> fnGetItem,
			Func<TItem, string> fnGetName, Func<TItem, string> fnGetValue, string documentName)
		{
			var xmlDocument = new XmlDocument();
			var document = xmlDocument.CreateElement(documentName);

			xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "utf-16", null));
			xmlDocument.AppendChild(document);
			document.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");

			for (var i = 0; i < count; ++i)
			{
				var item = fnGetItem(source, i);
				var element = xmlDocument.CreateElement(fnGetName(item));
				var value = fnGetValue(item);

				document.AppendChild(element);

				if (value == null)
				{
					var xsinil = xmlDocument.CreateAttribute("xsi", "nil", "http://www.w3.org/2001/XMLSchema-instance");
					xsinil.Value = "true";
					element.SetAttributeNode(xsinil);
				}
				else
				{
					element.InnerText = fnGetValue(item);
				}
			}

			return xmlDocument;
		}

		public static T ToObject<T, TItem>(this XmlDocument xmlDocument, T destiny, int count, Func<T, int, TItem> fnGetItem,
			Func<TItem, string> fnGetName, Action<TItem, string> acSetValue, string documentName = null)
		{
			var document = documentName == null ? xmlDocument.DocumentElement : xmlDocument[documentName];

			for (var i = 0; i < count; ++i)
			{
				var item = fnGetItem(destiny, i);

				var element = document[fnGetName(item)];

				if (element != null)
				{
					acSetValue(item, element.InnerText);
				}
			}

			return destiny;
		}

		public static XmlDocument Serialize<T>(this T source)
		{
			var typeOfSource = source.GetType();
			var serializer = new XmlSerializer(typeOfSource);
			var stringBuilder = new StringBuilder();
			var stringWriter = new StringWriter(stringBuilder);

			serializer.Serialize(stringWriter, source);

			var xmlDocument = new XmlDocument();

			xmlDocument.LoadXml(stringBuilder.ToString());

			return xmlDocument;
		}

		public static T Deserialize<T>(this XmlDocument xmlDocument)
		{
			if (xmlDocument == null)
				throw new ArgumentNullException("xmlDocument");

			var deserializer = new XmlSerializer(typeof (T));
			var reader = new XmlNodeReader(xmlDocument.DocumentElement);
			var result = (T) deserializer.Deserialize(reader);

			return result;
		}
	}
}