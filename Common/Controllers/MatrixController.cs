using SAPbouiCOM;

namespace Common.Controllers
{
	public class MatrixController
	{
		#region AddMatrixRow
		public void AddMatrixRow(IMatrix matrix, IDBDataSource dBDataSource, bool focusCell)
		{
			// Insere um novo registro vazio dentro do data source
			dBDataSource.InsertRecord(dBDataSource.Size);

			if (dBDataSource.Size == 1)
			{
				dBDataSource.InsertRecord(dBDataSource.Size);
			}

			if (matrix.VisualRowCount.Equals(0))
			{
				dBDataSource.RemoveRecord(0);
			}

			// Loads the user interface with current data from the matrix objects data source.
			matrix.LoadFromDataSourceEx(false);

			if (focusCell)
				matrix.SetCellFocus(matrix.VisualRowCount, 1);
		}
		#endregion

		#region RemoveMatrixRow
		public void RemoveMatrixRow(IMatrix matrix, IDBDataSource dBDataSource, int row)
		{
			dBDataSource.RemoveRecord(row - 1);

			// Loads the user interface with current data from the matrix objects data source.
			matrix.LoadFromDataSource();
		}
		#endregion
	}
}
