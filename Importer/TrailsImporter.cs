namespace MyTrails.Importer
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Threading.Tasks;
    using log4net;
    using MyTrails.Contracts.Data;
    using MyTrails.DataAccess;
    using MyTrails.Importer.Extenders;
    using MyTrails.Importer.Wta;

    /// <summary>
    /// Imports trails from WTA into the MyTrails data store.
    /// </summary>
    [Export(typeof(ITrailsImporter))]
    public class TrailsImporter : ITrailsImporter
    {
        /// <summary>
        /// Construct a new <see cref="TrailsImporter"/> instance.
        /// </summary>
        public TrailsImporter()
        {
            this.TrailExtenders = new Collection<ITrailExtender>();
        }

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
        /// CollectExtion of extenders which add additional trail context. 
        /// </summary>
        [ImportMany]
        public ICollection<ITrailExtender> TrailExtenders { get; private set; }

        /// <summary>
        /// Logging interface.
        /// </summary>
        [Import]
        public ILog Logger { get; set; }

        /// <summary>
        /// Import and update trails.
        /// </summary>
        /// <returns>Task for asynchronous completion.</returns>
        /// <seealso cref="ITrailsImporter.Run"/>
        public async Task Run()
        {
            this.Logger.Info("Fetching existing trail IDs");
            List<string> existingTrailIds;
            using (MyTrailsContext context = new MyTrailsContext())
            {
                existingTrailIds = context.Trails
                    .Select(t => t.WtaId)
                    .ToList();
            }

            Task importTask = this.ImportNewTrails(existingTrailIds);

            await Task.WhenAll(importTask);
        }

        /// <summary>
        /// Import new trails from WTA.
        /// </summary>
        /// <param name="existingTrailIds">WTA IDs of existing trails.</param>
        /// <returns>Task for asynchronous completion.</returns>
        private async Task ImportNewTrails(List<string> existingTrailIds)
        {
            this.Logger.Debug("Importing new trails.");
            IList<WtaTrail> wtaTrails = await this.WtaClient.FetchTrails();
            IEnumerable<WtaTrail> newWtaTrails = this.DeDupeWtaTrails(wtaTrails, existingTrailIds);

            this.Logger.Debug("Creating new trail entries.");
            Task[] trailTasks = newWtaTrails
                .Select(this.ImportNewTrail)
                .ToArray();
            await Task.WhenAll(trailTasks);

            this.Logger.DebugFormat("Created {0} new trails.", trailTasks.Length);
        }

        /// <summary>
        /// Search imported trails for duplicates, and return a unique set of new trails.
        /// </summary>
        /// <param name="wtaTrails">Trails fetched from WTA.</param>
        /// <param name="existingTrailIds">WTA IDs of existing trails.</param>
        /// <returns>Sequence of uniuqe, new trails.</returns>
        private IEnumerable<WtaTrail> DeDupeWtaTrails(ICollection<WtaTrail> wtaTrails, List<string> existingTrailIds)
        {
            this.Logger.DebugFormat("Deduping {0} imported against existing trails.", wtaTrails.Count);

            List<WtaTrail> duplicates = wtaTrails
                .GroupBy(wt => wt.Uid)
                .Where(g => g.Count() > 1)
                .SelectMany(g => g.Skip(1))
                .ToList();

            if (duplicates.Any())
            {
                this.Logger.WarnFormat("Encountered {0} duplicate{1} while importing data.", duplicates.Count, duplicates.Count > 1 ? "s" : string.Empty);
                foreach (WtaTrail dupe in duplicates)
                {
                    wtaTrails.Remove(dupe);
                }
            }

            HashSet<string> existingTrailIdSet = new HashSet<string>(existingTrailIds);
            IEnumerable<WtaTrail> newWtaTrails = wtaTrails
                .Where(wt => !existingTrailIdSet.Contains(wt.Uid));

            return newWtaTrails;
        }

        /// <summary>
        /// Import a new <see cref="WtaTrail"/>.
        /// </summary>
        /// <param name="wtaTrail">The <see cref="WtaTrail"/> to import.</param>
        /// <returns>Task for asynchronous completion.</returns>
        private async Task ImportNewTrail(WtaTrail wtaTrail)
        {
            int trailId;
            using (MyTrailsContext trailContext = new MyTrailsContext())
            {
                Trail trail = this.TrailFactory.CreateTrail(wtaTrail, trailContext);
                trailContext.Trails.Add(trail);

                trailContext.SaveChanges(this.Logger);
                trailId = trail.Id;
            }

            IEnumerable<Task> extenderTasks = this.TrailExtenders
                .Select(te => this.RunExtender(te, trailId));

            await Task.WhenAll(extenderTasks);
        }

        /// <summary>
        /// Execute a trail extender for a new trail.
        /// </summary>
        /// <param name="extender">The extender to run.</param>
        /// <param name="trailId">The trail ID to extend.</param>
        /// <returns>Task for asyncrhonous execution.</returns>
        private async Task RunExtender(ITrailExtender extender, int trailId)
        {
            using (MyTrailsContext trailContext = new MyTrailsContext())
            {
                Trail trail = trailContext.Trails.Find(trailId);
                await extender.Extend(trail, trailContext);

                trailContext.SaveChanges(this.Logger);
            }
        }
    }
}
