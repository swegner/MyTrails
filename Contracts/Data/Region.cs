namespace MyTrails.Contracts.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// A geographic region of Washington state.
    /// </summary>
    public class Region
    {
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
        /// Return a string representation of the region.
        /// </summary>
        /// <returns>A string representation of the region.</returns>
        /// <seealso cref="object.ToString"/>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
