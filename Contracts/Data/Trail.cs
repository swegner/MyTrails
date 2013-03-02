namespace MyTrails.Contracts.Data
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Spatial;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A hiking trail.
    /// </summary>
    public class Trail
    {
        /// <summary>
        /// Data store ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the hike.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The WTA ID of the trail.
        /// </summary>
        public string WtaId { get; set; }

        /// <summary>
        /// WTA link for the trail.
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
        /// Helper property for serializing <see cref="Url"/>.
        /// </summary>
        [Column("Uri")]
        [DebuggerHidden]
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings",
            Justification = "String helper needed for EntityFramework serialization.")]
        public string UrlString { get; set; }

        /// <summary>
        /// Geographic coordinates for the trail head.
        /// </summary>
        public DbGeography Location { get; set; }

        /// <summary>
        /// The geographic region of the trail.
        /// </summary>
        public SubRegion SubRegion { get; set; }

        /// <summary>
        /// WTA rating for the trail.
        /// </summary>
        public double WtaRating { get; set; }
    }
}
