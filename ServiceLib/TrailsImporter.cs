namespace MyTrails.ServiceLib
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using log4net;
    using Microsoft.Practices.TransientFaultHandling;
    using MyTrails.Contracts.Data;
    using MyTrails.DataAccess;
    using MyTrails.ServiceLib.Extenders;
    using MyTrails.ServiceLib.Wta;

    /// <summary>
    /// Imports trails from WTA into the MyTrails data store.
    /// </summary>
    [Export(typeof(ITrailsImporter))]
    public class TrailsImporter : ITrailsImporter
    {
        /// <summary>
        /// Cumulative number of errors encountered while importing new or updated trails.
        /// </summary>
        private int _numImportErrors;

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
        /// Configuration for the trails importer.
        /// </summary>
        [Import]
        public IImporterConfiguration Configuration { get; set; }

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

            string exceptionString = null;
            const string errorStringFormat = "Errors encountered during execution:\n{0}";
            int? logEntryId = null;
            try
            {
                this.LogConnectionString();
                logEntryId = this.CreateImportLog();
                await this.RunInternal(logEntryId.Value);
            }
            catch (AggregateException ae)
            {
                exceptionString = string.Join(Environment.NewLine, ae.Flatten().InnerExceptions);
                this.Logger.ErrorFormat(errorStringFormat, exceptionString);
                throw;
            }
            catch (Exception ex)
            {
                exceptionString = ex.ToString();
                this.Logger.ErrorFormat(errorStringFormat, exceptionString);
                throw;
            }
            finally
            {
                if (logEntryId.HasValue)
                {
                    this.FinalizeAndCommitLog(logEntryId.Value, exceptionString);
                }
            }

            this.Logger.Info("Done!");
        }

        /// <summary>
        /// Log the connection string for debugging.
        /// </summary>
        private void LogConnectionString()
        {
            using (MyTrailsContext context = new MyTrailsContext())
            {
                string connectionString = context.Database.Connection.ConnectionString;
                this.Logger.DebugFormat("Using connection string: {0}", connectionString);
            }
        }

        /// <summary>
        /// Create a new <see cref="ImportLogEntry"/> for the import run.
        /// </summary>
        /// <returns>The ID of the newly created <see cref="ImportLogEntry"/>.</returns>
        private int CreateImportLog()
        {
            int importLogId;
            using (MyTrailsContext context = new MyTrailsContext())
            {
                ImportLogEntry logEntry = new ImportLogEntry
                {
                    StartTime = DateTime.Now,
                    StartTrailsCount = context.Trails.Count(),
                    StartTripReportsCount = context.TripReports.Count(),
                };

                context.ImportLog.Add(logEntry);
                context.SaveChanges();

                importLogId = logEntry.Id;
            }

            return importLogId;
        }

        /// <summary>
        /// Add completion statistics to the import log and save it to the datastore.
        /// </summary>
        /// <param name="logEntryId">The ID log entry to finalize.</param>
        /// <param name="errorString">An error string to associate, or null if no error was found.</param>
        private void FinalizeAndCommitLog(int logEntryId, string errorString)
        {
            using (MyTrailsContext context = new MyTrailsContext())
            {
                ImportLogEntry logEntry = context.ImportLog.Find(logEntryId);

                logEntry.CompletedTrailsCount = context.Trails.Count();
                logEntry.CompletedTripReportsCount = context.TripReports.Count();
                logEntry.CompletedTime = DateTime.Now;
                logEntry.ErrorString = errorString;
                logEntry.ErrorsCount = this._numImportErrors;

                context.SaveChanges(this.Logger);
            }
        }

        /// <summary>
        /// Run the importer.
        /// </summary>
        /// <param name="logEntryId">The associated <see cref="ImportLogEntry"/> ID for the import run.</param>
        /// <returns>Task for asynchronous completion.</returns>
        private async Task RunInternal(int logEntryId)
        {
            using (CancellationTokenSource heartbeatTokenSource = new CancellationTokenSource())
            {
                Task heartbeatTask = this.SendHeartbeats(logEntryId, heartbeatTokenSource.Token);

                RetryPolicy policy = this.WtaClient.BuildRetryPolicy(this.Logger);
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

                heartbeatTokenSource.Cancel();
                await heartbeatTask;
            }
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
            try
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
            catch (Exception ex)
            {
                Interlocked.Increment(ref this._numImportErrors);
                this.Logger.ErrorFormat("Error importing trail '{0}': {1}", wtaTrail, ex);

                throw;
            }
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

        /// <summary>
        /// Attach heartbeats to the import log entry that the importer is running until cancellation is requested.
        /// </summary>
        /// <param name="entryId">Log entry ID to append heartbeats to.</param>
        /// <param name="token">Cancellation token to stop heartbeats.</param>
        /// <returns>Task for asynchronous runtime.</returns>
        private async Task SendHeartbeats(int entryId, CancellationToken token)
        {
            TimeSpan interval = this.Configuration.HeartbeatInterval;
            this.Logger.DebugFormat("Sending heartbeats at interval: {0}", interval);

            try
            {
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(interval, token);

                    using (MyTrailsContext context = new MyTrailsContext())
                    {
                        ImportLogEntry logEntry = context.ImportLog.Find(entryId);
                        logEntry.LastHeartbeat = DateTime.Now;

                        context.SaveChanges();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Cancellation requested.
            }

            this.Logger.Debug("Finished heartbeating.");
        }
    }
}
