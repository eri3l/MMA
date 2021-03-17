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
	public class Geo : RDISplImplRWM {
		private static Logger Log = LogManager.GetCurrentClassLogger();

		public Geo() {
			system.block.item.App_c oApp = new system.block.item.App_c(typeof(system.block.item.App_c), new string[0]);
			this.rdi = (geolgeolocation_c) geolgeolocation_c.getInstance(oApp.context);
		}

		private bool ValidateAddNewParams(Dictionary<string, object> newData, out long? nodeId) {
			string tableName = this.rdi.TableName;
			string tablePrefix = tableName.Substring(0, 4);
			nodeId = null;

			try {
				#region Validate Name
				string nmFieldName = tablePrefix + "nm";
				object nmFieldValueAsObject;

				if (!newData.TryGetValue(nmFieldName, out nmFieldValueAsObject))
					throw new ApplicationException(string.Format("The \"{0}\" field was not found among the AddNew parameters", nmFieldName));

				if (!(nmFieldValueAsObject is string))
					throw new ApplicationException(string.Format("The \"{0}\" AddNew parameter ({1}) is not of type string", nmFieldName, nmFieldValueAsObject));

				string name = (string) nmFieldValueAsObject;

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

				#region Validate Type ID
				string typeIdFieldName = tablePrefix + "typ_gltpid";
				object typeIdFieldValueAsObject;

				if (newData.TryGetValue(typeIdFieldName, out typeIdFieldValueAsObject)) {
					if (!(typeIdFieldValueAsObject is short))
						throw new ApplicationException(string.Format("The \"{0}\" field is not of type short", typeIdFieldName));

					short typeId = (short) typeIdFieldValueAsObject;

					if (typeId < 0)
						throw new ApplicationException(string.Format("The \"{0}\" field has non-positive value = {1}", typeIdFieldName, typeId));
				} else
					throw new ApplicationException(string.Format("The \"{0}\" field was not found among the AddNew parameters", typeIdFieldName));
				#endregion

				#region Validate Bounding Box
				string bFieldName = tablePrefix + "b";
				object bFieldValueAsObject;

				if (!newData.TryGetValue(bFieldName, out bFieldValueAsObject))
					throw new ApplicationException(string.Format("The \"{0}\" field was not found among the AddNew parameters", bFieldName));

				if (!(bFieldValueAsObject is string))
					throw new ApplicationException(string.Format("The \"{0}\" AddNew parameter ({1}) is not of type string", bFieldName, bFieldValueAsObject));

				string b = (string) bFieldValueAsObject;

				if (string.IsNullOrEmpty(b) || b.Trim() == string.Empty)
					throw new ApplicationException(string.Format("\"{0}\" AddNew parameter is null or empty", bFieldName));

				byte maxBoundingBoxLength = 60;

				if (b.Length > maxBoundingBoxLength)
					throw new ApplicationException(string.Format("The size {0} of \"{1}\" param is beyond the valid one {2)", b.Length, bFieldName, maxBoundingBoxLength));
				#endregion

				#region Validate X
				string xFieldName = tablePrefix + "x";
				object xFieldValueAsObject;

				if (newData.TryGetValue(xFieldName, out xFieldValueAsObject)) {
					if (!(xFieldValueAsObject is float))
						throw new ApplicationException(string.Format("The \"{0}\" field is not of type float", xFieldName));

					float x = (float) xFieldValueAsObject;

					if (x < -180.0 && x > 180.0)
						throw new ApplicationException(string.Format("\"{0}\" AddNew parameter ({0}) is beyong (-180.0, 180.0) range", xFieldName, x));
				} else
					throw new ApplicationException(string.Format("The \"{0}\" field was not found among the AddNew parameters", xFieldName));
				#endregion

				#region Validate Y
				string yFieldName = tablePrefix + "y";
				object yFieldValueAsObject;

				if (newData.TryGetValue(yFieldName, out yFieldValueAsObject)) {
					if (!(yFieldValueAsObject is float))
						throw new ApplicationException(string.Format("The \"{0}\" field is not of type float", yFieldName));

					float y = (float) yFieldValueAsObject;

					if (y < -90.0 && y > 90.0)
						throw new ApplicationException(string.Format("\"{0}\" AddNew parameter ({0}) is beyong (-90.0, 90.0) range", yFieldName, y));
				} else
					throw new ApplicationException(string.Format("The \"{0}\" field was not found among the AddNew parameters", yFieldName));
				#endregion

				#region Validate R
				string rFieldName = tablePrefix + "r";
				object rFieldValueAsObject;

				if (newData.TryGetValue(rFieldName, out rFieldValueAsObject)) {
					if (!(rFieldValueAsObject is float))
						throw new ApplicationException(string.Format("The \"{0}\" field is not of type float", rFieldName));

					float r = (float) rFieldValueAsObject;

					if (r < 0.0)
						throw new ApplicationException(string.Format("\"{0}\" AddNew parameter ({0}) is negative number", rFieldName, r));
				} else
					throw new ApplicationException(string.Format("The \"{0}\" field was not found among the AddNew parameters", rFieldName));
				#endregion

				#region Validate City
				string cityFieldName = tablePrefix + "city";
				object cityFieldValueAsObject;

				if (newData.TryGetValue(cityFieldName, out cityFieldValueAsObject)) {
					if (!(cityFieldValueAsObject is string))
						throw new ApplicationException(string.Format("The \"{0}\" field is not of type string", cityFieldName));
				}
				#endregion

				#region Validate Country
				string countryFieldName = tablePrefix + "cntry";
				object countryFieldValueAsObject;

				if (newData.TryGetValue(countryFieldName, out countryFieldValueAsObject)) {
					if (!(countryFieldValueAsObject is string))
						throw new ApplicationException(string.Format("The \"{0}\" field is not of type string", countryFieldName));

					if (string.IsNullOrEmpty((string) countryFieldValueAsObject))
						throw new ApplicationException(string.Format("\"{0}\" AddNew parameter is null or empty", countryFieldName));
				} else
					throw new ApplicationException(string.Format("The \"{0}\" field was not found among the AddNew parameters", countryFieldName));
				#endregion

				#region Validate Zip
				string zipFieldName = tablePrefix + "zip";
				object zipFieldValueAsObject;

				if (newData.TryGetValue(zipFieldName, out zipFieldValueAsObject)) {
					if (!(zipFieldValueAsObject is string))
						throw new ApplicationException(string.Format("The \"{0}\" field is not of type string", zipFieldName));
				}
				#endregion

				#region Validate Time Zone
				string tzFieldName = tablePrefix + "tz";
				object tzFieldValueAsObject;

				if (newData.TryGetValue(tzFieldName, out tzFieldValueAsObject)) {
					if (!(tzFieldValueAsObject is string))
						throw new ApplicationException(string.Format("The \"{0}\" field is not of type string", tzFieldName));
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
			if (string.IsNullOrEmpty (alias))
				alias = "";
			string tableName = this.rdi.TableName;
			string tablePrefix = tableName.Substring(0, 4);
			Dictionary<string, object> newNodeData = new Dictionary<string, object>(newData.Count);
			Dictionary<string, object> newGeoData = new Dictionary<string, object>(newData.Count);

			foreach (KeyValuePair<string, object> dictEntry in newData) {
				string dictKey = dictEntry.Key;

				if (dictKey.StartsWith(tablePrefix))
					newGeoData.Add(dictKey, dictEntry.Value);
				else if (dictKey.StartsWith("node"))
					newNodeData.Add(dictKey, dictEntry.Value);
			}

			long? nodeIdFieldValue;

			if (!ValidateAddNewParams(newGeoData, out nodeIdFieldValue)) {
				string errorMessage = "Error validating AddNew params";
				Log.Log(LogLevel.Error, errorMessage);
				throw new ApplicationException(errorMessage);
			}

			newGeoData[tablePrefix + "e"] = true;
			newGeoData[tablePrefix + "z"] = (float) 0.0;
			newGeoData[tablePrefix + "active"] = true;

			// TODO: use transaction.
			if (!nodeIdFieldValue.HasValue) {
				Nd node = new Nd();
				newNodeData["nodetyp_ndtpid"] = (short) 3; // NoteType.Geo
				nodeIdFieldValue = node.AddNew(alias, "default", newNodeData, dbConnection);
				newGeoData[tablePrefix + "node_nodeid"] = nodeIdFieldValue;
			}

			long geoIdFieldValue = base.AddNew(alias, critName, newGeoData, dbConnection);
			return geoIdFieldValue;
		}

		public RetCde all_countries(object[] io) {
			string alias, criterion;
			ArrayList parameters;
			long offset, limit;
			string[] fieldNames;
			string errorMessage;

			try {
				alias = (string) io[0];
				criterion = (string) io[1];
				parameters = (ArrayList) io[2];
				offset = io[3] is long ? (long) io[3] : 0L;
				limit = io[4] is long ? (long) io[4] : -1L;
				fieldNames = (string[]) io[5];
			} catch (Exception ex) {
				errorMessage = "all_countries: error validating input params";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				io[io.Length - 1] = new object[,] {{"geolid", "geolnm"}};
				return RetCde.GeneralError;
			}

			object[,] resultArray;

			try {
				resultArray = this.AllCounties(alias, criterion, parameters, offset, limit, fieldNames);
			} catch (Exception ex) {
				errorMessage = "all_countries: error loading message history";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				io[io.Length - 1] = new object[,] {{"geolid", "geolnm"}};
				return RetCde.GeneralError;
			}

			io[io.Length - 1] = resultArray;
			return RetCde.Success;
		}

		internal object[,] AllCounties(string alias, string criterion, ArrayList parameters, long offset, long limit, string[] fieldNames) {
			byte requiredParamsCount = 0;
			string errorMessage;

			if (parameters != null && parameters.Count != requiredParamsCount) {
				errorMessage = string.Format("AllCounties: parameters array must contain {0} items while the number of its items is {1}", requiredParamsCount, parameters.Count);
				Log.Log(LogLevel.Error, errorMessage);
				throw new ApplicationException(errorMessage);
			}
			string searchCriterion = "allcountries";
			ArrayList critParameters = new ArrayList();
			object[,] resultData;

			try {
				resultData = this.Search(alias, searchCriterion, critParameters, 0L, -1L, null);
			} catch (Exception ex) {
				errorMessage = "AllCounties: execution of \"" + criterion +  "\" criterion returned error.";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				throw new ApplicationException(errorMessage);
			}

			if (resultData == null) {
				errorMessage = "AllCounties: execution of \"" + criterion +  "\" criterion returned null data array";
				Log.Log(LogLevel.Error, errorMessage);
				throw new ApplicationException(errorMessage);
			}

			Log.Log(LogLevel.Debug, "AllCounties: Number of data records returned: " + (resultData.GetLength(0) - 1));
			return resultData;

		}

		// FIXME: hack to provide types per table type - should be checked from actual db table typeid
		public static short GetTypId( string styp){
			short typid = 0;
			switch(styp){
				case "unknown":
				typid = 0; break;

				case "country":
				typid = 1; break;

				case "state":
				typid = 2; break;

				case "region":
				typid = 3; break;

				case "town":
				typid = 4; break;

				case "point":
				typid = 5; break;

				case "feature": // eg. river
				typid = 6; break;

				default:
				Log.Log(LogLevel.Debug, "Node GetTypId: unknown 'styp': " + styp); 
				break;

			}
			return typid;
		}

	}
}
