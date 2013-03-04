namespace MyTrails.Contracts.Data
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Guidebook containing trails.
    /// </summary>
    public class Guidebook
    {
        /// <summary>
        /// Constructe a new <see cref="Guidebook"/> instance.
        /// </summary>
        public Guidebook()
        {
            this.Trails = new Collection<Trail>();
        }

        /// <summary>
        /// Datastore ID for the guide book.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Title for the guide book.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Name of the author.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Trails contained within the book.
        /// </summary>
        public virtual ICollection<Trail> Trails { get; private set; }
    }
}
