namespace MyTrails.Contracts.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// A geographic region of Washington state.
    /// </summary>
    public class Region
    {
        /// <summary>
        /// Construct a new <see cref="Region"/> instance.
        /// </summary>
        public Region()
        {
            this.SubRegions = new Collection<Region>();
        }

        /// <summary>
        /// The ID of the region in the datastore.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// SubRegion GUID from the WTA website.
        /// </summary>
        public Guid WtaId { get; set; }

        /// <summary>
        /// Name of the region.
        /// </summary>
        [Required, MaxLength(40)]
        public string Name { get; set; }

        /// <summary>
        /// List of sub-regions for in region.
        /// </summary>
        public virtual ICollection<Region> SubRegions { get; private set; }
    }
}
