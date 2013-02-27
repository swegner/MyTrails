namespace MyTrails.Importer.Wta
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Guidebook reference from WTA.
    /// </summary>
    public class WtaGuideBook
    {
        /// <summary>
        /// Title for the guide book.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Name of the author.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Image link for the book cover image.
        /// </summary>
        public Uri CoverImage { get; set; }

        /// <summary>
        /// Merchants which carry this guidebook.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "Class only used for deserialization."),
        SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists",
            Justification = "Class only used for deserialization.")]
        public List<WtaGuideBookMerchant> Merchants { get; set; }
    }
}
