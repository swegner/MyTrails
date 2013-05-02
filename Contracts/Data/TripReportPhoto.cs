namespace MyTrails.Contracts.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A photo from a trip report.
    /// </summary>
    public class TripReportPhoto
    {
        /// <summary>
        /// Photo ID in the datastore.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// URL for the full report.
        /// </summary>
        [NotMapped]
        public Uri Url
        {
            get
            {
                return string.IsNullOrEmpty(this.UrlString) ? null : new Uri(this.UrlString);
            }

            set
            {
                this.UrlString = value == null ? null : value.AbsoluteUri;
            }
        }

        /// <summary>
        /// EF helper for <see cref="Url"/>
        /// </summary>
        [Required, Column("Uri")]
        [DebuggerHidden]
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings",
            Justification = "String helper needed for EntityFramework serialization.")]
        public string UrlString { get; set; }

        /// <summary>
        /// The text of the trip report.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Datastore ID of the associated <see cref="TripReport"/>.
        /// </summary>
        public int TripReportId { get; set; }

        /// <summary>
        /// The associated trip report.
        /// </summary>
        public virtual TripReport TripReport { get; set; }
    }
}
