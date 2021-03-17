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
	public class EntInfo : RDISplImplRWM {
		private static Logger Log = LogManager.GetCurrentClassLogger();

		public EntInfo() {
			system.block.item.App_c oApp = new system.block.item.App_c(typeof(system.block.item.App_c), new string[0]);
			this.rdi = (eninentityinfo_c) eninentityinfo_c.getInstance(oApp.context);
		}

		private bool ValidateAddNewParams(Dictionary<string, object> newData) {
			string tableName = this.rdi.TableName;
			string tablePrefix = tableName.Substring(0, 4);

			try {
				#region Validate Type ID
				string typeIdFieldName = tablePrefix + "infotype_intpid";
				object typeIdFieldValueAsObject;

				if (newData.TryGetValue(typeIdFieldName, out typeIdFieldValueAsObject)) {
					if (!(typeIdFieldValueAsObject is short))
						throw new ApplicationException(string.Format("The \"{0}\" field is not of type short", typeIdFieldName));

					short typeId = (short) typeIdFieldValueAsObject;

					if (typeId < 1)
						throw new ApplicationException(string.Format("The \"{0}\" field has non-positive value = {1}", typeIdFieldName, typeId));
				} else
					throw new ApplicationException(string.Format("The \"{0}\" field was not found among the AddNew parameters", typeIdFieldName));
				#endregion

				#region Validate Entity ID
				string entityIdFieldName = tablePrefix + "entity_enttid";
				object entityIdFieldValueAsObject;

				if (newData.TryGetValue(entityIdFieldName, out entityIdFieldValueAsObject)) {
					if (!(entityIdFieldValueAsObject is long))
						throw new ApplicationException(string.Format("The \"{0}\" field is not of type long", entityIdFieldName));

					if ((long) entityIdFieldValueAsObject < 0)
						throw new ApplicationException(string.Format("The \"{0}\" field has negative value = {1}", entityIdFieldName, entityIdFieldValueAsObject));
				} else
					throw new ApplicationException(string.Format("The \"{0}\" field was not found among the AddNew parameters", entityIdFieldName));
				#endregion

				#region Validate Value
				string valFieldName = tablePrefix + "val";
				object valFieldValueAsObject;

				if (newData.TryGetValue(valFieldName, out valFieldValueAsObject)) {
					if (!(valFieldValueAsObject is string))
						throw new ApplicationException(string.Format("The \"{0}\" field is not of type string", valFieldName));

					if (string.IsNullOrEmpty((string) valFieldValueAsObject))
						throw new ApplicationException(string.Format("\"{0}\" AddNew parameter is null or empty", valFieldName));
				} else
					throw new ApplicationException(string.Format("The \"{0}\" field was not found among the AddNew parameters", valFieldName));
				#endregion

				return true;
			} catch (Exception ex) {
				string errorMessage = "Error validating AddNew params";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				return false;
			}
		}

		public override long AddNew(string alias, string critName, Dictionary<string, object> newData) {
			return this.AddNew(alias, critName, newData, null);
		}

		public override long AddNew(string alias, string critName, Dictionary<string, object> newData, IDbConnection dbConnection) {
			string tableName = this.rdi.TableName;
			string tablePrefix = tableName.Substring(0, 4);
			Dictionary<string, object> newEntityInfoData = new Dictionary<string, object>(newData.Count);

			foreach (KeyValuePair<string, object> dictEntry in newData) {
				string dictKey = dictEntry.Key;

				if (dictKey.StartsWith(tablePrefix))
					newEntityInfoData.Add(dictKey, dictEntry.Value);
			}

			if (!ValidateAddNewParams(newEntityInfoData)) {
				string errorMessage = "Error validating AddNew params";
				Log.Log(LogLevel.Error, errorMessage);
				throw new ApplicationException(errorMessage);
			}

			newEntityInfoData[tablePrefix + "active"] = true;

			long idFieldValue = base.AddNew(alias, critName, newEntityInfoData, dbConnection);
			return idFieldValue;
		}
	}
}
