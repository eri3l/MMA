using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Data;
using System.Text;
using app_c.mma.rdi;
using system.block.item;
using NLog;

namespace app.mma {
	public class Ent : RDISplImplRWM {
		private static Logger Log = LogManager.GetCurrentClassLogger();

		public Ent() {
			system.block.item.App_c oApp = new system.block.item.App_c(typeof(system.block.item.App_c), new string[0]);
			this.rdi = (enttentity_c) enttentity_c.getInstance(oApp.context);
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

				#region Validate Screen Name
				string snmFieldName = tablePrefix + "snm";
				object snmFieldValueAsObject;

				if (!newData.TryGetValue(snmFieldName, out snmFieldValueAsObject))
					throw new ApplicationException(string.Format("The \"{0}\" field was not found among the AddNew parameters", snmFieldName));

				if (!(snmFieldValueAsObject is string))
					throw new ApplicationException(string.Format("The \"{0}\" AddNew parameter ({1}) is not of type string", snmFieldName, snmFieldValueAsObject));

				string sname = (string) snmFieldValueAsObject;

				if (string.IsNullOrEmpty(sname) || sname.Trim() == string.Empty)
					throw new ApplicationException(string.Format("\"{0}\" AddNew parameter is null or empty", snmFieldName));

				byte maxSNameLength = 100;

				if (sname.Length > maxSNameLength)
					throw new ApplicationException(string.Format("The size {0} of \"{1}\" param is beyond the valid one {2)", sname.Length, snmFieldName, maxSNameLength));
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

				#region Validate gender
				string genderFieldName = tablePrefix + "gender";
				object genderFieldValueAsObject;

				if (newData.TryGetValue(genderFieldName, out genderFieldValueAsObject)) {
					if (genderFieldValueAsObject != null) {
						if (!(genderFieldValueAsObject is char))
							throw new ApplicationException(string.Format("The \"{0}\" AddNew parameter ({1}) is not of type char", genderFieldName, genderFieldValueAsObject));

						char gender = (char) genderFieldValueAsObject;

						switch (gender) {
							case 'm':
							case 'f':
							case 'o':
								break;
							default:
								throw new ApplicationException(string.Format("Invalid value ({0}) for \"{1}\" AddNew parameter", gender, genderFieldName));
						}
					} else
						throw new ApplicationException(string.Format("\"{0}\" AddNew parameter is null", genderFieldName));
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
			Dictionary<string, object> newEntityData = new Dictionary<string, object>(newData.Count);

			foreach (KeyValuePair<string, object> dictEntry in newData) {
				string dictKey = dictEntry.Key;

				if (dictKey.StartsWith(tablePrefix))
					newEntityData.Add(dictKey, dictEntry.Value);
				else if (dictKey.StartsWith("node"))
					newNodeData.Add(dictKey, dictEntry.Value);
			}

			string nmFieldName = tablePrefix + "nm";
			string nmFieldValue;
			long? nodeIdFieldValue;

			if (!ValidateAddNewParams(newEntityData, out nmFieldValue, out nodeIdFieldValue)) {
				string errorMessage = "Error validating AddNew params";
				Log.Log(LogLevel.Error, errorMessage);
				throw new ApplicationException(errorMessage);
			}

			newEntityData[tablePrefix + "active"] = true;
			string langIdFieldName = tablePrefix + "lang_langid";

			if (!newEntityData.ContainsKey(langIdFieldName))
				newEntityData[langIdFieldName] = 1L;

			nmFieldValue = this.GenerateNextUniqueDbFieldValue(alias, "countbynm", nmFieldValue, 100);
			newEntityData[nmFieldName] = nmFieldValue;

			string[] fieldNamesSuffixToClear = new string[] {
				"enttsmt",
				"enttkeywords"
				// ,"enttpic_docuid"
			};

			foreach (string fieldNameSuffix in fieldNamesSuffixToClear)
				newNodeData.Remove(tablePrefix + fieldNameSuffix);

			// TODO: use transaction.
			// TODO: Doc.addNew()

			if (!nodeIdFieldValue.HasValue) {
				Nd node = new Nd();
				newNodeData["nodetyp_ndtpid"] = (short) 1; // NoteType.Entity
				nodeIdFieldValue = node.AddNew(alias, "default", newNodeData, dbConnection);
				newEntityData[tablePrefix + "node_nodeid"] = nodeIdFieldValue;
			}

			long acctIdFieldValue = base.AddNew(alias, critName, newEntityData, dbConnection);
			return acctIdFieldValue;
		}
	}
}
