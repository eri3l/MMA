using System;

namespace app.mma.utils {
	///<summary>
	/// Represents a point on the surface of a sphere. (The Earth is almost spherical).
	/// http://JanMatuschek.de/LatitudeLongitudeBoundingCoordinates
	///</summary>
	public class GeoLocation {
		/// <summary>
		/// The radius of the Earth, e.g. the average radius for a spherical approximation of the figure of the Earth is approximately 6371.01 kilometers.
		/// </summary>
		public const double EarthRadius = 6371.01D;
		private static readonly double MinLatitude = DegreeToRadian(-90d); // -PI/2
		private static readonly double MaxLatitude = DegreeToRadian(90d); // PI/2
		private static readonly double MinLongitude = DegreeToRadian(-180d); // -PI
		private static readonly double MaxLongitude = DegreeToRadian(180d); // PI

		/// <summary>
		/// Latitude in radians.
		/// </summary>
		private double radLat;

		/// <summary>
		/// Longitude in radians.
		/// </summary>
		private double radLon;

		/// <summary>
		/// Latitude in degrees.
		/// </summary>
		private double degLat;

		/// <summary>
		/// Longitude in degrees.
		/// </summary>
		private double degLon;

		private GeoLocation () {
		}

		private static double DegreeToRadian(double angle) {
			return Math.PI * angle / 180D;
		}

		private static double RadianToDegree(double angle) {
			return angle * (180D / Math.PI);
		}

		/// <param name="latitude">The latitude, in degrees.</param>
		/// <param name="longitude">The longitude, in degrees.</param>
		public static GeoLocation FromDegrees(double latitude, double longitude) {
			GeoLocation result = new GeoLocation();
			result.radLat = DegreeToRadian(latitude);
			result.radLon = DegreeToRadian(longitude);
			result.degLat = latitude;
			result.degLon = longitude;
			result.CheckBounds();
			return result;
		}

		/// <param name="latitude">The latitude, in radians.</param>
		/// <param name="longitude">The longitude, in radians.</param>
		public static GeoLocation FromRadians(double latitude, double longitude) {
			GeoLocation result = new GeoLocation();
			result.radLat = latitude;
			result.radLon = longitude;
			result.degLat = RadianToDegree(latitude);
			result.degLon = RadianToDegree(longitude);
			result.CheckBounds();
			return result;
		}

		private void CheckBounds() {
			if (this.radLat < MinLatitude || this.radLat > MaxLatitude)
				throw new ArgumentOutOfRangeException("radLat");

			if (this.radLon < MinLongitude || this.radLon > MaxLongitude)
				throw new ArgumentOutOfRangeException("radLon");
		}

		public double LatitudeInDegrees {
			get {
				return this.degLat;
			}
		}

		public double LongitudeInDegrees {
			get {
				return this.degLon;
			}
		}

		public double LatitudeInRadians {
			get {
				return this.radLat;
			}
		}

		public double LongitudeInRadians {
			get {
				return this.radLon;
			}
		}

		public override string ToString() {
			return string.Format("({0} \u00B0, {1} \u00B0) = ({2} rad, {3} rad)", this.degLat, this.degLon, this.radLat, this.radLon);
		}
		/// <summary>
		/// Computes the great circle distance between this GeoLocation instance and the location argument.
		/// </summary>
		/// <param name="location"></param>
		/// <returns>The distance, measured in the same unit as the radius.</returns>
		public double DistanceTo(GeoLocation location) {
			return Math.Acos(Math.Sin(this.radLat) * Math.Sin(location.radLat) +
				Math.Cos(this.radLat) * Math.Cos(location.radLat) * Math.Cos(this.radLon - location.radLon)) * EarthRadius;
		}
		
		/// <summary>
		/// Computes the bounding coordinates of all points on the surface of a sphere that have a great circle distance to the point represented by this GeoLocation instance
		/// that is less or equal to the distance argument.
		/// For more information about the formulae used in this method visit http://JanMatuschek.de/LatitudeLongitudeBoundingCoordinates.
		/// </summary>
		/// <param name="distance">Distance the distance from the point represented by this GeoLocation instance. Must me measured in the same unit as the EarthRadius.</param>
		/// <param name="simplifiedLongitudeCalcs">If set to True, then the Bounding Min/Max Longitude calcs are simplified using the "small circle" (http://janmatuschek.de/LatitudeLongitudeBoundingCoordinates#LongitudeIncorrect). The result is approxiate.
		/// If set to False, then the Bounding Min/Max Longitude calcs are not simplified - they use the "great circle" (http://janmatuschek.de/LatitudeLongitudeBoundingCoordinates#Longitude)</param>
		/// <returns>An array of two GeoLocation objects such that:
		/// 
		///		* The latitude of any point within the specified distance is greater or equal to the latitude of the first array element and smaller or equal to the latitude
		///		of the second array element.
		///		* If the longitude of the first array element is smaller or equal to the longitude of the second element, then the longitude of any point within the specified distance
		///		is greater or equal to the longitude of the first array element and smaller or equal to the longitude of the second array element.
		///		* If the longitude of the first array element is greater than the longitude of the second element (this is the case if the 180th meridian is within the distance), then
		///		the longitude of any point within the specified distance is greater or equal to the longitude of the first array element OR smaller or equal to the longitude of the second array element.
		///	</returns>
		public GeoLocation[] GetBoundingCoordinates(double distance, bool simplifiedLongitudeCalcs) {
			if (distance < 0D)
				throw new ArgumentOutOfRangeException("distance");

			// Angular distance in radians on a great circle.
			double radDist = distance / EarthRadius;
			double minLat = radLat - radDist;
			double maxLat = radLat + radDist;
			double minLon, maxLon;

			if (minLat > MinLatitude && maxLat < MaxLatitude) {
				double deltaLon;
				
				if (simplifiedLongitudeCalcs)
					deltaLon = (radDist / Math.Cos(this.radLat));
				else
					deltaLon = Math.Asin(Math.Sin(radDist) / Math.Cos(this.radLat));

				minLon = this.radLon - deltaLon;

				if (minLon < MinLongitude)
					minLon += 2D * Math.PI;

				maxLon = this.radLon + deltaLon;

				if (maxLon > MaxLongitude)
					maxLon -= 2D * Math.PI;
			} else {
				// A pole is within the distance.
				minLat = Math.Max(minLat, MinLatitude);
				maxLat = Math.Min(maxLat, MaxLatitude);
				minLon = MinLongitude;
				maxLon = MaxLongitude;
			}

			return new GeoLocation[] {FromRadians(minLat, minLon), FromRadians(maxLat, maxLon)};
		}
	}
}
