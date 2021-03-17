using System;

namespace app.mma.utils {
	internal class DateTimeUtils {
		/// <summary>
		/// Converts DateTime object to Unix timestamp format.
		/// </summary>
		/// <param name="dateTime">The DateTime object to convert to Unix timestamp format.</param>
		/// <returns>Returns Unix timestamp representation of the DateTime object.</returns>
		public static long ConvertToUnixTime(DateTime dateTime) {
			DateTime unixDateTimeZeroPoint = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			TimeSpan unixDateTime = dateTime - unixDateTimeZeroPoint;
			long unixTimeStamp = (long) unixDateTime.TotalSeconds;
			return unixTimeStamp;
		}

		/// <summary>
		/// Converts Unix timestamp to DateTime object.
		/// </summary>
		/// <param name="unixtime">The Unix time stamp you want to convert to DateTime.</param>
		/// <returns>Returns a DateTime representation of of the Unix timestamp.</returns>
		public static DateTime UnixTimeToDateTime(long unixTime) {
			DateTime unixDateTimeZeroPoint = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return unixDateTimeZeroPoint.AddSeconds(unixTime);
		}
	}
}
