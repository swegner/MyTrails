namespace MyTrails.Contracts.Data
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Trip type associated with a trip report.
    /// </summary>
    public class TripType
    {
        /// <summary>
        /// Construct a new <see cref="TripType"/> instance.
        /// </summary>
        public TripType()
        {
            this.Trips = new Collection<TripReport>();
        }

        /// <summary>
        /// Datastore ID of the trip type.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID of the associated WTA trip type.
        /// </summary>
        [Required, MaxLength(30)]
        public string WtaId { get; set; }

        /// <summary>
        /// Description of the trip type.
        /// </summary>
        [Required, MaxLength(50)]
        public string Description { get; set; }

        /// <summary>
        /// Trip reports with the associated trip type.
        /// </summary>
        public virtual ICollection<TripReport> Trips { get; private set; }

        /// <summary>
        /// Retrieve a string representation of the trip type.
        /// </summary>
        /// <returns>A string representation of the trip type.</returns>
        /// <seealso cref="object.ToString"/>
        public override string ToString()
        {
            return this.Description;
        }
    }
}
