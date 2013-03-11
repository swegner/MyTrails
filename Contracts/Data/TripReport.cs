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
    /// A trip report for a hike on one or more trails.
    /// </summary>
    public class TripReport
    {
        /// <summary>
        /// Construct a new <see cref="TripReport"/> instance.
        /// </summary>
        public TripReport()
        {
            this.Trails = new Collection<Trail>();
        }

        /// <summary>
        /// Report ID in the datastore.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Report ID from the WTA website.
        /// </summary>
        [Required]
        public string WtaId { get; set; }

        /// <summary>
        /// Date the trip report was added.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Title for the trip report.
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// The trip report author.
        /// </summary>
        [Required]
        public string Author { get; set; }

        /// <summary>
        /// The type of hike for the report.
        /// </summary>
        [Required]
        public string TripType { get; set; }

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
        /// Trails included in the trip report.
        /// </summary>
        public virtual ICollection<Trail> Trails { get; private set; } 
    }
}
