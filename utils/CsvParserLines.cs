using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace app.mma.utils {
	public class CsvParserLines : List<string[]>
	{
		protected string csv = string.Empty;
		protected string separator = "" + System.Environment.NewLine;

		public CsvParserLines(string csvFileName, string separator)
		{
			this.csv = (new StreamReader(csvFileName)).ReadToEnd();
			if( separator != null) this.separator = separator;

			foreach (string line in Regex.Split(this.csv, System.Environment.NewLine) )
			{
				string[] values = Regex.Split(line, separator);

				for (int i = 0; i < values.Length; i++)
				{
					//Trim values
					values[i] = values[i].Trim('\"');
				}

				this.Add(values);
			}
		}
	}
	/*
	public class CsvParserLines : List<string[]> {
		public CsvParserLines(string csvFileName, char separator) {
			char[] separatorArray = new char[] {separator};
			string[] lines = File.ReadAllLines(csvFileName);

			foreach (string line in lines) {
				string[] values = line.Split(separatorArray);
				int valuesCount = values.Length;

				for (int i = 0; i < valuesCount; i++)
					values[i] = values[i].Trim();

				this.Add(values);
			}
		}
	}
	*/
}