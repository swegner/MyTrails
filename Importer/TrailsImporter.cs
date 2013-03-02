namespace MyTrails.Importer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Threading.Tasks;
    using log4net;
    using MyTrails.Contracts.Data;
    using MyTrails.DataAccess;
    using MyTrails.Importer.Wta;

    /// <summary>
    /// Imports trails from WTA into the MyTrails data store.
    /// </summary>
    [Export(typeof(ITrailsImporter))]
    public class TrailsImporter : ITrailsImporter
    {
        /// <summary>
        /// The run mode for the importer.
        /// </summary>
        /// <seealso cref="ITrailsImporter.Modes"/>
        public ImportModes Modes { get; set; }

        /// <summary>
        /// Interface for communicating with WTA.
        /// </summary>
        [Import]
        public IWtaClient WtaClient { get; set; }

        /// <summary>
        /// Creates new <see cref="Trail"/> instances from an imported <see cref="WtaTrail"/>.
        /// </summary>
        [Import]
        public ITrailFactory TrailFactory { get; set; }

        /// <summary>
        /// Logging interface.
        /// </summary>
        [Import]
        public ILog Logger { get; set; }

        /// <summary>
        /// Import and update trails according to the configured <see cref="Modes"/>
        /// </summary>
        /// <returns>Task for asynchronous completion.</returns>
        /// <seealso cref="ITrailsImporter.Run"/>
        public async Task Run()
        {
            ImportModes modes = this.Modes;
            if (!Enum.IsDefined(typeof(ImportModes), modes) || modes == ImportModes.None)
            {
                throw new InvalidOperationException(string.Format("Invalid ImportModes specified: {0}.", modes));
            }

            this.Logger.DebugFormat("Running importer in mode: {0}.", modes);

            using (MyTrailsContext context = new MyTrailsContext())
            {
                IList<Trail> newTrails;
                if (modes.HasFlag(ImportModes.ImportOnly))
                {
                    this.Logger.Debug("Importing new trails.");
                    IList<WtaTrail> wtaTrails = await this.WtaClient.FetchTrails();
                    
                    this.Logger.DebugFormat("Deduping {0} imported against existing trails.", wtaTrails.Count);

                    List<WtaTrail> duplicates = wtaTrails
                        .GroupBy(wt => wt.Uid)
                        .Where(g => g.Count() > 1)
                        .SelectMany(g => g.Skip(1))
                        .ToList();
                    if (duplicates.Any())
                    {
                        this.Logger.WarnFormat("Encountered {0} duplicate{1} while importing data.", 
                            duplicates.Count,
                            duplicates.Count > 1 ? "s" : string.Empty);
                        foreach (WtaTrail dupe in duplicates)
                        {
                            wtaTrails.Remove(dupe);
                        }
                    }

                    List<string> importedIds = wtaTrails
                        .Select(wt => wt.Uid)
                        .ToList();

                    HashSet<string> existingTrailIds = new HashSet<string>(context.Trails
                        .Where(t => importedIds.Contains(t.WtaId))
                        .Select(t => t.WtaId));

                    this.Logger.Debug("Creating new trail entries.");
                    newTrails = wtaTrails
                        .Where(wt => !existingTrailIds.Contains(wt.Uid))
                        .Select(wt => this.TrailFactory.CreateTrail(wt, context.Regions))
                        .ToList();

                    this.Logger.DebugFormat("Created {0} new trails.", newTrails.Count);
                }
                else
                {
                    newTrails = new List<Trail>();
                }

                this.Logger.Debug("Adding new trails to database.");
                foreach (Trail trail in newTrails)
                {
                    context.Trails.Add(trail);
                }

                this.Logger.Debug("Saving changes.");
                context.SaveChanges();
            }
        }
    }
}
