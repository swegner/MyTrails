namespace MyTrails.Contracts.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
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
        /// Construct a new <see cref="Trail"/> instance.
        /// </summary>
        public Trail()
        {
            this.Features = new Collection<TrailFeature>();
            this.Characteristics = new Collection<TrailCharacteristic>();
            this.TripReports = new Collection<TripReport>();
            this.PhotoLinks = new Collection<string>();
        }

        /// <summary>
        /// Data store ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the hike.
        /// </summary>
        [Required, MaxLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// The WTA ID of the trail.
        /// </summary>
        [Required, MaxLength(100)]
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
        [Required, Column("Url"), MaxLength(200)]
        [DebuggerHidden]
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings",
            Justification = "String helper needed for EntityFramework serialization.")]
        public string UrlString { get; set; }

        /// <summary>
        /// Geographic coordinates for the trail head.
        /// </summary>
        public DbGeography Location { get; set; }

        /// <summary>
        /// WTA rating for the trail.
        /// </summary>
        public double WtaRating { get; set; }

        /// <summary>
        /// The round-trip mileage for the hike.
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
        /// Datastore ID of the associated <see cref="Region"/>.
        /// </summary>
        public int? RegionId { get; set; }

        /// <summary>
        /// The geographic region of the trail.
        /// </summary>
        public virtual Region Region { get; set; }

        /// <summary>
        /// Datastore ID of the associated <see cref="Guidebook"/>
        /// </summary>
        public int? GuidebookId { get; set; }

        /// <summary>
        /// Book reference containing trail information.
        /// </summary>
        public virtual Guidebook Guidebook { get; set; }

        /// <summary>
        /// Datastore ID of the associated <see cref="RequiredPass"/>
        /// </summary>
        public int? RequiredPassId { get; set; }

        /// <summary>
        /// Required pass for the trail.
        /// </summary>
        public virtual RequiredPass RequiredPass { get; set; }

        /// <summary>
        /// Features associated with the trail.
        /// </summary>
        public virtual ICollection<TrailFeature> Features { get; private set; }

        /// <summary>
        /// Characteristics associated with the trail.
        /// </summary>
        public virtual ICollection<TrailCharacteristic> Characteristics { get; private set; }

        /// <summary>
        /// Collection of trip reports.
        /// </summary>
        public virtual ICollection<TripReport> TripReports { get; private set; } 

        /// <summary>
        /// Links to photos of the trail.
        /// </summary>
        public virtual ICollection<string> PhotoLinks { get; private set; }
    }
}
