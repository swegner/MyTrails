namespace MyTrails.Importer.Wta
{
    using System;

    /// <summary>
    /// Location definition from the WTA website.
    /// </summary>
    public class WtaLocation
    {
        /// <summary>
        /// Latitude part of the coordinates.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// Longitude part of the coordinate.
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// WTA Region ID for the location.
        /// </summary>
        public Guid? RegionId { get; set; }
    }
}
