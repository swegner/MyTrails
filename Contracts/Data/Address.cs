namespace MyTrails.Contracts.Data
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Spatial;

    /// <summary>
    /// Home address location for a user.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Construct a new <see cref="Address"/> instance.
        /// </summary>
        public Address()
        {
            this.Directions = new Collection<DrivingDirections>();
        }

        /// <summary>
        /// ID of the address in the datastore.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The user-entered address location.
        /// </summary>
        [Required]
        public string Location { get; set; }

        /// <summary>
        /// The latitude/longitude coordinates of the location.
        /// </summary>
        [Required]
        public DbGeography Coordinate { get; set; }

        /// <summary>
        /// Driving directions to various trails.
        /// </summary>
        [Required]
        public virtual ICollection<DrivingDirections> Directions { get; private set; }
    }
}
