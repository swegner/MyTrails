namespace MyTrails.Contracts.Data
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Trail categorization features.
    /// </summary>
    public class TrailFeature
    {
        /// <summary>
        /// Consruct a new <see cref="TrailFeature"/> instance.
        /// </summary>
        public TrailFeature()
        {
            this.Trails = new Collection<Trail>();
        }

        /// <summary>
        /// ID of the feature in the datastore.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Value of the associated WTA feature.
        /// </summary>
        public int WtaId { get; set; }

        /// <summary>
        /// Feature description.
        /// </summary>
        [Required, MaxLength(30)]
        public string Description { get; set; }

        /// <summary>
        /// Trails which have the associated feature.
        /// </summary>
        public virtual ICollection<Trail> Trails { get; private set; }

        /// <summary>
        /// Retrieve a string representation of the feature.
        /// </summary>
        /// <returns>A string representation of the feature.</returns>
        /// <seealso cref="object.ToString"/>
        public override string ToString()
        {
            return this.Description;
        }
    }
}
