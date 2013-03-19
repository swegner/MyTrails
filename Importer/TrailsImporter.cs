﻿namespace MyTrails.Importer
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Threading.Tasks;
    using log4net;
    using Microsoft.Practices.TransientFaultHandling;
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
            this.Logger.Debug("Importing new trails.");
            RetryPolicy policy = Wta.WtaClient.BuildWtaRetryPolicy(this.Logger);
            Task<IList<WtaTrail>> fetchTrailTask = policy.ExecuteAsync(() => this.WtaClient.FetchTrails());

            this.Logger.Info("Fetching existing trail IDs.");
            List<string> existingTrailIds;
            using (MyTrailsContext context = new MyTrailsContext())
            {
                existingTrailIds = context.Trails
                    .Select(t => t.WtaId)
                    .ToList();
            }

            IList<WtaTrail> wtaTrails = await fetchTrailTask;
            this.DeDupeWtaTrails(wtaTrails);

            IEnumerable<Tuple<WtaTrail, bool>> wtaTrailTuples = this.MatchExistingTrails(wtaTrails, existingTrailIds);

            this.Logger.Debug("Creating new trail entries.");
            Task[] trailTasks = wtaTrailTuples
                .Select(tt => this.ImportOrUpdateTrail(tt.Item1, tt.Item2))
                .ToArray();
            await Task.WhenAll(trailTasks);

            this.Logger.DebugFormat("Created {0} new trails.", trailTasks.Length);
        }

        /// <summary>
        /// Search and remove duplicates from the set of WTA trails.
        /// </summary>
        /// <param name="wtaTrails">Trails fetched from WTA.</param>
        private void DeDupeWtaTrails(ICollection<WtaTrail> wtaTrails)
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
        }

        /// <summary>
        /// Determine whether fetched WTA trails exist in the database.
        /// </summary>
        /// <param name="wtaTrails">Fetched trails from WTA.</param>
        /// <param name="existingTrailIds">Trail IDs from the WTA database.</param>
        /// <returns>Pairs of trails and a boolean of whether the trail exists in the database.</returns>
        private IEnumerable<Tuple<WtaTrail, bool>> MatchExistingTrails(IEnumerable<WtaTrail> wtaTrails, IEnumerable<string> existingTrailIds)
        {
            IEnumerable<Tuple<WtaTrail, bool>> trailTuples = wtaTrails
                .GroupJoin(existingTrailIds, wt => wt.Uid, id => id, (wt, ids) => Tuple.Create(wt, ids.Any()));

            return trailTuples;
        }

        /// <summary>
        /// Import a new <see cref="WtaTrail"/>, or update an existing one.
        /// </summary>
        /// <param name="wtaTrail">The <see cref="WtaTrail"/> to import or update.</param>
        /// <param name="exists">Whether the trail already exists in the database.</param>
        /// <returns>Task for asynchronous completion.</returns>
        private async Task ImportOrUpdateTrail(WtaTrail wtaTrail, bool exists)
        {
            int trailId;
            using (MyTrailsContext trailContext = new MyTrailsContext())
            {
                Trail trail;
                if (exists)
                {
                    trail = trailContext.Trails
                        .Where(t => t.WtaId == wtaTrail.Uid)
                        .First();
                    this.TrailFactory.UpdateTrail(trail, wtaTrail, trailContext);
                }
                else
                {
                    trail = this.TrailFactory.CreateTrail(wtaTrail, trailContext);
                    trailContext.Trails.Add(trail);
                }

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
