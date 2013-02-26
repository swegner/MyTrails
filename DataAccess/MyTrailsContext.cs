namespace MyTrails.DataAccess
{
    using System.Data.Entity;

    using MyTrails.Contracts.Data;

    /// <summary>
    /// Data context for MyTrails data store access.
    /// </summary>
    public class MyTrailsContext : DbContext
    {
        /// <summary>
        /// Hiking trails available in the data store.
        /// </summary>
        public DbSet<Trail> Trails { get; set; }
    }
}
