using System;
using System.Collections;
using System.Web;

namespace app.mma.utils {
	internal static class ValidatorUtils {
		public static void SanitiseInputParams(ArrayList inputParams) {
			if (inputParams == null)
				return;

			int paramsCount = inputParams.Count;

			for (int i = 0; i < paramsCount; i++) {
				string paramValue = Convert.ToString(inputParams[i]);

				switch (paramValue) {
					case "undefined":
					case "null":
					inputParams[i] = string.Empty;
					break;
				}
			}
		}

		public static void SanitiseInputParams(string[] inputParams) {
			if (inputParams == null)
				return;

			int paramsCount = inputParams.Length;

			for (int i = 0; i < paramsCount; i++) {
				string paramValue = Convert.ToString(inputParams[i]);

				switch (paramValue) {
					case "undefined":
					case "null":
					inputParams[i] = string.Empty;
					break;
				}
			}
		}

		private static int[] GetArrayColumnsIndexes(object[,] array, string[] columns) {
			if (array == null)
				throw new ArgumentNullException("array");

			if (columns == null)
				throw new ArgumentNullException("columns");

			int rowsCount = array.GetLength(0);
			int colsCount = array.GetLength(1);

			if (rowsCount  == 0)
				throw new ArgumentException("The source array must contain at least one row", "array");

			// Get columns indexes.
			int colsToEncodeCount = columns.Length;
			int[] colsIndexes = new int[colsToEncodeCount];

			for (int i = 0; i < colsToEncodeCount; i++) {
				string colName = columns[i];
				bool colFound = false;

				for (int j = 0; j < colsCount; j++) {
					int compareResult = string.Compare(colName, Convert.ToString(array[0, j]), false);

					if (compareResult == 0) {
						colsIndexes[i] = j;
						colFound = true;
						break;
					}
				}

				if (!colFound)
					throw new ApplicationException (string.Format ("Column \"{0}\" is missing in the source array"));
			}

			return colsIndexes;
		}

		public static void HtmlEncodeArrayColumns(object[,] array, string[] columns) {
			if (array == null)
				throw new ArgumentNullException("array");

			if (columns == null)
				throw new ArgumentNullException("columns");

			int rowsCount = array.GetLength(0);

			if (rowsCount == 0)
				throw new ArgumentException("The source array must contain at least one row.", "array");

			// Get columns indexes.
			int[] colsIndexes = GetArrayColumnsIndexes(array, columns);
			int colsToEncodeCount = colsIndexes.Length;

			foreach (int j in colsIndexes)
				for (int i = 1; i < rowsCount; i++) {
					array [i, j] = HttpUtility.HtmlEncode(Convert.ToString(array[i, j]));
					Console.WriteLine("encoding: " + Convert.ToString(array[i, j]));
				}
		}
	}
}
