﻿namespace MyTrails.DataAccess
{
    using System.Data.Entity;

    using MyTrails.Contracts.Data;
    using MyTrails.DataAccess.Migrations;

    /// <summary>
    /// Data context for MyTrails data store access.
    /// </summary>
    public class MyTrailsContext : DbContext
    {
        /// <summary>
        /// Static initializer; set the database initialize for the context.
        /// </summary>
        static MyTrailsContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<MyTrailsContext, Configuration>());
        }

        /// <summary>
        /// Hiking trails available in the data store.
        /// </summary>
        public DbSet<Trail> Trails { get; set; }

        /// <summary>
        /// Washington state regions available in the datastore.
        /// </summary>
        public DbSet<Region> Regions { get; set; }

        /// <summary>
        /// Guidebooks containing trail information.
        /// </summary>
        public DbSet<Guidebook> Guidebooks { get; set; }
    }
}
