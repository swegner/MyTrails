namespace MyTrails.Importer
{
    using System.Data.Entity;

    /// <summary>
    /// Extension methods for <see cref="DbSet{TEntity}"/> objects.
    /// </summary>
    internal static class DbSetExtensions
    {
        /// <summary>
        /// Truncate the table; remove all of its contents.
        /// </summary>
        /// <typeparam name="T">The model type for the <see cref="DbSet"/></typeparam>
        /// <param name="this">The <see cref="DbSet"/> to truncate.</param>
        public static void Truncate<T>(this DbSet<T> @this)
            where T : class
        {
            foreach (T elmt in @this)
            {
                @this.Remove(elmt);
            }
        }
    }
}
