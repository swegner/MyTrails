namespace MyTrails.ServiceLib.Wta
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Trip report from the WTA website.
    /// </summary>
    public class WtaTripReport
    {
        /// <summary>
        /// Construct a new <see cref="WtaTripReport"/>
        /// </summary>
        public WtaTripReport()
        {
            this.Photos = new List<Uri>();
            this.HikeIds = new List<string>();
        }

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

        /// <summary>
        /// List of photo URLs for the trip report.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "Class only used for deserialization."),
        SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists",
            Justification = "Class only used for deserialization.")]
        public List<Uri> Photos { get; set; }

        /// <summary>
        /// List of hike IDs that the trip report is associated with.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "Class only used for deserialization."),
        SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists",
            Justification = "Class only used for deserialization.")]
        public List<string> HikeIds { get; set; }
    }
}
