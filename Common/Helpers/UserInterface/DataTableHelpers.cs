using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using SAPbouiCOM;

namespace Common.Helpers.UserInterface
{
	public class DataTableHelpers
	{
		public static Expression<Func<IDataTable, int, TDestiny>> GetMemberInit<TDestiny>(IDataTable source,
			bool useXmlAttributes = true)
			where TDestiny : new()
		{
			var typeOfDestiny = typeof(TDestiny);

			var props = ObjectHelpers.LoadProperties(typeOfDestiny, p => p.CanWrite, useXmlAttributes);

			var typeOfSource = typeof(IDataTable);
			Expression<Func<IDataTable, object, int, object>> expGetValue = (dt, column, row) => dt.GetValue(column, row);
			var methodGetValue = ((MethodCallExpression)expGetValue.Body).Method;

			var count = source.Columns.Count;
			var paramSource = Expression.Parameter(typeOfSource, "dataTable");
			var paramIndex = Expression.Parameter(typeof(int), "index");
			var bindings = new List<MemberBinding>(count);

			for (var i = 0; i < count; ++i)
			{
				var column = source.Columns.Item(i);
				PropertyInfo propertyInfo;

				if (!props.TryGetValue(column.Name, out propertyInfo))
				{
					continue;
				}

				MemberBinding binding;
				var value = Expression.Call(paramSource, methodGetValue, Expression.Constant(i, ObjectHelpers.TypeOfObject), paramIndex);

				if (propertyInfo.PropertyType.Name == "Boolean" && column.Type == BoFieldsType.ft_AlphaNumeric &&
					column.MaxLength == 1)
				{
					binding = Expression.Bind(
						propertyInfo,
						Expression.Equal(Expression.Convert(value, ObjectHelpers.TypeOfString), Expression.Constant("Y")));
				}
				else
				{
					binding = Expression.Bind(propertyInfo, Expression.Convert(value, propertyInfo.PropertyType));
				}

				bindings.Add(binding);
			}

			var lambda = Expression.Lambda<Func<IDataTable, int, TDestiny>>(
				Expression.MemberInit(Expression.New(typeOfDestiny), bindings),
				new[] { paramSource, paramIndex });

			return lambda;
		}

		public static IEnumerable<TDestiny> AsEnumerable<TDestiny>(IDataTable source, bool useXmlAttributes = true)
			where TDestiny : new()
		{
			if (source == null) throw new ArgumentNullException("source");

			var fnMemberInit = GetMemberInit<TDestiny>(source, useXmlAttributes).Compile();

			for (var i = 0; i < source.Rows.Count; ++i)
			{
				yield return fnMemberInit(source, i);
			}
		}

		public static IEnumerable<TDestiny> AsEnumerable<TDataTable, TDestiny>(TDataTable source,
			Expression<Func<TDataTable, int, TDestiny>> select)
			where TDataTable : class, IDataTable
		{
			if (source == null) throw new ArgumentNullException("source");
			if (select == null) throw new ArgumentNullException("select");

			var fnMemberInit = select.Compile();

			for (var i = 0; i < source.Rows.Count; ++i)
			{
				yield return fnMemberInit(source, i);
			}
		}
		 
	}
}