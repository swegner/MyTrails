namespace MyTrails.Importer.Wta
{
    /// <summary>
    /// Trail statistics from the WTA website.
    /// </summary>
    public class WtaStatistics
    {
        /// <summary>
        /// Round-trip mileage for the trail.
        /// </summary>
        public double? Mileage { get; set; }

        /// <summary>
        /// Total elevation gain for the trial.
        /// </summary>
        public double? ElevationGain { get; set; }

        /// <summary>
        /// The high point of the trail.
        /// </summary>
        public double? HighPoint { get; set; }

        /// <summary>
        /// Features for the trail.
        /// </summary>
        public WtaFeatures Features { get; set; }

        /// <summary>
        /// User info for the trail.
        /// </summary>
        public WtaUserInfo UserInfo { get; set; }
    }
}
