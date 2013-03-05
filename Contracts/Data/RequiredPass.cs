namespace MyTrails.Contracts.Data
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Pass requirement or fee associated with a hike.
    /// </summary>
    public class RequiredPass
    {
        /// <summary>
        /// Construct a new <see cref="RequiredPass"/> instance.
        /// </summary>
        public RequiredPass()
        {
            this.Trails = new Collection<Trail>();
        }

        /// <summary>
        /// The ID of the pass in the datastore.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Short-name of the pass requirement.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the pass requirement.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Trails that require the associated pass.
        /// </summary>
        public virtual ICollection<Trail> Trails { get; private set; }
    }
}
