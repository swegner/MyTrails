namespace MyTrails.Contracts.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Driving directions between a <see cref="Address"/> and <see cref="Trail"/> location.
    /// </summary>
    public class DrivingDirections
    {
        /// <summary>
        /// Datastore ID of the driving directions.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The driving time between the locations.
        /// </summary>
        public TimeSpan DrivingTime { get; set; }

        /// <summary>
        /// The starting address for the directions.
        /// </summary>
        [Required]
        public virtual Address Address { get; set; }

        /// <summary>
        /// The trail destination for the directions.
        /// </summary>
        [Required]
        public virtual Trail Trail { get; set; }
    }
}
