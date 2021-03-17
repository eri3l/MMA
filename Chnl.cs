using System;
using System.Collections;
using System.Globalization;
using System.Text;
using app.mma.utils;
using app_c.mma.rdi;
using system.block.item;
using NLog;

namespace app.mma {
	public class Chnl : RDISplImplRWM {
		private static Logger Log = LogManager.GetCurrentClassLogger();

		public Chnl() {
			system.block.item.App_c oApp = new system.block.item.App_c(typeof(system.block.item.App_c), new string[0]);
			this.rdi = (chnlchannel_c) chnlchannel_c.getInstance(oApp.context);
		}

		public RetCde user_profiles(object[] io) {
			string alias, criterion;
			ArrayList parameters;
			long offset, limit;
			string[] fieldNames;
			UserInfo userInfo;
			string errorMessage;

			try {
				alias = (string) io[0];
				criterion = (string) io[1];
				parameters = (ArrayList) io[2];
				offset = io[3] is long ? (long) io[3] : 0L;
				limit = io[4] is long ? (long) io[4] : -1L;
				fieldNames = (string[]) io[5];
				userInfo = (UserInfo) io[6];

				if (parameters == null)
					throw new ArgumentException("Null paramaters array (io[2])", "io");

				byte requiredParamsCount = 0; // Count before adding the User Info params.

				if (parameters.Count != requiredParamsCount) {
					errorMessage = string.Format("\"parameters\" array must contain {0} items while the number of its items is {1}", requiredParamsCount, parameters.Count);
					Log.Log(LogLevel.Error, errorMessage);
					throw new ArgumentException("Incorrect number of items in paramaters array (io[2])", "io");
				}

				parameters.Add(userInfo.UserId);
			} catch (Exception ex) {
				errorMessage = "user_profiles: error validating input params";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				io[7] = new object[,] {{"chnlid", "chnlsnm"}};
				return RetCde.GeneralError;
			}

			object[,] resultArray;

			try {
				resultArray = this.UserProfiles(alias, criterion, parameters, offset, limit, fieldNames);
			} catch (Exception ex) {
				errorMessage = "user_profiles: error loading message history";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				io[7] = new object[,] {{"chnlid", "chnlsnm"}};
				return RetCde.GeneralError;
			}

			io[7] = resultArray;
			return RetCde.Success;
		}

		private static bool TryParseUserProfilesParams(ArrayList parameters, out long userId) {
			userId = 0L;
			string errorMessage;

			try {
				if (parameters == null) {
					errorMessage = "TryParseUserProfilesParams: parameters array is null";
					Log.Log(LogLevel.Error, errorMessage);
					return false;
				}

				byte requiredParamsCount = 1;

				if (parameters.Count != requiredParamsCount) {
					errorMessage = string.Format("TryParseUserProfilesParams: parameters array must contain {0} items while the number of its items is {1}", requiredParamsCount, parameters.Count);
					Log.Log(LogLevel.Error, errorMessage);
					return false;
				}

				ValidatorUtils.SanitiseInputParams(parameters);
				string userIdAsString = Convert.ToString(parameters[0]);

				if (!(long.TryParse(userIdAsString, NumberStyles.None, CultureInfo.InvariantCulture, out userId) && userId > 0)) {
					errorMessage = "TryParseUserProfilesParams: error validating userId param: " + userIdAsString;
					Log.Log(LogLevel.Error, errorMessage);
					return false;
				}

				return true;
			} catch (Exception ex) {
				errorMessage = "TryParseUserProfilesParams: error validating input params";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				return false;
			}
		}

		internal object[,] UserProfiles(string alias, string criterion, ArrayList parameters, long offset, long limit, string[] fieldNames) {
			long userId;

			if (!TryParseUserProfilesParams(parameters, out userId)) {
				string errorMessage = "UserProfiles: error validating input params";
				Log.Log(LogLevel.Error, errorMessage);
				throw new ApplicationException(errorMessage);
			}

			Log.Log(LogLevel.Debug, string.Format("UserProfiles: userId: {0}", userId));

			string searchCriterion = "userprofiles";
			ArrayList critParameters = new ArrayList {userId};
			object[,] resultData;

			try {
				resultData = this.Search(alias, searchCriterion, critParameters, 0L, -1L, null);
			} catch (Exception ex) {
				string errorMessage = "UserProfiles: execution of \"" + criterion +  "\" criterion returned error.";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				throw new ApplicationException(errorMessage);
			}

			if (resultData == null) {
				string errorMessage = "UserProfiles: execution of \"" + criterion +  "\" criterion returned null data array";
				Log.Log(LogLevel.Error, errorMessage);
				throw new ApplicationException(errorMessage);
			}

			Log.Log(LogLevel.Debug, "UserProfiles: Number of data records returned: " + (resultData.GetLength(0) - 1));
			return resultData;
		}
	}
}
