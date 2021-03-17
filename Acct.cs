using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Data;
using System.Text;
using app_c.mma.rdi;
using system.block.item;
using app.mma.utils;
using app.pwdhasher;
using NLog;

namespace app.mma {
	public class Acct : RDISplImplRWM {
		private static Logger Log = LogManager.GetCurrentClassLogger();

		public Acct() {
			system.block.item.App_c oApp = new system.block.item.App_c(typeof(system.block.item.App_c), new string[0]);
			this.rdi = (acctaccount_c) acctaccount_c.getInstance(oApp.context);
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

				#region Validate sname
				string snmFieldName = tablePrefix + "snm";
				object snmFieldValueAsObject;

				if (!newData.TryGetValue(snmFieldName, out snmFieldValueAsObject))
					throw new ApplicationException(string.Format("The \"{0}\" field was not found among the AddNew parameters", snmFieldName));

				if (!(snmFieldValueAsObject is string))
					throw new ApplicationException(string.Format("The \"{0}\" AddNew parameter ({1}) is not of type string", snmFieldName, snmFieldValueAsObject));

				string sname = (string) snmFieldValueAsObject;

				if (string.IsNullOrEmpty(sname) || sname.Trim() == string.Empty)
					throw new ApplicationException(string.Format("\"{0}\" AddNew parameter is null or empty", snmFieldName));

				long maxSNameLength = 100;

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

				#region Validate Source ID
				string sourceIdFieldName = tablePrefix + "source_srceid";
				object sourceIdFieldValueAsObject;
				long sourceId;

				if (newData.TryGetValue(sourceIdFieldName, out sourceIdFieldValueAsObject)) {
					if (!(sourceIdFieldValueAsObject is long))
						throw new ApplicationException(string.Format("The \"{0}\" field is not of type long", sourceIdFieldName));

					sourceId = (long) sourceIdFieldValueAsObject;

					switch (sourceId) {
						case 1L: // MMA
						case 3L: // Twitter
							break;
						default:
							throw new ApplicationException(string.Format("The \"{0}\" field has invalid value = {1}", sourceIdFieldName, sourceId));
					}
						
				} else
					throw new ApplicationException(string.Format("The \"{0}\" field was not found among the AddNew parameters", sourceIdFieldName));

				#endregion

				#region Validate Foreign ID
				string foreignIdFieldName = tablePrefix + "foreignid";
				object foreignIdFieldValueAsObject;

				if (newData.TryGetValue(foreignIdFieldName, out foreignIdFieldValueAsObject)) {
					if (!(foreignIdFieldValueAsObject is string))
						throw new ApplicationException(string.Format("The \"{0}\" field is not of type string", foreignIdFieldName));

					if (string.IsNullOrEmpty((string) foreignIdFieldValueAsObject))
						throw new ApplicationException(string.Format("\"{0}\" AddNew parameter is null or empty", foreignIdFieldName));
				}
				#endregion

				#region Validate Secret
				string secretFieldName = tablePrefix + "secret";
				object secretFieldValueAsObject;

				if (newData.TryGetValue(secretFieldName, out secretFieldValueAsObject)) {
					if (!(secretFieldValueAsObject is string))
						throw new ApplicationException(string.Format("The \"{0}\" field is not of type string", secretFieldName));

					if (string.IsNullOrEmpty((string) secretFieldValueAsObject))
						throw new ApplicationException(string.Format("\"{0}\" AddNew parameter is null or empty", secretFieldName));
				} else
					throw new ApplicationException(string.Format("The \"{0}\" field was not found among the AddNew parameters", secretFieldName));
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

				if (sourceId > 1L) {
					#region Validate Auth
					string authFieldName = tablePrefix + "auth";
					object authFieldValueAsObject;

					if (newData.TryGetValue(authFieldName, out authFieldValueAsObject)) {
						if (!(authFieldValueAsObject is string))
							throw new ApplicationException(string.Format("The \"{0}\" field is not of type string", authFieldName));

						if (string.IsNullOrEmpty((string) authFieldValueAsObject))
							throw new ApplicationException(string.Format("\"{0}\" AddNew parameter is null or empty", authFieldName));
					} else
						throw new ApplicationException(string.Format("The \"{0}\" field was not found among the AddNew parameters", authFieldName));
					#endregion
				}
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
			Dictionary<string, object> newAccountData = new Dictionary<string, object>(newData.Count);

			foreach (KeyValuePair<string, object> dictEntry in newData) {
				string dictKey = dictEntry.Key;

				if (dictKey.StartsWith(tablePrefix))
					newAccountData.Add(dictKey, dictEntry.Value);
				else if (dictKey.StartsWith("node"))
					newNodeData.Add(dictKey, dictEntry.Value);
			}

			string nmFieldName = tablePrefix + "nm";
			string nmFieldValue;
			long? nodeIdFieldValue;

			if (!ValidateAddNewParams(newAccountData, out nmFieldValue, out nodeIdFieldValue)) {
				string errorMessage = "Error validating AddNew params";
				Log.Log(LogLevel.Error, errorMessage);
				throw new ApplicationException(errorMessage);
			}

			newAccountData[tablePrefix + "active"] = true;
			newAccountData[tablePrefix + "status"] = (short) 1;
			nmFieldValue = this.GenerateNextUniqueDbFieldValue(alias, "countbynm", nmFieldValue, 100);
			newAccountData[nmFieldName] = nmFieldValue;

			// TODO: use transaction.
			if (!nodeIdFieldValue.HasValue) {
				Nd node = new Nd();
				newNodeData["nodetyp_ndtpid"] = (short) 4; // NoteType.Account
				nodeIdFieldValue = node.AddNew(alias, "default", newNodeData, dbConnection);
				newAccountData[tablePrefix + "node_nodeid"] = nodeIdFieldValue;
			}

			long acctIdFieldValue = base.AddNew(alias, critName, newAccountData, dbConnection);
			return acctIdFieldValue;
		}

		private static bool TryParseUserLoginParams(ArrayList parameters, out string userName, out string userPassword) {
			userName = userPassword = null;
			string errorMessage;

			try {
				if (parameters == null) {
					errorMessage = "TryParseUserLoginParams: parameters array is null";
					Log.Log(LogLevel.Error, errorMessage);
					return false;
				}

				byte requiredParamsCount = 2;

				if (parameters.Count != requiredParamsCount) {
					errorMessage = string.Format("TryParseUserLoginParams: parameters array must contain {0} items while the number of its items is {1}", requiredParamsCount, parameters.Count);
					Log.Log(LogLevel.Error, errorMessage);
					return false;
				}

				ValidatorUtils.SanitiseInputParams(parameters);
				string userNameAsString = Convert.ToString(parameters[0]);

				if (string.IsNullOrEmpty(userNameAsString) || userNameAsString.Trim() == string.Empty) {
					errorMessage = "TryParseUserLoginParams: error validating userName param: " + userNameAsString;
					Log.Log(LogLevel.Error, errorMessage);
					return false;
				} else
					userName = userNameAsString;

				string userPasswordAsString = Convert.ToString(parameters[1]);

				if (string.IsNullOrEmpty(userPasswordAsString) || userPasswordAsString.Trim() == string.Empty) {
					errorMessage = "TryParseUserLoginParams: error validating userPassword param: " + userPasswordAsString;
					Log.Log(LogLevel.Error, errorMessage);
					return false;
				} else
					userPassword = userPasswordAsString;

				return true;
			} catch (Exception ex) {
				errorMessage = "TryParseUserLoginParams: error validating input params";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				return false;
			}
		}

		private object[,] UserLogin(string alias, string criterion, ArrayList parameters, long offset, long limit, string[] fieldNames) {
			string userName, userPassword;

			if (!TryParseUserLoginParams(parameters, out userName, out userPassword)) {
				string errorMessage = "UserLogin: error validating input params";
				Log.Log(LogLevel.Error, errorMessage);
				throw new ApplicationException(errorMessage);
			}

			Log.Log(LogLevel.Debug, string.Format("UserLogin: userName: {0}", userName));
			string searchCriterion = "userlogin";
			ArrayList critParameters = new ArrayList {userName};
			object[,] resultData;

			try {
				resultData = this.Search(alias, searchCriterion, critParameters, 0L, -1L, null);
			} catch (Exception ex) {
				string errorMessage = "UserLogin: execution of \"" + criterion +  "\" criterion returned error.";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				throw new ApplicationException(errorMessage);
			}

			if (resultData == null) {
				string errorMessage = "UserLogin: execution of \"" + criterion +  "\" criterion returned null data array";
				Log.Log(LogLevel.Error, errorMessage);
				throw new ApplicationException(errorMessage);
			}

			int rowsReturned = resultData.GetLength(0) - 1;

			if (rowsReturned != 1) {
				if (rowsReturned == 0) {
					Log.Log(LogLevel.Error, string.Format("UserLogin: no record has been found for userName = \"{0}\"", userName));
					throw new ApplicationException("Login failed: invalid username");
				} else if (rowsReturned > 1) {
					Log.Log(LogLevel.Error, string.Format("UserLogin: more than one record ({0}) has been found for userName = \"{1}\"", rowsReturned, userName));
					throw new ApplicationException("Login failed: login db data consistency issue");
				}
			}

			string validPasswordHash;

			try {
				validPasswordHash = (string) resultData[1, 2];
			} catch (Exception ex) {
				string errorMessage = "UserLogin: Error extracting data for \"" + criterion +  "\" criterion.";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				throw new ApplicationException(errorMessage);
			}

			try {
				IPasswordHasher pwdHasher = new PBKDF2PasswordHasher();
				bool isPasswordValid = pwdHasher.Verify(userPassword, validPasswordHash);

				if (!isPasswordValid)
					throw new ApplicationException("Login failed: invalid password");
			} catch (Exception ex) {
				string errorMessage = "UserLogin: Error validating user password for \"" + criterion +  "\" criterion.";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				throw new ApplicationException(errorMessage);
			}

			return resultData;
		}

		public bool TryUserLogin(string userName, string userPassword, out long userId, out long userEntityId) {
			userId = userEntityId = -1L;
			string alias = "acctaccount";
			string criterion = "userlogin";
			ArrayList parameters = new ArrayList {userName, userPassword};
			long offset = 0L;
			long limit = -1L;
			string[] fieldNames = null;
			bool success = false;

			try {
				object[,] data = this.UserLogin(alias, criterion, parameters, offset, limit, fieldNames);
				long resultUserId = (long) data[1, 0];
				long resultUserEntityId = (long) data[1, 1];
				userId = resultUserId;
				userEntityId = resultUserEntityId;
				success = true;
			} catch (Exception ex) {
				userId = userEntityId = -1L;
				string errorMessage = "Acct.TryUserLogin: execution of \"" + criterion +  "\" criterion returned error.";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
			}

			return success;
		}

		public bool AccountNameExists(string accountName) {
			string alias = "acctaccount";
			string criterion = "countbynm";
			ArrayList parameters = new ArrayList {accountName};
			long offset = 0L;
			long limit = -1L;
			string[] fieldNames = null;
			bool exist;

			try {
				object[,] data = this.Search(alias, criterion, parameters, offset, limit, fieldNames);
				int duplicatesCount = Convert.ToInt32(data[1, 0]);
				exist = duplicatesCount > 0;
			} catch (Exception ex) {
				string errorMessage = "Execution of \"" + criterion +  "\" criterion returned error.";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				throw;
			}

			return exist;
		}
	}
}
