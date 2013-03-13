namespace MyTrails.Contracts.Data
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Characteristic attributes which describe a trail.
    /// </summary>
    public class TrailCharacteristic
    {
        /// <summary>
        /// Construct a new <see cref="TrailCharacteristic"/> instance.
        /// </summary>
        public TrailCharacteristic()
        {
            this.Trails = new Collection<Trail>();
        }

        /// <summary>
        /// ID of the characteristic in the datastore.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID of the associated WTA UserInfo.
        /// </summary>
        public int WtaId { get; set; }

        /// <summary>
        /// Characteristic description.
        /// </summary>
        [Required, MaxLength(40)]
        public string Description { get; set; }

        /// <summary>
        /// Trails which have the associated characteristic.
        /// </summary>
        public virtual ICollection<Trail> Trails { get; private set; }

        /// <summary>
        /// Retrive a string representation of the characteristic.
        /// </summary>
        /// <returns>A string representation of the characteristic.</returns>
        /// <seealso cref="object.ToString"/>
        public override string ToString()
        {
            return this.Description;
        }
    }
}
