namespace MyTrails.Contracts.Data
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Registered MyTrails user with address and profile information.
    /// </summary>
    public class User
    {
        /// <summary>
        /// The ID of the user in the datastore.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Unique username for the user.
        /// </summary>
        [Required, MaxLength(50)]
        public string UserName { get; set; }

        /// <summary>
        /// User home address.
        /// </summary>
        [Required]
        public virtual Address HomeAddress { get; set; }

        /// <summary>
        /// Retrieve a string representation of the user.
        /// </summary>
        /// <returns>A string representation of the user.</returns>
        /// <seealso cref="object.ToString"/>
        public override string ToString()
        {
            return this.UserName;
        }
    }
}
