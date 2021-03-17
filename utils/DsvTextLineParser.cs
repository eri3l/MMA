using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace app.mma.utils {
	/// <summary>
	/// Delimiter-separated values text stream parser.
	/// </summary>
	public class DsvTextLineParser {
		public char Delimiter { get; set;}
		public string TextLine { get; set;}
		private Dictionary<string, int> columnNameToIndexMap = null;

		public DsvTextLineParser(string textLineWithColumnNames, char delimiter) {
			this.Delimiter = delimiter;
			this.BuildColumnsMap(textLineWithColumnNames);
		}

		public void BuildColumnsMap(string textLineWithColumnNames) {
			if (string.IsNullOrEmpty(textLineWithColumnNames))
				throw new ArgumentException("The \"textLineWithColumnNames\" parameter is null or empty", "textLineWithColumnNames");

			string[] columnChunks = textLineWithColumnNames.Split(new char[] {this.Delimiter});
			int columnsCount = columnChunks.Length;

			this.columnNameToIndexMap = new Dictionary<string, int>(columnsCount);

			for (int i = 0; i < columnsCount; i++) {
				string columnName = columnChunks[i];
				this.columnNameToIndexMap.Add(columnName, i);
			}
		}
	}
}

