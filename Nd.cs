using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Text;
using app_c.mma.rdi;
using app.mma.utils;
using system.block.item;
using NLog;

namespace app.mma {
	public class Nd : RDISplImplRWM {
		private static Logger Log = LogManager.GetCurrentClassLogger();

		public Nd() {
			system.block.item.App_c oApp = new system.block.item.App_c(typeof(system.block.item.App_c), new string[0]);
			this.rdi = (nodenode_c) nodenode_c.getInstance(oApp.context);
		}

		private bool ValidateAddNewParams(Dictionary<string, object> newData, out string name, out DateTime? birthDate) {
			string tableName = this.rdi.TableName;
			string tablePrefix = tableName.Substring(0, 4);
			name = "node";
			birthDate = null;

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

				#region Validate Node Type
				string nodeTypeFieldName = tablePrefix + "typ_ndtpid";
				object nodeTypeFieldValueAsObject;

				if (newData.TryGetValue(nodeTypeFieldName, out nodeTypeFieldValueAsObject) && !(nodeTypeFieldValueAsObject is short))
					throw new ApplicationException(string.Format("The \"{0}\" AddNew parameter ({1}) is not of type short", nodeTypeFieldName, nodeTypeFieldValueAsObject));

				if ((short) nodeTypeFieldValueAsObject < 0)
					throw new ApplicationException(string.Format("The \"{0}\" AddNew parameter ({1}) has negative value", nodeTypeFieldName, nodeTypeFieldValueAsObject));
				#endregion

				#region Validate Birth Date
				string birthDateFieldName = tablePrefix + "birthd";
				object birthDateAsObject;

				if (newData.TryGetValue(birthDateFieldName, out birthDateAsObject)) {
					if (birthDateAsObject is DateTime)
						birthDate = (DateTime) birthDateAsObject;
					else
						throw new ApplicationException(string.Format("The \"{0}\" AddNew parameter ({1}) is not of type short", birthDateFieldName, birthDateAsObject));
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

			Console.Write ("DEBUG: In Nd AddNew");

			return this.AddNew(alias, critName, newData, null);
		}

		public override long AddNew(string alias, string critName, Dictionary<string, object> newData, IDbConnection dbConnection) {

			string tableName = this.rdi.TableName;
			string tablePrefix = tableName.Substring(0, 4);
			string errorMessage;

			Dictionary<string, object> newNodeData = new Dictionary<string, object>(newData.Count);

			foreach (KeyValuePair<string, object> dictEntry in newData) {
				string dictKey = dictEntry.Key;

				if (dictKey.StartsWith(tablePrefix))
					newNodeData.Add(dictKey, dictEntry.Value);
			}

			#region Params Validation
			string nmFieldName = tablePrefix + "nm";
			string nmFieldValue;
			DateTime? birthDateFieldValue;

			if (!ValidateAddNewParams(newNodeData, out nmFieldValue, out birthDateFieldValue)) {
				errorMessage = "Error validating AddNew params";
				Log.Log(LogLevel.Error, errorMessage);
				throw new ApplicationException(errorMessage);
			}

			if (birthDateFieldValue.HasValue) {
				long birthDateNumValue = DateTimeUtils.ConvertToUnixTime(birthDateFieldValue.Value);
				newNodeData[tablePrefix + "birthdnum"] = birthDateNumValue;
			} else
				newNodeData.Remove(tablePrefix + "birthdnum");
			#endregion

			DateTime utcNow = DateTime.UtcNow;
			newNodeData[tablePrefix + "delta"] = newNodeData[tablePrefix + "delta0"] = utcNow;
			newNodeData[tablePrefix + "active"] = true;

			string[] fieldNamesSuffixToClear = new string[] {
				"vld",
				"rpn",
				"dat",
				"geo_geolid",
				"subs",
				"subtyp",
				"prvs",
				"prvtyp"
			};

			foreach (string fieldNameSuffix in fieldNamesSuffixToClear)
				newNodeData.Remove(tablePrefix + fieldNameSuffix);

			nmFieldValue = this.GenerateNextUniqueDbFieldValue(alias, "countbynm", nmFieldValue, 100);
			newNodeData[nmFieldName] = nmFieldValue;
			long idFieldValue = base.AddNew(alias, critName, newNodeData, dbConnection);
			return idFieldValue;
		}

		// FIXME: hack to provide types per table type - change to lookup db (make sure types match in db before reading from it)
		public static short GetTypId( string styp){
			short typid = 0;
			switch(styp){
			case "geo":
				typid = 3; break;
			case "doc":
				typid = 2; break;
			case "ent":
				typid = 1; break;
			case "usr":
				typid = 6; break;
			case "acc":
				typid = 4; break;
			case "chnl":
				typid = 7; break;
			case "src":
				typid = 5; break;
			default:
				throw new Exception("Node GetTypId: unknown 'styp': " + styp); // TODO doesnt print out
				break;
			}
			return typid;
		}
	}
}
