using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Globalization;
using System.Data;
using System.Text;
using app_c.mma.rdi;
using system.block.item;
using NLog;

namespace app.mma {
	public class Usr : RDISplImplRWM {
		private static Logger Log = LogManager.GetCurrentClassLogger();

		public Usr() {
			system.block.item.App_c oApp = new system.block.item.App_c(typeof(system.block.item.App_c), new string[0]);
			this.rdi = (useruser_c) useruser_c.getInstance(oApp.context);
		}

		private bool ValidateAddNewParams(Dictionary<string, object> newData, out string name, out long? nodeId) {
			string tableName = this.rdi.TableName;
			string tablePrefix = tableName.Substring(0, 4);
			name = "node";
			nodeId = null;

			try {
				#region Validate Name
				string nmFieldName = tablePrefix + "nm";
				object nmFieldValueAsObject;

				if (!newData.TryGetValue(nmFieldName, out nmFieldValueAsObject))
					throw new ApplicationException(string.Format("The \"{0}\" field was not found among the AddNew parameters", nmFieldName));

				if (!(nmFieldValueAsObject is string))
					throw new ApplicationException(string.Format("The \"{0}\" AddNew parameter ({1}) is not of type string", nmFieldName, nmFieldValueAsObject));

				name = (string) nmFieldValueAsObject;

				if (string.IsNullOrEmpty(name) || name.Trim() == string.Empty)
					throw new ApplicationException(string.Format("\"{0}\" AddNew parameter is null or empty", nmFieldName));

				byte maxNameLength = 100;

				if (name.Length > maxNameLength)
					throw new ApplicationException(string.Format("The size {0} of \"{1}\" param is beyond the valid one {2)", name.Length, nmFieldName, maxNameLength));
				#endregion

				#region Validate Node ID
				string nodeIdFieldName = tablePrefix + "node_nodeid";
				object nodeIdFieldValueAsObject;

				if (newData.TryGetValue(nodeIdFieldName, out nodeIdFieldValueAsObject)) {
					if (!(nodeIdFieldValueAsObject is long))
						throw new ApplicationException(string.Format("The \"{0}\" field is not of type long", nodeIdFieldName));

					if ((long) nodeIdFieldValueAsObject < 0)
						throw new ApplicationException(string.Format("The \"{0}\" field has negative value = {1}", nodeIdFieldName, nodeIdFieldValueAsObject));

					nodeId = (long) nodeIdFieldValueAsObject;
				} else
					nodeId = null;
				#endregion

				#region Validate Account ID
				string accountIdFieldName = tablePrefix + "account_acctid";
				object accountIdFieldValueAsObject;

				if (!newData.TryGetValue(accountIdFieldName, out accountIdFieldValueAsObject))
					throw new ApplicationException(string.Format("The \"{0}\" field was not found among the AddNew parameters", accountIdFieldName));

				if (!(accountIdFieldValueAsObject is long))
					throw new ApplicationException(string.Format("The \"{0}\" AddNew parameter ({1}) is not of type long", accountIdFieldName, accountIdFieldValueAsObject));

				if ((long) accountIdFieldValueAsObject < 0)
					throw new ApplicationException(string.Format("The \"{0}\" AddNew parameter ({1}) has negative value", accountIdFieldName, accountIdFieldValueAsObject));
				#endregion

				#region Validate Prefs
				string prefsFieldName = tablePrefix + "prefs";
				object prefsFieldValueAsObject;

				if (newData.TryGetValue(prefsFieldName, out prefsFieldValueAsObject) && !(prefsFieldValueAsObject is string))
					throw new ApplicationException(string.Format("The \"{0}\" AddNew parameter ({1}) is not of type string", prefsFieldName, prefsFieldValueAsObject));
				#endregion

				#region Validate State
				string stateFieldName = tablePrefix + "state";
				object stateFieldValueAsObject;

				if (newData.TryGetValue(stateFieldName, out stateFieldValueAsObject)) {
					if (!(stateFieldValueAsObject is int))
						throw new ApplicationException(string.Format("The \"{0}\" AddNew parameter ({1}) is not of type int", stateFieldName, stateFieldValueAsObject));

					if (((int) stateFieldValueAsObject) < 0)
						throw new ApplicationException(string.Format("The \"{0}\" AddNew parameter ({1}) has negative value", stateFieldName, stateFieldValueAsObject));
				}
				#endregion

				#region Validate Day Limit
				string dayLimitFieldName = tablePrefix + "daylimit";
				object dayLimitFieldValueAsObject;

				if (newData.TryGetValue(dayLimitFieldName, out dayLimitFieldValueAsObject)) {
					if (!(dayLimitFieldValueAsObject is short))
						throw new ApplicationException(string.Format("The \"{0}\" AddNew parameter ({1}) is not of type short", dayLimitFieldName, dayLimitFieldValueAsObject));

					if ((short) dayLimitFieldValueAsObject < 0)
						throw new ApplicationException(string.Format("The \"{0}\" AddNew parameter ({1}) has negative value", dayLimitFieldName, dayLimitFieldValueAsObject));
				}
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
			Dictionary<string, object> newNodeData = new Dictionary<string, object>(newData.Count);
			Dictionary<string, object> newUserData = new Dictionary<string, object>(newData.Count);

			foreach (KeyValuePair<string, object> dictEntry in newData) {
				string dictKey = dictEntry.Key;

				if (dictKey.StartsWith(tablePrefix))
					newUserData.Add(dictKey, dictEntry.Value);
				else if (dictKey.StartsWith("node"))
					newNodeData.Add(dictKey, dictEntry.Value);
			}

			string nmFieldName = tablePrefix + "nm";
			string nmFieldValue;
			long? nodeIdFieldValue;

			if (!ValidateAddNewParams(newUserData, out nmFieldValue, out nodeIdFieldValue)) {
				string errorMessage = "Error validating AddNew params";
				Log.Log(LogLevel.Error, errorMessage);
				throw new ApplicationException(errorMessage);
			}

			newUserData[tablePrefix + "active"] = true;

			nmFieldValue = this.GenerateNextUniqueDbFieldValue(alias, "countbynm", nmFieldValue, 100);
			newUserData[nmFieldName] = nmFieldValue;

			// TODO: use transaction.
			if (!nodeIdFieldValue.HasValue) {
				Nd node = new Nd();
				newNodeData["nodetyp_ndtpid"] = (short) 6; // NoteType.User
				nodeIdFieldValue = node.AddNew(alias, "default", newNodeData, dbConnection);
				newUserData[tablePrefix + "node_nodeid"] = nodeIdFieldValue;
			}

			long acctIdFieldValue = base.AddNew(alias, critName, newUserData, dbConnection);
			return acctIdFieldValue;
		}
	}
}
