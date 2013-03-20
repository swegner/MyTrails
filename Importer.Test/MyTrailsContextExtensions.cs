namespace MyTrails.Importer.Test
{
    using System;
    using MyTrails.DataAccess;

    /// <summary>
    /// Static and extension methods for working with <see cref="MyTrailsContext"/> instances.
    /// </summary>
    public static class MyTrailsContextExtensions
    {
        /// <summary>
        /// Remove all inserted data from the database, except seed data, to prepare for testing.
        /// </summary>
        /// <param name="this">The database context.</param>
        public static void ClearDatabase(this MyTrailsContext @this)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("this");
            }

            @this.Trails.Truncate();
            @this.Guidebooks.Truncate();
            @this.TripReports.Truncate();
            @this.ImportLog.Truncate();
        }
    }
}
