namespace MyTrails.Contracts.Data
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;

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
        [Required, MaxLength(50)]
        public string Title { get; set; }

        /// <summary>
        /// Name of the author.
        /// </summary>
        [Required, MaxLength(50)]
        public string Author { get; set; }

        /// <summary>
        /// Trails contained within the book.
        /// </summary>
        public virtual ICollection<Trail> Trails { get; private set; }
    }
}
