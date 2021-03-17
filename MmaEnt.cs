using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Text;
using app.pwdhasher;
using app.socnetconn;
using app_c.mma.rdi;
using system.block.item;
using Aeye.Framework;
using system.block.uda.dataSource;
using NLog;
using app.mma.utils;
using app.mma;

namespace app.mma {
	public class MmaEnt : Ent {
		private static Logger Log = LogManager.GetCurrentClassLogger();
		private const byte MinAge = 0;
		private const byte MaxAge = 120;
		private const byte MesageHistoryDefaultRecordLimit = 5;

		static MmaEnt() {
		}

		public MmaEnt() {
			system.block.item.App_c oApp = new system.block.item.App_c(typeof(system.block.item.App_c), new string[0]);
			this.rdi = (enttentity_c) enttentity_c.getInstance(oApp.context);
		}

		public override RetCde s(object[] io) {
			return this.get_fr_foll_by_geo(io);
		}

		public RetCde get_fr_foll_by_geo_stub(object[] io) {
			int paramsCount = io.Length;
			object[,] dummyArray = new object[,] {{"enttid", "enttsnm", "geolcntry", "noderpn"}, {1, "Name 1", "UK", -1}, {2, "Name 2", "France", -1}};
			io[paramsCount - 1] = dummyArray;
			return RetCde.Success;
		}

		public RetCde get_fr_foll_by_geo(object[] io) {
			string alias, criterion;
			ArrayList parameters;
			long offset, limit;
			string[] fieldNames;
			UserInfo userInfo;

			try {
				alias = (string) io[0];
				criterion = (string) io[1];
				parameters = (ArrayList) io[2];
				offset = io[3] is long ? (long) io[3] : 0L;
				limit = io[4] is long ? (long) io[4] : -1L;
				fieldNames = (string[]) io[5];
				userInfo = (UserInfo) io[6];

				if (parameters == null)
					throw new ArgumentException("Null parameters array (io[2])", "io");

				parameters[0] = userInfo.UserId;
			} catch (Exception ex) {
				string errorMessage = "get_fr_foll_by_geo: error validating input params";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				io[io.Length - 1] = new object[,] {{"enttid", "enttsnm", "geolcntry", "noderpn"}};
				return RetCde.GeneralError;
			}

			object[,] resultArray;

			try {
				resultArray = this.GetFriendsAndFollowersByGeo(alias, criterion, parameters, offset, limit, fieldNames);
			} catch (Exception ex) {
				string errorMessage = "get_fr_foll_by_geo: error getting friends and followers info by geo";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				io[io.Length - 1] = new object[,] {{"enttid", "enttsnm", "geolcntry", "noderpn"}};
				return RetCde.GeneralError;
			}

			io[io.Length - 1] = resultArray;
			return RetCde.Success;
		}

		private static bool TryParseFriendsAndFollowersByGeoParams(ArrayList parameters, out long userId, out long channelId, out ushort edgePriv, out long locationId, out ushort radius, out byte ageMin, out bool canUseAgeMinValue, out byte ageMax, out bool canUseAgeMaxValue, out string gender, out bool canUseGenderValue) {
			userId = channelId = locationId = 0L;
			edgePriv = radius = 0;
			canUseAgeMinValue = canUseAgeMaxValue = canUseGenderValue = false;
			ageMin = MinAge;
			ageMax = MaxAge;
			gender = null;
			string errorMessage;

			try {
				if (parameters == null) {
					errorMessage = "TryParseFriendsAndFollowersByGeoParams: parameters array is null";
					Log.Log(LogLevel.Error, errorMessage);
					return false;
				}

				byte requiredParamsCount = 8;

				if (parameters.Count != requiredParamsCount) {
					errorMessage = string.Format("TryParseFriendsAndFollowersByGeoParams: parameters array must contain {0} items while the number of its items is {1}", requiredParamsCount, parameters.Count);
					Log.Log(LogLevel.Error, errorMessage);
					return false;
				}

				ValidatorUtils.SanitiseInputParams(parameters);
				string userIdAsString = Convert.ToString(parameters[0]);
				
				if (!(long.TryParse(userIdAsString, NumberStyles.None, CultureInfo.InvariantCulture, out userId) && userId > 0)) {
					errorMessage = "TryParseFriendsAndFollowersByGeoParams: error validating userId param: " + userIdAsString;
					Log.Log(LogLevel.Error, errorMessage);
					return false;
				}

				string channelIdAsString = Convert.ToString(parameters[1]);
				
				if (!(long.TryParse(channelIdAsString, NumberStyles.None, CultureInfo.InvariantCulture, out channelId) && channelId > 0)) {
					errorMessage = "TryParseFriendsAndFollowersByGeoParams: error validating channelId param: " + channelIdAsString;
					Log.Log(LogLevel.Error, errorMessage);
					return false;
				}

				string edgePrivAsString = Convert.ToString(parameters[2]);

				if (!(ushort.TryParse(edgePrivAsString, NumberStyles.None, CultureInfo.InvariantCulture, out edgePriv) && (edgePriv >= 1 && edgePriv <= 3))) {
					errorMessage = "TryParseFriendsAndFollowersByGeoParams: error validating edgePriv param: " + edgePrivAsString;
					Log.Log(LogLevel.Error, errorMessage);
					return false;
				}

				string locationIdAsString = Convert.ToString(parameters[3]);
				
				if (!(long.TryParse(locationIdAsString, NumberStyles.None, CultureInfo.InvariantCulture, out locationId) && locationId > 0)) {
					errorMessage = "TryParseFriendsAndFollowersByGeoParams: error validating locationId param: " + locationIdAsString;
					Log.Log(LogLevel.Error, errorMessage);
					return false;
				}

				string radiusAsString = Convert.ToString(parameters[4]);
				                      
				if (!ushort.TryParse(radiusAsString, NumberStyles.None, CultureInfo.InvariantCulture, out radius)) {
					errorMessage = "TryParseFriendsAndFollowersByGeoParams: error validating radius param: " + radiusAsString;
					Log.Log(LogLevel.Error, errorMessage);
					return false;
				}

				string ageMinAsString = Convert.ToString(parameters[5]);

				if (byte.TryParse(ageMinAsString, NumberStyles.None, CultureInfo.InvariantCulture, out ageMin) && (ageMin >= MinAge  && ageMin <= MaxAge))
					canUseAgeMinValue = true;
				else {
					canUseAgeMinValue = false;
					ageMin = MinAge;
				}

				string ageMaxAsString = Convert.ToString(parameters[6]);

				if (byte.TryParse(ageMaxAsString, NumberStyles.None, CultureInfo.InvariantCulture, out ageMax) && (ageMax >= MinAge && ageMax <= MaxAge))
					canUseAgeMaxValue = true;
				else {
					canUseAgeMaxValue = false;
					ageMax = MaxAge;
				}

				if (canUseAgeMinValue && canUseAgeMaxValue && ageMin > ageMax) {
					errorMessage = string.Format("TryParseFriendsAndFollowersByGeoParams: error validating age params: ageMin ({0}) is greater than ageMax ({1})", ageMin, ageMax);
					Log.Log(LogLevel.Error, errorMessage);
					return false;
				}

				string genderAsString = Convert.ToString(parameters[7]);
				ushort genderAsInt;

				if (ushort.TryParse(genderAsString, NumberStyles.None, CultureInfo.InvariantCulture, out genderAsInt) && (genderAsInt >= 0 && genderAsInt <= 3)) {
					switch (genderAsInt) {
						case 0:
							canUseGenderValue = false;
							break;
						case 1:
							gender = "m";
							canUseGenderValue = true;
							break;
						case 2:
							gender = "f";
							canUseGenderValue = true;
							break;
						case 3:
							gender = "o";
							canUseGenderValue = true;
							break;
						default:
							canUseGenderValue = false;
							errorMessage = "TryParseFriendsAndFollowersByGeoParams: error validating gender param: " + genderAsString;
							Log.Log(LogLevel.Error, errorMessage);
							return false;
					}
				} else {
					canUseGenderValue = false;
					errorMessage = "TryParseFriendsAndFollowersByGeoParams: error validating gender param: " + genderAsString;
					Log.Log(LogLevel.Error, errorMessage);
					return false;
				}

				return true;
			} catch (Exception ex) {
				errorMessage = "TryParseFriendsAndFollowersByGeoParams: error validating input params";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				return false;
			}
		}

		internal object[,] GetFriendsAndFollowersByGeo(string alias, string criterion, ArrayList parameters, long offset, long limit, string[] fieldNames) {
			long userId, channelId, locationId;
			ushort edgePriv, radius;
			byte ageMin, ageMax;
			bool canUseAgeMinValue, canUseAgeMaxValue, canUseGenderValue;
			string gender;

			if (!TryParseFriendsAndFollowersByGeoParams(parameters, out userId, out channelId, out edgePriv, out locationId, out radius, out ageMin, out canUseAgeMinValue, out ageMax, out canUseAgeMaxValue, out gender, out canUseGenderValue)) {
				string errorMessage = "GetFriendsAndFollowersByGeo: error validating input params";
				Log.Log(LogLevel.Error, errorMessage);
				throw new ApplicationException(errorMessage);
			}

			Log.Log(LogLevel.Debug, string.Format("GetFriendsAndFollowersByGeo: userId: {0}, channelId: {1}, edgePriv: {2}, locationId: {3}, radius: {4}, ageMin: {5} ({8}), ageMax: {6} ({9}), gender: {7} ({10})", userId, channelId, edgePriv, locationId, radius, ageMin, ageMax, gender, canUseAgeMinValue, canUseAgeMaxValue, canUseGenderValue));
			double geolbxMinOriginal;
			double geolbxMaxOriginal;
			double geolbyMinOriginal;
			double geolbyMaxOriginal;
			GetGeoRecordBoundaryBox (alias, locationId, out geolbxMinOriginal, out geolbxMaxOriginal, out geolbyMinOriginal, out geolbyMaxOriginal);
			Log.Log(LogLevel.Debug, string.Format("GetFriendsAndFollowersByGeo: geolbxMinOriginal: {0}, geolbxMaxOriginal: {1}, geolbyMinOriginal: {2}, geolbyMaxOriginal: {3}", geolbxMinOriginal, geolbxMaxOriginal, geolbyMinOriginal, geolbyMaxOriginal));
			double geolxMin, geolxMax, geolyMin, geolyMax;

			if (radius == 0) {
				// No need to calculate extra radius boundary.
				geolxMin = geolbxMinOriginal;
				geolxMax = geolbxMaxOriginal;
				geolyMin = geolbyMinOriginal;
				geolyMax = geolbyMaxOriginal;
			} else {
				// Location's bounding box has two points calculated: xyMin and xyMax.
				// Calculate the bounding box for xyMin point using the radius. From the result xy1Min and xy1Max use the xy1Min as the xyMin of the result bounding box.
				GeoLocation xyMinOriginal = GeoLocation.FromDegrees(geolbxMinOriginal, geolbyMinOriginal);
				GeoLocation[] xyMinBoundingBoxArray = xyMinOriginal.GetBoundingCoordinates((double) radius, true);
				GeoLocation xyMinBoundingBox = xyMinBoundingBoxArray[0];
				geolxMin = xyMinBoundingBox.LatitudeInDegrees;
				geolyMin = xyMinBoundingBox.LongitudeInDegrees;
				Log.Log(LogLevel.Debug, string.Format("GetFriendsAndFollowersByGeo: x1Min: {0}, y1Min: {1}, x1Max: {2}, y1Max: {3}", geolxMin, geolyMin, xyMinBoundingBoxArray[1].LatitudeInDegrees, xyMinBoundingBoxArray[1].LongitudeInDegrees));

				// Calculate the bounding box for xyMax point using the radius. From the result xy2Min and xy2Max use the xy2Min as the xyMax of the result bounding box.
				GeoLocation xyMaxOriginal = GeoLocation.FromDegrees(geolbxMaxOriginal, geolbyMaxOriginal);
				GeoLocation[] xyMaxBoundingBoxArray = xyMaxOriginal.GetBoundingCoordinates((double) radius, true);
				GeoLocation xyMaxBoundingBox = xyMaxBoundingBoxArray[1];
				geolxMax = xyMaxBoundingBox.LatitudeInDegrees;
				geolyMax = xyMaxBoundingBox.LongitudeInDegrees;
				Log.Log(LogLevel.Debug, string.Format("GetFriendsAndFollowersByGeo: x2Min: {0}, y2Min: {1}, x2Max: {2}, y2Max: {3}", xyMaxBoundingBoxArray[0].LatitudeInDegrees, xyMaxBoundingBoxArray[0].LongitudeInDegrees, geolxMax, geolyMax));
			}

			Log.Log(LogLevel.Debug, string.Format("GetFriendsAndFollowersByGeo: geolxMin: {0}, geolxMax: {1}, geolyMin: {2}, geolyMax: {3}, ", geolxMin, geolxMax, geolyMin, geolyMax));

			string searchCriterion;
			ArrayList critParameters;

			if (canUseAgeMaxValue || canUseAgeMinValue) {
				if (!canUseAgeMaxValue)
					ageMax = MaxAge;

				if (!canUseAgeMinValue)
					ageMin = MinAge;

				long birthdMin, birthdMax;
				CalculateBirthdateUnixTimeStamps(ageMin, ageMax, out birthdMin, out birthdMax);

				if (canUseGenderValue) {
					searchCriterion = "getfrfollbygeoagegender";
					critParameters = new ArrayList {channelId, userId, edgePriv, gender, locationId, geolxMin, geolxMax, geolyMin, geolyMax, birthdMin, birthdMax};
				} else {
					searchCriterion = "getfrfollbygeoage";
					critParameters = new ArrayList {channelId, userId, edgePriv, locationId, geolxMin, geolxMax, geolyMin, geolyMax, birthdMin, birthdMax};
				}
			} else {
				if (canUseGenderValue) {
					searchCriterion = "getfrfollbygeogender";
					critParameters = new ArrayList {channelId, userId, edgePriv, gender, locationId, geolxMin, geolxMax, geolyMin, geolyMax};
				} else {
					searchCriterion = "getfrfollbygeo";
					critParameters = new ArrayList {channelId, userId, edgePriv, locationId, geolxMin, geolxMax, geolyMin, geolyMax};
				}
			}

			object[,] resultData;

			try {
				resultData = this.Search(alias, searchCriterion, critParameters, 0L, -1L, null);
			} catch (Exception ex) {
				string errorMessage = "GetFriendsAndFollowersByGeo: execution of \"" + criterion +  "\" criterion returned error.";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				throw new ApplicationException(errorMessage);
			}

			if (resultData == null) {
				string errorMessage = "GetFriendsAndFollowersByGeo: execution of \"" + criterion +  "\" criterion returned null data array";
				Log.Log(LogLevel.Error, errorMessage);
				throw new ApplicationException(errorMessage);
			}

			Log.Log(LogLevel.Debug, "GetFriendsAndFollowersByGeo: Number of data records returned: " + (resultData.GetLength(0) - 1));
			return resultData;
		}

		private static void CalculateBirthdateUnixTimeStamps(byte ageMin, byte ageMax, out long birthdMin, out long birthdMax) {
			DateTime utcNow = DateTime.UtcNow;
			// Birthdate min date range begins one year earlier minus 1 day, eg. the age 30 includes the days between age 30 and age 31.
			DateTime minBirthdDate = DateTime.UtcNow.AddYears(-(ageMax + 1)).AddDays(1);
			DateTime minBirthdDateEnd = new DateTime(minBirthdDate.Year, minBirthdDate.Month, minBirthdDate.Day, 0, 0, 0, DateTimeKind.Utc);
			birthdMin = DateTimeUtils.ConvertToUnixTime(minBirthdDateEnd);
			Log.Log(LogLevel.Debug, string.Format("CalculateBirthdateUnixTimeStamps: birthdDateMin: {0}, birthdMin (Unix timestamp): {1}", minBirthdDateEnd.ToString("yyyy-MM-dd HH:mm:ss"), birthdMin));

			DateTime maxBirthdDate = utcNow.AddYears(-ageMin);
			DateTime maxBirthdDateStart = new DateTime(maxBirthdDate.Year, maxBirthdDate.Month, maxBirthdDate.Day, 23, 59, 59, DateTimeKind.Utc);
			birthdMax = DateTimeUtils.ConvertToUnixTime(maxBirthdDateStart);
			Log.Log(LogLevel.Debug, string.Format("CalculateBirthdateUnixTimeStamps: birthdDateMax: {0}, birthdMax (Unix timestamp): {1}", maxBirthdDateStart.ToString("yyyy-MM-dd HH:mm:ss"), birthdMax));
		}

		private static void GetGeoRecordBoundaryBox(string alias, long locationId, out double geolbxMin, out double geolbxMax, out double geolbyMin, out double geolbyMax) {
			Geo geo = new Geo();
			string criterion = "id";
			ArrayList parameters = new ArrayList {locationId};
			object[,] data;

			try {
				data = geo.Search(alias, criterion, parameters, 0L, -1L, null);
			} catch (Exception ex) {
				string errorMessage = "GetGeoRecordBoundaryBox: execution of \"" + criterion +  "\" criterion returned error.";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				throw new ApplicationException(errorMessage);
			}

			int dataRowsCount;

			if (data == null) {
				string errorMessage = "GetGeoRecordBoundaryBox: execution of \"" + criterion +  "\" criterion returned null data array";
				Log.Log(LogLevel.Error, errorMessage);
				throw new ApplicationException(errorMessage);
			}

			dataRowsCount = data.GetLength(0);

			if (dataRowsCount != 2) {
				string errorMessage = "GetGeoRecordBoundaryBox: execution of \"" + criterion +  "\" criterion returned incorrect number of rows: " + dataRowsCount;
				Log.Log(LogLevel.Error, errorMessage);
				throw new ApplicationException(errorMessage);
			}

			string geolb = Convert.ToString(data[1, 10]);
			Log.Log(LogLevel.Debug, "GetGeoRecordBoundaryBox: geolb: " + geolb);
			string[] boundaryBoxArray = geolb.Split(new char[] {','});

			if (boundaryBoxArray.Length != 4) {
				string errorMessage = "GetGeoRecordBoundaryBox: execution of \"" + criterion +  "\" criterion returned invalid number of coordinates in geolb field: " + boundaryBoxArray.Length;
				Log.Log(LogLevel.Error, errorMessage);
				throw new ApplicationException(errorMessage);
			}

			geolbxMin = Convert.ToDouble(boundaryBoxArray[0].Trim());
			geolbyMin = Convert.ToDouble(boundaryBoxArray[1].Trim());
			geolbxMax = Convert.ToDouble(boundaryBoxArray[2].Trim());
			geolbyMax = Convert.ToDouble(boundaryBoxArray[3].Trim());
		}

		public RetCde get_profile(object[] io) {
			Log.Log(LogLevel.Debug, "get_profile");
			int paramsCount = io.Length;
			object[,] dummyArray = new object[,] {
				{"enttid", "enttsnm"},
				{1, "Name 1"}
			};
			io[paramsCount - 1] = dummyArray;
			return RetCde.Success;
		}

		private static bool TryParseSearchMsgHistoryParams(ArrayList parameters, out long srcEntId, out long targEntId, out DateTime? dateStart, out DateTime dateEnd, out string orderBy, out ushort recordLimit, out long userId) {
			srcEntId = targEntId = userId = 0L;
			dateStart = null;
			dateEnd = DateTime.UtcNow;
			orderBy = "DESC";
			recordLimit = 5;
			string errorMessage;

			try {
				if (parameters == null) {
					errorMessage = "TryParseSearchMsgHistoryParams: parameters array is null";
					Log.Log(LogLevel.Error, errorMessage);
					return false;
				}

				byte requiredParamsCount = 7;

				if (parameters.Count != requiredParamsCount) {
					errorMessage = string.Format("TryParseSearchMsgHistoryParams: parameters array must contain {0} items while the number of its items is {1}", requiredParamsCount, parameters.Count);
					Log.Log(LogLevel.Error, errorMessage);
					return false;
				}

				ValidatorUtils.SanitiseInputParams(parameters);
				string targEntIdAsString = Convert.ToString(parameters[0]);

				if (!(long.TryParse(targEntIdAsString, NumberStyles.None, CultureInfo.InvariantCulture, out targEntId) && targEntId > 0)) {
					errorMessage = "TryParseSearchMsgHistoryParams: error validating targEntId param: " + targEntIdAsString;
					Log.Log(LogLevel.Error, errorMessage);
					return false;
				}

				string dateTimeStringFormat = "yyyy-MM-dd HH:mm:ss";
				string dateStartAsString = Convert.ToString(parameters[1]);

				if (!string.IsNullOrEmpty(dateStartAsString)) {
					DateTime tmpDateStart;

					if (DateTime.TryParseExact(dateStartAsString, dateTimeStringFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDateStart))
						dateStart = tmpDateStart;
					else {
						errorMessage = "TryParseSearchMsgHistoryParams: error validating dateStart param: " + dateStartAsString;
						Log.Log(LogLevel.Error, errorMessage);
						return false;
					}
				}

				string dateEndAsString = Convert.ToString(parameters[2]);

				if (string.IsNullOrEmpty(dateEndAsString)) {
					dateEnd = DateTime.UtcNow;
				} else {
					DateTime tmpDateEnd;

					if (DateTime.TryParseExact(dateEndAsString, dateTimeStringFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDateEnd)) {
						if (dateStart.HasValue) {
							if (dateStart < tmpDateEnd)
								dateEnd = tmpDateEnd;
							else {
								errorMessage = string.Format("TryParseSearchMsgHistoryParams: error validating dateEnd param: {0}. It must be greater than startDate: {1}: ", dateEndAsString, ((DateTime) dateStart).ToString(dateTimeStringFormat));
								Log.Log(LogLevel.Error, errorMessage);
								return false;
							}
						}
					} else {
						errorMessage = "TryParseSearchMsgHistoryParams: error validating dateEnd param: " + dateEndAsString;
						Log.Log(LogLevel.Error, errorMessage);
						return false;
					}
				}

				string orderByAsString = Convert.ToString(parameters[3]);

				if (string.Compare(orderByAsString, "DESC", true) == 0 || string.Compare(orderByAsString, "ASC", true) == 0)
					orderBy = orderByAsString.ToUpper();
				else {
					errorMessage = "TryParseSearchMsgHistoryParams: error validating orderBy param: " + orderByAsString;
					Log.Log(LogLevel.Error, errorMessage);
					return false;
				}

				string recordLimitAsString = Convert.ToString(parameters[4]);

				if (string.IsNullOrEmpty(recordLimitAsString))
					recordLimit = MesageHistoryDefaultRecordLimit;
				else if (!ushort.TryParse(recordLimitAsString, NumberStyles.None, CultureInfo.InvariantCulture, out recordLimit)) {
					errorMessage = "TryParseSearchMsgHistoryParams: error validating recordLimit param: " + recordLimitAsString;
					Log.Log(LogLevel.Error, errorMessage);
					return false;
				}

				string userIdAsString = Convert.ToString(parameters[5]);

				if (!(long.TryParse(userIdAsString, NumberStyles.None, CultureInfo.InvariantCulture, out userId) && userId > 0)) {
					errorMessage = "TryParseSearchMsgHistoryParams: error validating userId param: " + userIdAsString;
					Log.Log(LogLevel.Error, errorMessage);
					return false;
				}

				string srcEntIdAsString = Convert.ToString(parameters[6]);

				if (!(long.TryParse(srcEntIdAsString, NumberStyles.None, CultureInfo.InvariantCulture, out srcEntId) && srcEntId > 0)) {
					errorMessage = "TryParseSearchMsgHistoryParams: error validating srcEntId param: " + srcEntIdAsString;
					Log.Log(LogLevel.Error, errorMessage);
					return false;
				}

				return true;
			} catch (Exception ex) {
				errorMessage = "TryParseFriendsAndFollowersByGeoParams: error validating input params";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				return false;
			}
		}

		public RetCde search_msg_hst_stub(object[] io) {
			Log.Log(LogLevel.Debug, "search_msg_hst");
			int paramsCount = io.Length;
			ArrayList parameters = (ArrayList) io[2];
			int entId;

			if (!int.TryParse(Convert.ToString(parameters[0]), out entId))
					return RetCde.GeneralError;

			object[,] dummyArray;

			if (entId > -1) {
				string name = "Name " + entId;

				dummyArray = new object[,] {
					{"msgqid", "enttsnm", "msgqtext", "msgqdate"},
					{1, name, "Message 1", "01 Apr 2015 15:36:27"},
					{5, name, "Message 2", "01 Apr 2015 19:03:18"},
					{7, name, "Message 3", "02 Apr 2015 17:17:03"},
					{9, name, "Message 4", "12 Apr 2015 22:12:05"},
					{11, name, "Message 5", "14 Apr 2015 23:11:43"}
				};
			} else
				dummyArray = new object[,] {
					{"msgqid", "enttsnm", "msgqtext", "msgqdate"}
				};

			io[paramsCount - 1] = dummyArray;
			return RetCde.Success;
		}

		public RetCde search_msg_hst(object[] io) {
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
					throw new ArgumentException("Null parameters array (io[2])", "io");

				byte requiredParamsCount = 5; // Count before adding the User Info params.

				if (parameters.Count != requiredParamsCount) {
					errorMessage = string.Format("\"parameters\" array must contain {0} items while the number of its items is {1}", requiredParamsCount, parameters.Count);
					Log.Log(LogLevel.Error, errorMessage);
					throw new ArgumentException("Incorrect number of items in paramaters array (io[2])", "io");
				}

				parameters.Add(userInfo.UserId);
				parameters.Add(userInfo.EntityId);
			} catch (Exception ex) {
				errorMessage = "search_msg_hst: error validating input params";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				io[7] = new object[,] {{"msgqid", "enttsnm", "msgqtext", "msgqdate"}};
				return RetCde.GeneralError;
			}

			object[,] resultArray;

			try {
				resultArray = this.SearchMessageHistory(alias, criterion, parameters, offset, limit, fieldNames);
			} catch (Exception ex) {
				errorMessage = "search_msg_hst: error loading message history";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				io[7] = new object[,] {{"msgqid", "enttsnm", "msgqtext", "msgqdate"}};
				return RetCde.GeneralError;
			}

			io[7] = resultArray;
			return RetCde.Success;
		}

		internal object[,] SearchMessageHistory(string alias, string criterion, ArrayList parameters, long offset, long limit, string[] fieldNames) {
			long srcEntId, targEntId, userId;
			DateTime? dateStart;
			DateTime dateEnd;
			string orderBy;
			ushort recordLimit;

			if (!TryParseSearchMsgHistoryParams(parameters, out srcEntId, out targEntId, out dateStart, out dateEnd, out orderBy, out recordLimit, out userId)) {
				string errorMessage = "SearchMessageHistory: error validating input params";
				Log.Log(LogLevel.Error, errorMessage);
				throw new ApplicationException(errorMessage);
			}

			Log.Log(LogLevel.Debug, string.Format("SearchMessageHistory: targEntId: {0}, dateStart: {1}, dateEnd: {2}, orderBy: {3}, recordLimit: {4}, userId: {5}, srcEntId: {6}", targEntId, dateStart, dateEnd, orderBy, recordLimit, userId, srcEntId));

			string searchCriterion = "searchmsghstnodaterange";
			ArrayList critParameters = new ArrayList {userId, srcEntId, targEntId /*, dateStart, dateEnd, orderBy, recordLimit*/};
			object[,] resultData;

			try {
				resultData = this.Search(alias, searchCriterion, critParameters, 0L, (long) recordLimit, null);
			} catch (Exception ex) {
				string errorMessage = "SearchMessageHistory: execution of \"" + criterion +  "\" criterion returned error.";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				throw new ApplicationException(errorMessage);
			}

			if (resultData == null) {
				string errorMessage = "SearchMessageHistory: execution of \"" + criterion +  "\" criterion returned null data array";
				Log.Log(LogLevel.Error, errorMessage);
				throw new ApplicationException(errorMessage);
			}

			Log.Log(LogLevel.Debug, "SearchMessageHistory: Number of data records returned: " + (resultData.GetLength(0) - 1));
			return resultData;
		}

		public RetCde send_msg(object[] io) {
			Log.Log(LogLevel.Debug, "send_msg");
			string[] parameters;
			UserInfo userInfo;
			string errorMessage;

			try {
				parameters = (string[]) io[0];
				userInfo = (UserInfo) io[1];

				if (parameters == null)
					throw new ArgumentException("Null parameters array (io[2])", "io");

				byte requiredParamsCount = 7; // Count before adding the User Info params.

				if (parameters.Length != requiredParamsCount) {
					errorMessage = string.Format("\"parameters\" array must contain {0} items while the number of its items is {1}", requiredParamsCount, parameters.Length);
					Log.Log(LogLevel.Error, errorMessage);
					throw new ArgumentException("Incorrect number of items in paramaters array (io[2])", "io");
				}

				string[] newParameters = new string[requiredParamsCount + 1];
				Array.Copy(parameters, newParameters, requiredParamsCount);
				newParameters[requiredParamsCount] = userInfo.EntityId.ToString();
				parameters = newParameters;
			} catch (Exception ex) {
				errorMessage = "send_msg: error validating input params";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				return RetCde.GeneralError;
			}

			try {
				this.SendMsg(parameters);
			} catch (Exception ex) {
				errorMessage = "send_msg: error processing messagess sending";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				return RetCde.GeneralError;
			}

			return RetCde.Success;
		}

		private static bool TryParseStringAsArrayOfIntegers(string arrayAsString, bool emptyIsValid, bool toArrayIfValid, out long[] parsedArray, out string errorMessage) {
			errorMessage = null;
			parsedArray = null;

			if (arrayAsString == null || arrayAsString == string.Empty) {
				if (emptyIsValid)
					return true;
				else {
					errorMessage = "The array is empty";
					return false;
				}
			}

			if (!arrayAsString.StartsWith("{") || !arrayAsString.EndsWith("}")) {
					errorMessage = "The array must be enclosed with {} brackets";
					return false;
			}

			int arrayAsStringLength = arrayAsString.Length;

			if (arrayAsStringLength < 3) {
					if (emptyIsValid)
						return true;
					else {
						errorMessage = "No content inside array {} brackets";
						return false;
					}
			}

			string[] array = arrayAsString.Substring(1, arrayAsStringLength - 2).Split(new char[] {','});
			int arrayLength = array.Length;

			if (toArrayIfValid) {
				parsedArray = new long[arrayLength];

				for (int i = 0; i < arrayLength; i++) {
					string item = array[i];
					long number;

					if (long.TryParse(item, NumberStyles.None, CultureInfo.InvariantCulture, out number) && number > 0L)
						parsedArray[i] = number;
					else {
						errorMessage = string.Format("Error validating item with index {0} inside array of numbers. It must be positive integer number: {1}", i, item);
						return false;
					}
				}
			} else
				for (int i = 0; i < arrayLength; i++) {
					string item = array[i];
					long number;

					if (!(long.TryParse(item, NumberStyles.None, CultureInfo.InvariantCulture, out number) && number > 0L)) {
						errorMessage = string.Format("Error validating item with index {0} inside array of numbers. It must be positive integer number: {1}", i, item);
						return false;
					}
				}

			return true;
		}

		private static bool TryParseSendMsgParams(string[] parameters, out long[] targEnt, out string targSrc, out long prov, out ushort msgTyp, out string text, out long? doc, out DateTime dateStart, out DateTime? dateEnd) {
			targSrc = text = null;
			targEnt = null;
			prov = 0L;
			doc = null;
			msgTyp = 0;
			dateStart = DateTime.UtcNow;
			dateEnd = null;
			string errorMessage;

			try {
				if (parameters == null) {
					errorMessage = "TryParseSendMsgParams: parameters array is null";
					Log.Log(LogLevel.Error, errorMessage);
					return false;
				}

				if (parameters.Length != 8) {
					errorMessage = "TryParseSendMsgParams: parameters array must contain 0 items while the number of its items is: " + parameters.Length;
					Log.Log(LogLevel.Error, errorMessage);
					return false;
				}

				ValidatorUtils.SanitiseInputParams(parameters);
				string targEntAsString = Convert.ToString(parameters[0]);

				if (!TryParseStringAsArrayOfIntegers(targEntAsString, false, true, out targEnt, out errorMessage)) {
					errorMessage = "TryParseSendMsgParams: error validating targEnt param: " + targEntAsString + ". Error message: " + errorMessage;
					Log.Log(LogLevel.Error, errorMessage);
					return false;
				}

				string targSrcAsString = Convert.ToString(parameters[1]);
				long[] targSrcAsArray;

				if (TryParseStringAsArrayOfIntegers(targSrcAsString, false, false, out targSrcAsArray, out errorMessage))
					targSrc = targSrcAsString;
				else {
					errorMessage = "TryParseSendMsgParams: error validating targSrc param: " + targSrcAsString + ". Error message: " + errorMessage;
					Log.Log(LogLevel.Error, errorMessage);
					return false;
				}

				string msgTypAsString = Convert.ToString(parameters[2]);

				if (!(ushort.TryParse(msgTypAsString, NumberStyles.None, CultureInfo.InvariantCulture, out msgTyp) && (msgTyp >= 0 && msgTyp <= 2))) {
					errorMessage = "TryParseSendMsgParams: error validating msgTyp param: " + msgTypAsString;
					Log.Log(LogLevel.Error, errorMessage);
					return false;
				}

				string textAsString = Convert.ToString(parameters[3]);
				
				if (string.IsNullOrEmpty(textAsString) || textAsString.Trim() == string.Empty) {
					errorMessage = "TryParseSendMsgParams: error validating text param: " + textAsString;
					Log.Log(LogLevel.Error, errorMessage);
					return false;
				} else
					text = textAsString;

				string docAsString = Convert.ToString(parameters[4]);

				if (docAsString != null && docAsString.Trim() != string.Empty) {
					long docTmp;

					if (long.TryParse(docAsString, NumberStyles.None, CultureInfo.InvariantCulture, out docTmp) && docTmp > 0)
						doc = docTmp;
					else {
						errorMessage = "TryParseSendMsgParams: error validating doc param: " + docAsString;
						Log.Log(LogLevel.Error, errorMessage);
						return false;
					}
				}

				string dateTimeStringFormat = "yyyy-MM-dd HH:mm:ss";
				string dateStartAsString = Convert.ToString(parameters[5]);

				if (!string.IsNullOrEmpty(dateStartAsString)) {
					DateTime tmpDateStart;

					if (DateTime.TryParseExact(dateStartAsString, dateTimeStringFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDateStart))
						dateStart = tmpDateStart;
					else {
						errorMessage = "TryParseSendMsgParams: error validating dateStart param: " + dateStartAsString;
						Log.Log(LogLevel.Error, errorMessage);
						return false;
					}
				}

				string dateEndAsString = Convert.ToString(parameters[6]);

				if (!string.IsNullOrEmpty(dateEndAsString)) {
					DateTime tmpDateEnd;

					if (DateTime.TryParseExact(dateEndAsString, dateTimeStringFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out tmpDateEnd)) {
						if (dateStart < tmpDateEnd)
							dateEnd = tmpDateEnd;
						else {
							errorMessage = string.Format("TryParseSendMsgParams: error validating dateEnd param {0}. It must be greater than startDate: {1}: " + dateEndAsString, dateStart.ToString(dateTimeStringFormat));
							Log.Log(LogLevel.Error, errorMessage);
							return false;
						}
					} else {
						errorMessage = "TryParseSendMsgParams: error validating dateEnd param: " + dateEndAsString;
						Log.Log(LogLevel.Error, errorMessage);
						return false;
					}
				}

				string provAsString = Convert.ToString(parameters[7]);

				if (!(long.TryParse(provAsString, NumberStyles.None, CultureInfo.InvariantCulture, out prov) && prov > 0L)) {
					errorMessage = "TryParseSendMsgParams: error validating prov param: " + provAsString;
					Log.Log(LogLevel.Error, errorMessage);
					return false;
				}

				return true;
			} catch (Exception ex) {
				errorMessage = "TryParseSendMsgParams: error validating input params";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				return false;
			}
		}

		internal void SendMsg(string[] parameters) {
			Log.Log(LogLevel.Debug, "SendMsg");
			string targSrc, text;
			long[] targEnt;
			long prov;
			long? doc;
			ushort msgTyp;
			DateTime dateStart;
			DateTime? dateEnd;

			if (!TryParseSendMsgParams(parameters, out targEnt, out targSrc, out prov, out msgTyp, out text, out doc, out dateStart, out dateEnd)) {
				string errorMessage = "SendMsg: error validating input params";
				Log.Log(LogLevel.Error, errorMessage);
				throw new ApplicationException(errorMessage);
			}

			Log.Log(LogLevel.Debug, string.Format("send_msg: targent.Length: {0}", targEnt.Length));
			Log.Log(LogLevel.Debug, string.Format("send_msg: targsrc: {0}", targSrc));
			Log.Log(LogLevel.Debug, string.Format("send_msg: prov: {0}", prov));
			Log.Log(LogLevel.Debug, string.Format("send_msg: msgtyp: {0}", msgTyp));
			Log.Log(LogLevel.Debug, string.Format("send_msg: text: {0}", text));
			Log.Log(LogLevel.Debug, string.Format("send_msg: doc: {0}", doc));
			Log.Log(LogLevel.Debug, string.Format("send_msg: date: {0}", dateStart));
			Log.Log(LogLevel.Debug, string.Format("send_msg: dateend = {0}", dateEnd));

			Dictionary<string, object> newData = new Dictionary<string, object>(9);
			newData["msgqprov_enttid"] = prov;
			newData["msgqsrc"] = targSrc;
			newData["msgqtyp"] = msgTyp;
			newData["msgqtext"] = text;
			newData["msgqdoc_docuid"] = doc;
			newData["msgqdatestart"] = dateStart;
			newData["msgqdateend"] = dateEnd;
			newData["msgqstatus"] = 1; // app.mma.msgqd.MessageStatus.Queued1
			newData["msgqactive"] = true;

			string alias = "msgqmessagequeue";
			string criterion = "id";
			MsgQ msgQ = new MsgQ();

			foreach (long targEntId in targEnt) {
				newData["msgqtarg_enttid"] = targEntId;
				msgQ.AddNew(alias, criterion, newData);
			}
		}

		private bool ValidateRegisterUserParams(string accountName, string screenName, string accountSecret, char? gender, short? accountDayLimit) {
			try {
				if (string.IsNullOrEmpty(accountName))
					throw new ArgumentException("Null or empty \"accountName\" parameter", "accountName");

				if (string.IsNullOrEmpty(accountSecret))
					throw new ArgumentException("Null or empty \"accountSecret\" parameter", "accountSecret");

				if (gender.HasValue) {
					switch (gender) {
						case 'm':
						case 'f':
						case 'o':
							break;
						default:
							throw new ArgumentException(string.Format("Invalid value ({0}) for \"gender\" parameter", gender), "gender");
					}
				}

				if (accountDayLimit.HasValue && accountDayLimit < 0)
					throw new ArgumentException(string.Format("The \"accountDayLimit\" parameter has negative value", accountDayLimit), "accountDayLimit");

				return true;
			} catch (Exception ex) {
				string errorMessage = "Error validating RegisterUser params";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				return false;
			}
		}

		public bool RegisterUser(string accountName, string screenName, string accountSecret, char? gender, short? accountDayLimit, out UserInfo userInfo) {
			userInfo = new UserInfo();

			try {
				Log.Log(LogLevel.Debug, string.Format("RegisterAccount: accountName = \"{0}\", screenName = \"{1}\", accountSecret = \"***\", gender = \"{3}\", accountDayLimit = \"{4}\"", accountName, screenName, accountSecret, gender, accountDayLimit));

				if (!ValidateRegisterUserParams(accountName, screenName, accountSecret, gender, accountDayLimit)) {
					string errorMessage = "Error validating RegisterUser params";
					Log.Log(LogLevel.Error, errorMessage);
					throw new ApplicationException(errorMessage);
				}

				if (string.IsNullOrEmpty(screenName))
					screenName = accountName;

				#region Set Entity Data
				Ent ent = new Ent();
				Dictionary<string, object> newEntityData = new Dictionary<string, object>(4);
				newEntityData["nodenm"] = "entt";
				newEntityData["enttnm"] = accountName;
				newEntityData["enttsnm"] = screenName;

				if (gender.HasValue)
					newEntityData["enttgender"] = gender;
				#endregion

				#region Set Account Data
				Acct acct = new Acct();
				Dictionary<string, object> newAccountData = new Dictionary<string, object>(7);
				newAccountData["nodenm"] = "acct";
				newAccountData["acctnm"] = accountName;
				newAccountData["acctsnm"] = screenName;
				newAccountData["acctsource_srceid"] = 1L; // MMA
				IPasswordHasher pwdHasher = new PBKDF2PasswordHasher();
				newAccountData["acctsecret"] = pwdHasher.GenerateHash(accountSecret);

				if (accountDayLimit.HasValue)
					newAccountData["acctdaylimit"] = accountDayLimit;
				#endregion

				#region Set User Data
				Usr usr = new Usr();
				Dictionary<string, object> newUserData = new Dictionary<string, object>(4);
				newUserData["nodenm"] = "user";
				newUserData["nodebirthd"] = DateTime.UtcNow;
				newUserData["usernm"] = accountName;
				#endregion

				long entityId, userId;
				AeyeConnection aeyeDbConn = new AeyeConnection();

				using (IDbConnection dbConn = aeyeDbConn.CreateDbConnection()) {
					dbConn.Open();
					IDbTransaction tr = null;

					using (tr = dbConn.BeginTransaction()) {
						try {
							entityId = ent.AddNew(null, "default", newEntityData, dbConn);

							newAccountData["acctentity_enttid"] = entityId;
							long accountId = acct.AddNew(null, "default", newAccountData, dbConn);

							newUserData["useraccount_acctid"] = accountId;
							userId = usr.AddNew(null, "default", newUserData, dbConn);
							tr.Commit();
						} catch (Exception ex) {
							Log.Log(LogLevel.Error, "Error storing User's data in the db, Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);

							if (tr != null) {
								try {
									tr.Rollback();
									Log.Log(LogLevel.Error, "Rollback storing User's data in the db, Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
								} catch (Exception ex2) {
									Log.Log(LogLevel.Error, "Error in rollback User's data in the db, Message: " + ex2.Message + ", StackTrace: " + ex2.StackTrace);
									throw;
								}
							}

							throw;
						}
					}
				}

				userInfo.UserId = userId;
				userInfo.EntityId = entityId;
				return true;
			} catch (Exception ex) {
				string errorMessage = "Error registerting user";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
			}

			return false;
		}

		private bool ValidateRegisterAccountParams(string accountName, string screenName, string foreignId, string accountAuthKey, string accountAuthKeySecret, DateTime? accountBirthDate, short? accountDayLimit, long entityId, long sourceId) {
			try {
				/* if (string.IsNullOrEmpty(accountName))
					throw new ArgumentException("Null or empty \"accountName\" parameter", "accountName");
				*/

				if (string.IsNullOrEmpty(foreignId))
					throw new ArgumentException("Null or empty \"foreignId\" parameter", "foreignId");

				if (string.IsNullOrEmpty(accountAuthKey))
					throw new ArgumentException("Null or empty \"accountAuthKey\" parameter", "accountAuthKey");

				if (string.IsNullOrEmpty(accountAuthKeySecret))
					throw new ArgumentException("Null or empty \"accountAuthKeySecret\" parameter", "accountAuthKeySecret");

				if (accountBirthDate.HasValue && accountBirthDate >= DateTime.UtcNow)
					throw new ArgumentOutOfRangeException("accountBirthDate", string.Format("The \"accountBirthDate\" parameter ({0}) is in the future", accountBirthDate.Value.ToString("yyyy-MM-dd HH:mm:ss")));

				if (accountDayLimit.HasValue && accountDayLimit < 0)
					throw new ArgumentOutOfRangeException("accountDayLimit", string.Format("The \"accountDayLimit\" parameter ({0}) has negative value", accountDayLimit));

				if (entityId < 0L)
					throw new ArgumentException(string.Format("The \"entityId\" parameter ({0}) has negative value", entityId), "entityId");

				switch (sourceId) {
					// case 1L: // For now, do not use RegisterAccount to register MMA Account records.
					case 3L: // Twitter
						break;
					default:
						throw new ArgumentException(string.Format("The \"sourceId\" parameter ({0}) is invalid", sourceId), "sourceId");
				}

				return true;
			} catch (Exception ex) {
				string errorMessage = "Error validating RegisterAccount params";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
				return false;
			}
		}

		public bool RegisterAccount(string accountName, string screenName, string foreignId, string accountAuthKey, string accountAuthKeySecret, DateTime? accountBirthDate, short? accountDayLimit, long entityId, long sourceId) {
			try {
				Log.Log(LogLevel.Debug, string.Format("RegisterAccount: accountName = \"{0}\", screenName = \"{1}\", foreignId = \"{2}\", accountAuthKey = \"{3}\", accountAuthKeySecret = \"{4}\", accountBirthDate = \"{5}\", accountDayLimit = {6}, entityId = {7}, sourceId = {8}", accountName, screenName, foreignId, accountAuthKey, accountAuthKeySecret, accountBirthDate, accountDayLimit, entityId, sourceId));

				if (!ValidateRegisterAccountParams(accountName, screenName, foreignId, accountAuthKey, accountAuthKeySecret, accountBirthDate, accountDayLimit, entityId, sourceId)) {
					string errorMessage = "Error validating RegisterAccount params";
					Log.Log(LogLevel.Error, errorMessage);
					throw new ApplicationException(errorMessage);
				}

				#region Get existing Entity info
				Ent ent = new Ent();
				object[,] entData = ent.Search(null, "getnmsnmbyid", new ArrayList {entityId}, 0L, -1L, null);

				if (entData == null)
					throw new ApplicationException(string.Format("Null data returned for Entity with ID = {0}", entityId));

				int entDataRowsCount = entData.GetLength(0);

				if (entDataRowsCount != 2)
					throw new ApplicationException(string.Format("Incorrect number of rows {0} (rather than 2) returned when searching for Entity with ID = {1}", entDataRowsCount, entityId));

				string entityName = Convert.ToString(entData[1, 0]);
				string entityScreenName = Convert.ToString(entData[1, 1]);
				#endregion

				#region Get existing Source info
				Src src = new Src();
				object[,] srcData = src.Search(null, "sctpnmbysrceid", new ArrayList {sourceId}, 0L, -1L, null);

				if (srcData == null)
					throw new ApplicationException(string.Format("Null data returned for Source with ID = {0}", sourceId));

				int srcDataRowsCount = srcData.GetLength(0);

				if (srcDataRowsCount != 2)
					throw new ApplicationException(string.Format("Incorrect number of rows {0} (rather than 2) returned when searching for Source with ID = {1}", srcDataRowsCount, entityId));

				string sourceTypeName = Convert.ToString(srcData[1, 0]);
				#endregion

				// For social networks, ignore the incoming account name param value, but auto-generate it instead.
				accountName = entityName + sourceTypeName; // The numeric suffix might be added later in Acct.AddNew() when looking for unique .nm values.

				if (string.IsNullOrEmpty(screenName))
					screenName = entityScreenName;

				#region Set Account Data
				Acct acct = new Acct();
				Dictionary<string, object> newAccountData = new Dictionary<string, object>(10);
				newAccountData["nodenm"] = "acct";

				if (accountBirthDate.HasValue)
					newAccountData["nodebirthd"] = accountBirthDate;

				newAccountData["acctnm"] = accountName;
				newAccountData["acctsnm"] = screenName;
				newAccountData["acctforeignid"] = foreignId;
				newAccountData["acctentity_enttid"] = entityId;
				newAccountData["acctsource_srceid"] = sourceId;
				newAccountData["acctauth"] = accountAuthKey;
				newAccountData["acctsecret"] = accountAuthKeySecret;

				if (accountDayLimit.HasValue)
					newAccountData["acctdaylimit"] = accountDayLimit;
				#endregion

				AeyeConnection aeyeDbConn = new AeyeConnection();

				using (IDbConnection dbConn = aeyeDbConn.CreateDbConnection()) {
					dbConn.Open();
					IDbTransaction tr = null;

					using (tr = dbConn.BeginTransaction()) {
						try {
							acct.AddNew(null, "default", newAccountData, dbConn);
							tr.Commit();
						} catch (Exception ex) {
							Log.Log(LogLevel.Error, "Error storing Account's data in the db, Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);

							if (tr != null) {
								try {
									tr.Rollback();
									Log.Log(LogLevel.Error, "Rollback storing Account's data in the db, Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
								} catch (Exception ex2) {
									Log.Log(LogLevel.Error, "Error in rollback Account's data in the db, Message: " + ex2.Message + ", StackTrace: " + ex2.StackTrace);
									throw;
								}
							}

							throw;
						}
					}
				}

				return true;
			} catch (Exception ex) {
				string errorMessage = "Error registering account";
				Log.Log(LogLevel.Error, errorMessage + ", Message: " + ex.Message + ", StackTrace: " + ex.StackTrace);
			}

			return false;
		}

		public void ImportSubs (Dictionary<string, object> dargs, long entid, System.Data.IDbConnection dbConn)
		{
			string filename;
			string separator = (dargs.ContainsKey("separator") ) ? Convert.ToString(dargs.ContainsKey("separator")) : "\t";
			// if(!dargs.ContainsKey("separator") ) string separator = "" + '\t';

			if (entid < 0) { // default
				// FIXME: entid = this.id; - If none -> fail
				// FIXME: RegisterUser() -> get id (if use not found in db, would expect args to contain the params for this method to add to the db )
			} else {
				//TODO: search for the id and check
				// FIXME: (we should probably default to search 'nm' for match of the filename (ignoring file postfix)
				// now just assume it's right
			}

			if ( dargs.Count < 1 ){ // TODO: fix if console will prompt here
				//Console.WriteLine ("Enter followers import file and optional delimiter (default is tab)");
				filename = "/home/aeye/importfile.tsv";
				separator = "\t";
				//Console.WriteLine ("<followers.tsv>?:");
				// string cms = Console.ReadLine ();
				//if (!string.IsNullOrEmpty (cms.Trim()))
				//	filename = cms;
			}
			else {
				filename = (string)dargs["filename"];
				separator = (string)dargs["separator"];
			}

			CsvParserLines csvl=null;
			Ent ent = null;
			Doc doc = null;
			Edg edg = null;
			Acct acc = null;
			Geo geo = null;
			Lang lang = null;
			EntInfo info = null;
			string alias = null;
			string criterion = "default";
			string[] line = {};
			int prop = 0;
			string[] props;

			try {
				csvl = new CsvParserLines( filename, separator );
				ent = new Ent();
				doc = new Doc ();
				edg = new Edg ();
				acc = new Acct ();
				geo = new Geo ();
				lang = new Lang ();
				info = new EntInfo ();
			} catch (Exception ex) {
				Console.WriteLine ("Couldn't initiallise import of file: {0}\n error = {1}\nstacktrace=:", filename, ex.Message, ex.StackTrace);
				System.Environment.Exit(1);
			}

			props = csvl [0];
			for(int i = 1; i < csvl.Count; i++){
				try { //parse a line

					line = csvl [i];

					#region create bags for each follower/fan entry/import
					// TODO: leave dicts here for debug, but move one above later
					Dictionary<string, object> entNewData = new Dictionary<string, object>();
					Dictionary<string, object> infoNewData = new Dictionary<string, object>();
					Dictionary<string, object> langNewData = new Dictionary<string, object>();
					Dictionary<string, object> edgNewData = new Dictionary<string, object>();
					Dictionary<string, object> accNewData = new Dictionary<string, object>();
					Dictionary<string, object> geoNewData = new Dictionary<string, object>();
					Dictionary<string, object> docNewData = new Dictionary<string, object>();


					for (prop=0; prop < props.Length; prop++){

						line[prop] = ((string)line[prop]).Trim();

						switch (props[prop]){


							case "statusesCount":
							case "followersCount":
							case "favoritesCount":
							case "friendsCount":
							case "listedCount":
							edgNewData [props [prop]] = Int32.Parse(line[prop]); 
							break;

							case "description":
							// string : nodedat
							entNewData["nodedat"] = line[prop];
							break;

							case "verified":
							float verif = line[prop] == "TRUE" ?  1.0f: 0.0f;  
							accNewData["nodevld"] = verif;
							entNewData ["nodevld"] = verif/2; 
							break;

							case "url":
							// add new val to Entinfo
							infoNewData["eninval"] = line[prop];
							break;

							case "location":
							// rawaddress
							infoNewData["rawaddress"] = line[prop];
							break;

							case "lang":
							langNewData[props[prop]] = line[prop];
							break;

							case "profileImageUrl":
							docNewData["docudat"] = line[prop];
							break;

							case "name":
							// this is the user-supplied name
							// 1. create a modified Ent.snm in some way
							entNewData["enttsnm"] = "tw." + line[prop];
							// 2. write to field acctsnm in the acct dict
							accNewData["acctsnm"] = line[prop];
							break;

							case "screenName":
							// this is the twitter unique nm
							// 1. create a modified Ent.nm
							entNewData["enttnm"] = "tw." + line[prop];
							// 2. write to field acctnm
							accNewData["acctnm"] = line[prop];
							break;

							case "id":
							// write to field acctforeignid
							accNewData["acctforeignid"] = Int64.Parse(line[prop]);
							break;

							case "created":
							DateTime date = DateTime.Parse(line[prop], new CultureInfo("en-US", false));
							accNewData["nodebirthd"] = date; // TODO DateTime convert 
							break;

							case "geolcntry":
							case "geolb":
							case "locality":
							case "area_2":
							case "area_1":
							geoNewData[props[prop]] = line[prop];
							break;

							//default:
							// TODO: error unrecognised column name

						} //switch prop
					} //for props

					#endregion // creat bags

					#region geo
					string sb = (string)geoNewData["geolb"];
					geoNewData["geolzip"] = "NA";
					geoNewData["geoltz"] = "NA";
					geoNewData["geoltyp_gltpid"] = Geo.GetTypId("unknown"); // TODO: guess better approximation from other address/geob info
					geoNewData["nodenm"] = "geo."+entNewData["enttnm"];
					geoNewData["nodetyp_ndtpid"] = Nd.GetTypId("geo");

					float xll, yll, xur, yur;

					if(!string.IsNullOrEmpty(sb)) {
						string[] sbbox = sb.Split(',');
						xll = float.Parse((string)sbbox[0]);
						yll = float.Parse((string)sbbox[1]); //, CultureInfo.InvariantCulture.NumberFormat);
						xur = float.Parse((string)sbbox[2]);
						yur = float.Parse((string)sbbox[3]);

						geoNewData["geolx"] = (xll + xur) / 2F; // FIXME: (i think these need to be in meters? check )
						geoNewData["geoly"] = (yll + yur) / 2F; // FIXME:
						geoNewData["geolr"] = (yur - yll) * 60 * 1852/2F; // FIXME: rough meters
						//geoNewData["geolz"] = 0F; //TODO: check/decide default

						geoNewData["geolnm"] = "" + geoNewData["locality"] + "." + geoNewData["area_2"]; // TODO: create unique geo nm from cols locality, area_1, area_2 (props)
						geoNewData["nodenm"] = "nd." + geoNewData["geolnm"];
						// FIXME: (consider using: app.mma.RDISplImplRWM.GenerateNextUniqueDbFieldValue )

						long geo_id = geo.AddNew (alias, criterion, geoNewData, dbConn); // TODO: fix params
						entNewData["geolid"] = geo_id;				}
					else{
						xll = yll = xur = yur = 0;
					}
					//for( int j = 0; j < sbbox.Length; j++) sbbox[j] = sbbox[j].Trim(); 

					//double xll, yll, xur, yur;
					//xur = Double.TryParse((string)sbbox[2]); //, CultureInfo.InvariantCulture.NumberFormat);
					//yur = Double.TryParse((string)sbbox[3]); //, CultureInfo.InvariantCulture.NumberFormat);

					// TODO: Ent.Nd.Geo = if Geo regions exists within 80%, use existing Geo.id. Else, create new Geo and get Geo.id
					#endregion // geo

					#region ent pic: create doc
					// nm default = srcTYpeName+nm+profilepic
					// case "enttpic_docuid":
					// 1. add doc
					docNewData["docuconttyp"] = "pic";
					docNewData["docunm"] = "pic.prof.tw." + accNewData["acctsnm"] + ".0"; // FIXME: add next sequence
					docNewData["nodenm"] = "doc." + accNewData["acctsnm"] + ".0"; // FIXME: add next sequence
					long doc_id = doc.AddNew(alias, criterion, docNewData, dbConn);
					// 2. add ent doc id to ent newData
					entNewData["enttpic_docuid"] = doc_id;
					#endregion // ent pic: create doc

					#region lang_id
					long lang_id = 0;
					ArrayList critParams = new ArrayList {langNewData["lang"]};
					object[,] resultData = lang.Search(alias, "idbynm", critParams, 0L, -1L, null);
					if(resultData.Length == 1){
						lang_id = (long)resultData[1,0]; //TODO: check
					}
					else if( resultData.Length == 0) {
						// if doesn't exist, add new lang
						lang_id = lang.AddNew(null, "default", langNewData, dbConn);
					}
					entNewData["enttlang_langid"] = lang_id;

					#endregion // lang_id

					#region ent
					long ent_id = ent.AddNew(alias, criterion, entNewData, dbConn);
					#endregion // ent

					#region account
					accNewData["acctentity_enttid"] = ent_id;
					accNewData["acctsource_srceid"] = 1L; // FIXME: get source id from db, currently hardcoded to tw id
					acc.AddNew(alias, criterion, accNewData, dbConn);
					#endregion //account


					#region edge
					// TODO: move to main loop
					edgNewData["edgeprov_nodeid"] = ent_id;
					edgNewData["edgenm"] = "st." + ent_id.ToString();
					edgNewData["edgetyp_edtpid"] = Edg.GetTypId("statusesCount"); 
					edgNewData["edgesub_nodeid"] = edgNewData["statusesCount"];
					edg.AddNew(alias, criterion,edgNewData, dbConn);

					edgNewData["edgenm"] = "fv." + ent_id.ToString();
					edgNewData["edgetyp_edtpid"] = Edg.GetTypId("favoritesCount"); //12;
					edgNewData["edgesub_nodeid"] = edgNewData["favoritesCount"];
					edg.AddNew(alias, criterion,edgNewData, dbConn);

					edgNewData["edgenm"] = "fr." + ent_id.ToString();
					edgNewData["edgetyp_edtpid"] = Edg.GetTypId("friendsCount"); //13; 
					edgNewData["edgesub_nodeid"] = edgNewData["friendsCount"];
					edg.AddNew(alias, criterion,edgNewData, dbConn);

					edgNewData["edgenm"] = "fo." + ent_id.ToString();
					edgNewData["edgetyp_edtpid"] = Edg.GetTypId("followersCount"); // 11; 
					edgNewData["edgesub_nodeid"] = edgNewData["followersCount"];
					edg.AddNew(alias, criterion,edgNewData, dbConn);

					edgNewData["edgenm"] = "ls." + ent_id.ToString();
					edgNewData["edgetyp_edtpid"] = Edg.GetTypId("listedCount"); //14; 
					edgNewData["edgesub_nodeid"] = edgNewData["listedCount"];
					edg.AddNew(alias, criterion,edgNewData, dbConn);
					#endregion //edge

					#region eninentityinfo
					infoNewData["eninentity_enttid"] = ent_id;
					// url
					string url = (string)infoNewData["eninval"];
					if(url != "NA" && !string.IsNullOrEmpty(url)){
						infoNewData["enininfotype_intpid"] = 11; 
						info.AddNew(alias, criterion, infoNewData, dbConn);
					}
					// unverified address
					string rawadd = (string)infoNewData["rawaddress"];
					if(rawadd != "NA" && !string.IsNullOrEmpty(rawadd) ){
						infoNewData["enininfotype_intpid"] = 9; 
						infoNewData["eninval"] = infoNewData["rawaddress"];
						info.AddNew(alias, criterion, infoNewData, dbConn);
					}
					#endregion
				} catch (Exception ex) {
					Log.Log(LogLevel.Debug, "error parsing line: {0}, column: {1}, value {2}\n{3}", i, props[prop], line[prop], ex.Message);
					// TODO: need to rollback on record per record basis (problem, testing, we rollback all)
				}

			} // for lines
		}// ImportSubs


		public void ImportFrFollFile(Dictionary<string, object> dargs, long entid) {

			Aeye.Framework.AeyeConnection aeyeDbConn = new Aeye.Framework.AeyeConnection();

			bool rolledback = false; // FIXME: rollback always while debug
			IDbTransaction tr = null;

			using (IDbConnection dbConn = aeyeDbConn.CreateDbConnection()) {
				dbConn.Open();

				using (tr = dbConn.BeginTransaction()) {
					try {
						ImportSubs(dargs, entid, dbConn);

						// FIXME: **** DEBUG ONLY: **** add when debugged:  tr.Commit();
					} catch (Exception ex) {
						Console.WriteLine("Error: " + ex.Message);

						if (tr != null) {
							try {
								tr.Rollback();
								rolledback = true;
							} catch (Exception ex2) {
								Console.WriteLine("Error2: " + ex2.Message);
							}
						}
					}
				}
			}
			if(!rolledback && tr != null) tr.Rollback (); // TODO: **** DEBUG ONLY ****  FIXME WHEN DEBUGGED

		} // ImportFrFollFile


	}
}
