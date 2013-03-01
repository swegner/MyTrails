namespace MyTrails.Importer
{
    using System.Data.Spatial;
    using System.Globalization;

    /// <summary>
    /// Static and extension methods for working with <see cref="DbGeography"/> objects.s
    /// </summary>
    public static class DbGeographyExt
    {
        /// <summary>
        /// Construct a new <see cref="DbGeography"/> object from a latitude and longitude
        /// coordinate in the default coordinate system.
        /// </summary>
        /// <param name="latitude">The latitude coordinate.</param>
        /// <param name="longitude">The longitude coordinate.</param>
        /// <returns>A new <see cref="DbGeography"/> for the specified point.</returns>
        public static DbGeography PointFromCoordinates(double latitude, double longitude)
        {
            int coordinateSystemId = DbGeography.DefaultCoordinateSystemId;

            return DbGeographyExt.PointFromCoordinates(latitude, longitude, coordinateSystemId);
        }

        /// <summary>
        /// Construct a new <see cref="DbGeography"/> object from a latitude and longitude
        /// coordinate.
        /// </summary>
        /// <param name="latitude">The latitude coordinate.</param>
        /// <param name="longitude">The longitude coordinate.</param>
        /// <param name="coordinateSystemId">The coordinate system to use.</param>
        /// <returns>A new <see cref="DbGeography"/> for the specified point.</returns>
        public static DbGeography PointFromCoordinates(double latitude, double longitude, 
            int coordinateSystemId)
        {
            string pointString = string.Format(CultureInfo.InvariantCulture, "POINT({0} {1})", longitude, latitude);
            return DbGeography.PointFromText(pointString, coordinateSystemId);
        }
    }
}
