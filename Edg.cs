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
	public class Edg : RDISplImplRWM {
		private static Logger Log = LogManager.GetCurrentClassLogger();

		public Edg() {
			system.block.item.App_c oApp = new system.block.item.App_c(typeof(system.block.item.App_c), new string[0]);
			this.rdi = (edgeedge_c) edgeedge_c.getInstance(oApp.context);
		}

		private bool ValidateAddNewParams(Dictionary<string, object> newData, out string name, out short typeId) {
			string tableName = this.rdi.TableName;
			string tablePrefix = tableName.Substring(0, 4);
			name = "edge";
			typeId = 9; // General Type.

			try {
				#region Validate name
				string nmFieldName = tablePrefix + "nm";
				object nmFieldValueAsObject;

				if (!newData.TryGetValue(nmFieldName, out nmFieldValueAsObject))
					throw new ApplicationException(string.Format("The \"{0}\" field was not found among the AddNew parameters", nmFieldName));

				if (!(nmFieldValueAsObject is string))
					throw new ApplicationException(string.Format("The \"{0}\" AddNew parameter ({1}) is not of type string", nmFieldName, nmFieldValueAsObject));

				name = (string) nmFieldValueAsObject;

				if (string.IsNullOrEmpty(name) || name.Trim() == string.Empty)
					throw new ApplicationException(string.Format("\"{0}\" AddNew parameter is null or empty", nmFieldName));

				long maxNameLength = 100;

				if (name.Length > maxNameLength)
					throw new ApplicationException(string.Format("The size {0} of \"{1}\" param is beyond the valid one {2)", name.Length, nmFieldName, maxNameLength));
				#endregion

				#region Validate Type ID
				string typeIdFieldName = tablePrefix + "typ_edtpid";
				object typeIdFieldValueAsObject;

				if (newData.TryGetValue(typeIdFieldName, out typeIdFieldValueAsObject)) {
					if (!(typeIdFieldValueAsObject is short))
						throw new ApplicationException(string.Format("The \"{0}\" field is not of type short", typeIdFieldName));

					typeId = (short) typeIdFieldValueAsObject;

					if (typeId < 1)
						throw new ApplicationException(string.Format("The \"{0}\" field has non-positive value = {1}", typeIdFieldName, typeId));
				} else
					throw new ApplicationException(string.Format("The \"{0}\" field was not found among the AddNew parameters", typeIdFieldName));

				#endregion

				#region Validate Provider Node ID
				string provNodeIdFieldName = tablePrefix + "prov_nodeid";
				object provNodeIdFieldValueAsObject;

				if (newData.TryGetValue(provNodeIdFieldName, out provNodeIdFieldValueAsObject)) {
					if (!(provNodeIdFieldValueAsObject is long))
						throw new ApplicationException(string.Format("The \"{0}\" field is not of type long", provNodeIdFieldName));

					if ((long) provNodeIdFieldValueAsObject < 0)
						throw new ApplicationException(string.Format("The \"{0}\" field has negative value = {1}", provNodeIdFieldName, provNodeIdFieldValueAsObject));
				} else
					throw new ApplicationException(string.Format("The \"{0}\" field was not found among the AddNew parameters", provNodeIdFieldName));

				#endregion

				#region Validate Subscriber Node ID
				string subNodeIdFieldName = tablePrefix + "sub_nodeid";
				object subNodeIdFieldValueAsObject;

				if (newData.TryGetValue(subNodeIdFieldName, out subNodeIdFieldValueAsObject)) {
					if (!(subNodeIdFieldValueAsObject is long))
						throw new ApplicationException(string.Format("The \"{0}\" field is not of type long", subNodeIdFieldName));

					if ((long) subNodeIdFieldValueAsObject < 0)
						throw new ApplicationException(string.Format("The \"{0}\" field has negative value = {1}", subNodeIdFieldName, subNodeIdFieldValueAsObject));
				} else
					throw new ApplicationException(string.Format("The \"{0}\" field was not found among the AddNew parameters", subNodeIdFieldName));

				#endregion

				#region Validate Smt
				string smtFieldName = tablePrefix + "smt";
				object smtFieldValueAsObject;

				if (newData.TryGetValue(smtFieldName, out smtFieldValueAsObject)) {
					if (!(smtFieldValueAsObject is float))
						throw new ApplicationException(string.Format("The \"{0}\" field is not of type float", smtFieldName));

					float smtFieldValue = (float) smtFieldValueAsObject;

					if (smtFieldValue < 0.0 || smtFieldValue > 1.0)
						throw new IndexOutOfRangeException(string.Format("The \"{0}\" field has value beyong (0.0, 1.0) range = {1}", smtFieldName, smtFieldValue));
				}
				#endregion

				#region Validate Priv
				string privFieldName = tablePrefix + "priv";
				object privFieldValueAsObject;

				if (newData.TryGetValue(privFieldName, out privFieldValueAsObject)) {
					if (!(privFieldValueAsObject is short))
						throw new ApplicationException(string.Format("The \"{0}\" field is not of type short", privFieldName));

					short privFieldValue = (short) privFieldValueAsObject;

					if (privFieldValue < 1 || privFieldValue > 3)
						throw new IndexOutOfRangeException(string.Format("The \"{0}\" field has value beyong (1, 3) range = {1}", privFieldName, privFieldValue));
				} else {
					switch (typeId) {
						case 1: // Friend.
						case 2: // Follower.
							throw new ApplicationException(string.Format("The \"{0}\" field was not found among the AddNew parameters", privFieldName));
					}
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
			Dictionary<string, object> newEdgeData = new Dictionary<string, object>(newData.Count);

			foreach (KeyValuePair<string, object> dictEntry in newData) {
				string dictKey = dictEntry.Key;

				if (dictKey.StartsWith(tablePrefix))
					newEdgeData.Add(dictKey, dictEntry.Value);
			}

			string nmFieldName = tablePrefix + "nm";
			string nmFieldValue;
			short typeId;

			if (!ValidateAddNewParams(newEdgeData, out nmFieldValue, out typeId)) {
				string errorMessage = "Error validating AddNew params";
				Log.Log(LogLevel.Error, errorMessage);
				throw new ApplicationException(errorMessage);
			}

			newEdgeData[tablePrefix + "active"] = true;

			// For now set "priv" only for Friends/Followers.
			switch (typeId) {
				case 1: // Friend.
				case 2: // Follower.
					break;
				default:
					newEdgeData.Remove(tablePrefix + "priv");
					break;
			}

			nmFieldValue = this.GenerateNextUniqueDbFieldValue(alias, "countbynm", nmFieldValue, 100);
			newEdgeData[nmFieldName] = nmFieldValue;

			long idFieldValue = base.AddNew(alias, critName, newEdgeData, dbConnection);
			return idFieldValue;
		}

		// FIXME: hack to provide types per table type - change to lookup db (make sure types match in db before reading from it)
		public static short GetTypId( string styp){
			short typid = 0;
			switch(styp){
			case "statusesCount":
				typid = 10; break;
			case "favoritesCount":
				typid = 12; break;
			case "friendsCount":
				typid = 13; break;
			case "followersCount":
				typid = 11; break;
			case "listedCount":
				typid = 14;	break;

				default:
				throw new Exception("Node GetTypId: unknown 'styp': " + styp);
				break;
			}
			return typid;
		}
	}
}
