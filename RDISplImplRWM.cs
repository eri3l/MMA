using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using app_c.mma.rdi;
using system.block.item;
using NLog;

namespace app.mma {
	public class RDISplImplRWM {
		private static Logger Log = LogManager.GetCurrentClassLogger();
		// protected RDISplImplRWM_c rdi;
		public RDISplImplRWM_c rdi;

		public RDISplImplRWM() {
		}

		public virtual RetCde n(object[] io) {
			return RetCde.Success;
		}

		public virtual RetCde r(object[] io) {
			return RetCde.Success;
		}

		public virtual RetCde w(object[] io) {
			return RetCde.Success;
		}

		public virtual RetCde m(object[] io) {
			return RetCde.Success;
		}

		public virtual RetCde u(object[] io) {
			return RetCde.Success;
		}

		public virtual RetCde d(object[] io) {
			return RetCde.Success;
		}

		public virtual RetCde rb(object[] io) {
			return RetCde.Success;
		}
/*
		public virtual RetCde id(object[] io) {
			return RetCde.Success;
		}
*/
		public virtual RetCde nm(object[] io) {
			return RetCde.Success;
		}

		public virtual RetCde s(object[] io) {
			string alias;
			string criterion;
			ArrayList parameters;
			long offset;
			long limit;
			string[] fieldNames;

			try {
				alias = (string) io[0];
				criterion = (string) io[1];
				parameters = (ArrayList) io[2];
				offset = io[3] is long ? (long) io[3] : 0L;
				limit = io[4] is long ? (long) io[4] : -1L;
				fieldNames = (string[]) io[5];
			} catch (Exception ex) {
				string errorMessage = "s: error validating input params";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				return RetCde.GeneralError;
			}

			object[,] searchResult;

			try {
				searchResult = this.Search(alias, criterion, parameters, offset, limit, fieldNames);
			} catch (Exception ex) {
				string errorMessage = "s: error getting Channel Node Id";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				return RetCde.GeneralError;
			}

			io[io.Length - 1] = searchResult;
			return RetCde.Success;
		}

		public virtual object[,] Search(string alias, string criterion, ArrayList parameters, long offset, long limit, string[] fieldNames) {
			Log.Log(LogLevel.Debug, string.Format("Search: alias: {0}, criterion: {1}, parameters: {2}, offset: {3}, limit: {4}, fieldNames count: {5}", alias, criterion, parameters.Count, offset, limit, fieldNames == null ? "0" : fieldNames.Length.ToString()));
			int paramsCount = parameters.Count;

			for (int i = 0; i < paramsCount; i++)
				Log.Log(LogLevel.Debug, string.Format("Search: parameter[{0}]: {1}", i, parameters[i]));
			
			long iCritID = this.rdi.getCritID(criterion);

			try {
				this.rdi.search(criterion, parameters.ToArray());
				this.rdi.refresh(criterion);
			} catch (System.Exception ex) {
				string errorMessage = "Search: Error while performing RDI search.";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				throw new ApplicationException(errorMessage);
			}

			if (fieldNames == null)
				fieldNames = this.rdi.getSearchCriteriaFieldNames(iCritID);

			int fieldNamesCount = fieldNames.Length;
			long rowsCount = this.rdi.getDataCount(iCritID);

			if (limit == -1 || (offset + limit) > rowsCount)
				limit = rowsCount - offset;

			long finalRowsCount = offset + limit;
			object[,] arr = new object[limit + 1, fieldNamesCount];

			for (int i=0; i < fieldNamesCount; i++)
				arr[0, i] = fieldNames[i];

			for (long j = offset, n = 1; j < finalRowsCount; j++, n++)
				for (int i = 0; i < fieldNamesCount; i++)
					arr[n, i] = this.rdi.getO(j, fieldNames[i], iCritID);

			Log.Log(LogLevel.Debug, "Search: Result records: " + limit);
			return arr;
		}

		public virtual long AddNew(string alias, string critName, Dictionary<string, object> newData) {
			return this.AddNew(alias, critName, newData, null);
		}

		public virtual long AddNew(string alias, string critName, Dictionary<string, object> newData, IDbConnection dbConnection) {
			string tableName = this.rdi.TableName;
			string tablePrefix = tableName.Substring(0, 4);
			string idFieldName = tablePrefix + "id";
			// Since it's an AddNew operation, no need to pass the ID key if exists, but rather rely on this.rdi.addNew() logic to auto-generate it.
			newData.Remove(idFieldName);

			long critId = this.rdi.getCritID(critName);
			long[] addNewParamsIds = this.rdi.getAddCriteriaParams(critId);
			string[] fieldNames = this.rdi.getAddCriteriaParamNames(critId);
			int paramsCount = fieldNames.Length;
			int elemsCount = this.rdi.elemInfo.Count;
			object[] elemValues = new object[elemsCount];

			for (int i = 0; i < paramsCount; i++) {
				string fieldName = fieldNames[i];
				object newValue;

				if (!newData.TryGetValue(fieldName, out newValue))
					newValue = DBNull.Value;

				long paramElementId = addNewParamsIds[i];
				elemValues[paramElementId] = newValue;
			}

			long id = this.rdi.addNew(critId, elemValues, dbConnection);
			return id;
		}

		public virtual long Modify(string alias, string critName, Dictionary<string, object> newData) {
			return this.Modify(alias, critName, newData, null);
		}

		public virtual long Modify(string alias, string critName, Dictionary<string, object> newData, IDbConnection dbConnection) {
			string tableName = this.rdi.TableName;
			string tablePrefix = tableName.Substring(0, 4);
			string idFieldName = tablePrefix + "id";
			object idFieldValueAsObject;

			if (!newData.TryGetValue(idFieldName, out idFieldValueAsObject))
				throw new ApplicationException(string.Format("Modify: could not extract \"{0}\" from the incoming data", idFieldName));

			// Get the old record, if exists, to use its data for the new historical record.
			long idFieldValue = Convert.ToInt64(idFieldValueAsObject);
			object[] searchOldRecordParams = new object[] {idFieldValue};
			this.rdi.search("id", searchOldRecordParams);
			this.rdi.refresh();

			long critId = this.rdi.getCritID(critName);
			bool isOldRecordFound = this.rdi.getDataCount(critId) > 0;

			if (!isOldRecordFound)
				throw new ApplicationException(string.Format("Modify: could not get the old record with ID \"{0}\" from table \"{1}\"", idFieldValue, tableName));

			long[] addNewParamsIds = this.rdi.getAddCriteriaParams(critId);
			string[] fieldNames = this.rdi.getAddCriteriaParamNames(critId);
			int paramsCount = fieldNames.Length;
			int elemsCount = this.rdi.elemInfo.Count;
			object[] elemValues = new object[elemsCount];

			for (int i = 0; i < paramsCount; i++) {
				string fieldName = fieldNames[i];
				object newValue;

				if (!newData.TryGetValue(fieldName, out newValue)) {
					if (isOldRecordFound)
						newValue = this.rdi.getO(0, fieldName, critId);
					else
						newValue = DBNull.Value;
				}

				long paramElementId = addNewParamsIds[i];
				elemValues[paramElementId] = newValue;
			}

			long id = this.rdi.modify(critId, elemValues, dbConnection);
			return id;
		}

		public virtual long Delete(string alias, string critName, Dictionary<string, object> newData) {
			return this.Delete(alias, critName, newData, null);
		}

		public virtual long Delete(string alias, string critName, Dictionary<string, object> newData, IDbConnection dbConnection) {
			string tableName = this.rdi.TableName;
			string tablePrefix = tableName.Substring(0, 4);
			string recidFieldName = tablePrefix + "recid";
			object recidFieldValueAsObject;

			if (!newData.TryGetValue(recidFieldName, out recidFieldValueAsObject))
				throw new ApplicationException(string.Format("Delete: could not extract \"{0}\" from the incoming data", recidFieldName));

			// Get the old record, if exists, to use its data for the new historical record.
			long recidFieldValue = Convert.ToInt64(recidFieldValueAsObject);

			int recidElementId = (int) this.rdi.elemInfo[recidFieldName].elementID;
			int elemsCount = this.rdi.elemInfo.Count;
			object[] elemValues = new object[elemsCount];
			elemValues[recidElementId] = recidFieldValue;

			long critId = this.rdi.getCritID(critName);
			long id = this.rdi.delete(critId, elemValues, dbConnection);
			return id;
		}

		public string GenerateNextUniqueText(string originalText, ushort? maxResultTextLength) {
			if (originalText == null)
				throw new ArgumentNullException("originalText");

			string resultText = null;
			Random random = new Random();
			string extraValue = random.Next(2, 999999).ToString();

			if (maxResultTextLength.HasValue) {
				int originalTextLength = originalText.Length;
				int extraValueLength = extraValue.Length;

				if (extraValueLength >= maxResultTextLength)
					throw new ApplicationException(string.Format("The length ({0}) of the \"extra value\" (\"{1}\") is equal or greater than the maxResultTextLength ({2})", extraValueLength, extraValue, maxResultTextLength));

				if (originalTextLength + extraValueLength > maxResultTextLength) {
					resultText = originalText.Substring(0, maxResultTextLength.Value - extraValueLength) + extraValue;
				} else
					resultText = originalText + extraValue;
			} else
				resultText = originalText + extraValue;

			return resultText;
		}

		public string GenerateNextUniqueDbFieldValue(string alias, string criterion, string originalText, ushort? maxResultTextLength) {
			if (string.IsNullOrEmpty(criterion))
				throw new ArgumentException("The \"criterion\" param is null or empty", "criterion");

			if (originalText == null)
				throw new ArgumentNullException("originalText");

			string resultText = originalText;
			int duplicatesCount = 0;

			do {
				ArrayList critParameters = new ArrayList {resultText};
				object[,] resultData = this.Search(alias, criterion, critParameters, 0L, -1L, null);
				duplicatesCount = Convert.ToInt32(resultData[1, 0]);
				Log.Log(LogLevel.Debug, "Number of records with the same value: " + duplicatesCount);

				if (duplicatesCount > 0) {
					resultText = this.GenerateNextUniqueText(resultText, maxResultTextLength);
					Log.Log(LogLevel.Debug, "New name candidate: " + resultText);
				}
			} while (duplicatesCount > 0);

			Log.Log(LogLevel.Debug, "Unique name found: " + resultText);
			return resultText;
		}
	}
}
