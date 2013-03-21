namespace MyTrails.ServiceLib.Wta
{
    using System;

    /// <summary>
    /// Trip report from the WTA website.
    /// </summary>
    public class WtaTripReport
    {
        /// <summary>
        /// Date the trip report was added.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Title for the trip report.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The trip report author.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// The type of hike for the report.
        /// </summary>
        public string HikeType { get; set; }

        /// <summary>
        /// URL for the full report.
        /// </summary>
        public Uri FullReportUrl { get; set; }

        /// <summary>
        /// The text of the trip report.
        /// </summary>
        public string BodyText { get; set; }
    }
}
