namespace MyTrails.Importer.Wta
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Trail definition from the WTA website.
    /// </summary>
    public class WtaTrail
    {
        /// <summary>
        /// Unique ID for the trail.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Uid",
            Justification = "Name required for deserialization contract.")]
        public string Uid { get; set; }

        /// <summary>
        /// Trail title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// WTA link for the trail.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// User rating for the trail.
        /// </summary>
        public double Rating { get; set; }

        /// <summary>
        /// The trail location.
        /// </summary>
        public WtaLocation Location { get; set; }
        
        /// <summary>
        /// Statistics for the trail.
        /// </summary>
        public WtaStatistics Statistics { get; set; }

        /// <summary>
        /// Guidebook reference featuring this trail.
        /// </summary>
        public WtaGuideBook GuideBook { get; set; }

        /// <summary>
        /// Any required pass for the trail.
        /// </summary>
        public string RequiredPass { get; set; }

        /// <summary>
        /// List of photo URLs for the hike.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "Class only used for deserialization."), 
        SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists",
            Justification = "Class only used for deserialization.")]
        public List<Uri> Photos { get; set; }
    }
}