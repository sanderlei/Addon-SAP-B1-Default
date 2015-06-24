using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using SAPbobsCOM;

namespace Common.Helpers.DataInterface
{
	public class RecordsetHelpers
	{
		public static Expression<Func<IFields, TDestiny>> GetMemberInit<TDestiny>(IFields fields, bool useXmlAttributes = true)
			where TDestiny : new()
		{
			var typeOfDestiny = typeof(TDestiny);
			var props = ObjectHelpers.LoadProperties(typeOfDestiny, p => p.CanWrite, useXmlAttributes);

			var typeOfFields = typeof(IFields);
			var methodField = typeOfFields.GetMethod("Item");
			var propertyValue = typeof(IField).GetProperty("Value");

			var count = fields.Count;
			var paramSource = Expression.Parameter(typeOfFields, "fields");
			var bindings = new List<MemberBinding>(count);

			for (var i = 0; i < count; ++i)
			{
				var field = fields.Item(i);
				PropertyInfo propertyInfo;

				if (!props.TryGetValue(field.Name, out propertyInfo))
				{
					continue;
				}

				MemberBinding binding;
				var value = Expression.Property(
					Expression.Call(paramSource, methodField, Expression.Constant(i, ObjectHelpers.TypeOfObject)), propertyValue);

				if (propertyInfo.PropertyType.Name == "Boolean" && field.Type == BoFieldTypes.db_Alpha && field.Size == 1)
				{
					binding = Expression.Bind(
						propertyInfo,
						Expression.Equal(Expression.Convert(value, ObjectHelpers.TypeOfString), Expression.Constant("Y")));
					//binding = Expression.Bind(propertyInfo, Expression.Condition(
					//	Expression.Equal(
					//		Expression.Convert(value, typeOfString),
					//		Expression.Constant("Y")),
					//	Expression.Constant(true),
					//	Expression.Constant(false)));

				}
				else
				{
					binding = Expression.Bind(propertyInfo, Expression.Convert(value, propertyInfo.PropertyType));
				}

				bindings.Add(binding);
			}

			var lambda = Expression.Lambda<Func<IFields, TDestiny>>(
				Expression.MemberInit(Expression.New(typeOfDestiny), bindings),
				new[] { paramSource });

			return lambda;
		}

		public static IEnumerable<TDestiny> AsEnumerable<TDestiny>(IRecordset source, bool useXmlAttributes = true)
			where TDestiny : new()
		{
			if (source == null) throw new ArgumentNullException("source");

			if (source.RecordCount == 0)
				yield break;

			source.MoveFirst();

			var fnMemberInit = GetMemberInit<TDestiny>(source.Fields, useXmlAttributes).Compile();

			while (!source.EoF)
			{
				yield return fnMemberInit(source.Fields);

				source.MoveNext();
			}
		}

		public static IEnumerable<TDestiny> AsEnumerable<TRecordset, TDestiny>(TRecordset source, Func<TRecordset, TDestiny> select)
			where TRecordset : class, IRecordset
		{
			if (source == null) throw new ArgumentNullException("source");
			if (select == null) throw new ArgumentNullException("select");

			source.MoveFirst();

			while (!source.EoF)
			{
				yield return select(source);

				source.MoveNext();
			}
		}

		public static IEnumerable<TDestiny> AsEnumerable<TRecordset, TFields, TDestiny>(TRecordset source,
			Func<TRecordset, TFields> fields, Func<TFields, TDestiny> select)
			where TRecordset : class, IRecordset
			where TFields : class, IFields
		{
			if (source == null) throw new ArgumentNullException("source");
			if (select == null) throw new ArgumentNullException("select");

			source.MoveFirst();

			while (!source.EoF)
			{
				yield return select(fields(source));

				source.MoveNext();
			}
		}
	 
	}
}