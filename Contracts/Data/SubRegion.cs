namespace MyTrails.Contracts.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// A geographic region located within a larger region.
    /// </summary>
    public class SubRegion
    {
        /// <summary>
        /// Construct a new <see cref="SubRegion"/> instance.
        /// </summary>
        public SubRegion()
        {
            this.Trails = new Collection<Trail>();
        }

        /// <summary>
        /// The ID of the subregion in the datastore.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// SubRegion GUID from the WTA website.
        /// </summary>
        public Guid WtaId { get; set; }

        /// <summary>
        /// Name of the subregion.
        /// </summary>
        [MaxLength(40)]
        public string Name { get; set; }

        /// <summary>
        /// The region which this subregion is contained in.
        /// </summary>
        public virtual Region Region { get; set; }

        /// <summary>
        /// List of trails contained within the subregion.
        /// </summary>
        public virtual ICollection<Trail> Trails { get; private set; } 
    }
}