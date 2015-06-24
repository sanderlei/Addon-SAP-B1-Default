using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SAPbouiCOM;

namespace Common.Helpers.UserInterface
{
	public class MatrixHelpers
	{
		public static IEnumerable<TDestiny> AsEnumerable<TMatrix, TDestiny>(TMatrix source,
			Expression<Func<TMatrix, int, TDestiny>> select)
			where TMatrix : class, Matrix
		{
			if (source == null) throw new ArgumentNullException("source");
			if (select == null) throw new ArgumentNullException("select");

			var fnMemberInit = select.Compile();

			for (var i = 1; i <= source.RowCount; ++i)
			{
				yield return fnMemberInit(source, i);
			}
		}

		public static List<int> ListOfSelectedRows(IMatrix source, BoOrderType orderType = BoOrderType.ot_SelectionOrder, Func<int, int> fnTransform = null)
		{
			if (source == null) throw new ArgumentNullException("source");

			if (fnTransform == null)
			{
				fnTransform = x => x;
			}

			var last = 0;
			var result = new List<int>();

			while ((last = source.GetNextSelectedRow(last, orderType)) != -1)
			{
				result.Add(fnTransform(last));
			}

			return result;
		}
	}
}
