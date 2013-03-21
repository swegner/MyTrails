namespace MyTrails.Importer.Extenders
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Threading.Tasks;
    using log4net;
    using Microsoft.Practices.TransientFaultHandling;
    using MyTrails.Contracts.Data;
    using MyTrails.DataAccess;
    using MyTrails.Importer.Wta;
    using MyTrails.ServiceLib.Extenders;
    using MyTrails.ServiceLib.Wta;

    /// <summary>
    /// Associate trip reports with a trail.
    /// </summary>
    [Export(typeof(ITrailExtender))]
    public class TripReportExtender : ITrailExtender
    {
        /// <summary>
        /// Delay period to wait if a trip report is being added the the data store.
        /// </summary>
        private static readonly TimeSpan ConcurrentTripReportDelay = TimeSpan.FromMilliseconds(100);

        /// <summary>
        /// Lock object to synchronize initialization.
        /// </summary>
        private readonly object _initSyncObject;

        /// <summary>
        /// Synchronization dictionary to ensure trip reports are not duplicated on concurrent threads.
        /// A trip report is added to the dictionary when another thread begins to add it to the datastore.
        /// Other threads will wait for the datastore to be updated with the new trip report.
        /// </summary>
        private readonly ConcurrentDictionary<string, object> _tripReportDictionary;

        /// <summary>
        /// Dictionary of trip types, keyed by WTA ID.
        /// </summary>
        private Dictionary<string, int> _tripTypeDictionary; 

        /// <summary>
        /// Whether the maximum trip report date has been initialized.
        /// </summary>
        private bool _initialized;

        /// <summary>
        /// Construct a new <see cref="TripReportExtender"/> instance.
        /// </summary>
        public TripReportExtender()
        {
            this._initSyncObject = new object();
            this._tripReportDictionary = new ConcurrentDictionary<string, object>();
        }

        /// <summary>
        /// Interface for communicating with wta.org
        /// </summary>
        [Import]
        public IWtaClient WtaClient { get; set; }

        /// <summary>
        /// Logging interface.
        /// </summary>
        [Import]
        public ILog Logger { get; set; }

        /// <summary>
        /// Add trip reports to the trail.
        /// </summary>
        /// <param name="trail">The trail to extend.</param>
        /// <param name="context">Registered context.</param>
        /// <returns>Task for asynchronous completion.</returns>
        /// <seealso cref="ITrailExtender.Extend"/>
        public async Task Extend(Trail trail, MyTrailsContext context)
        {
            this.Initialize(context);

            string wtaTrailId = trail.WtaId;
            RetryPolicy policy = Wta.WtaClient.BuildWtaRetryPolicy(this.Logger);
            IList<WtaTripReport> reports = await policy.ExecuteAsync(() => this.WtaClient.FetchTripReports(wtaTrailId));

            foreach (WtaTripReport wtaReport in reports)
            {
                string wtaReportId = this.ParseWtaReportId(wtaReport);
                Lazy<bool> firstToAdd = new Lazy<bool>(() => this._tripReportDictionary.TryAdd(wtaReportId, null));
                TripReport report;
                do
                {
                    report = context.TripReports
                        .Where(tr => tr.WtaId == wtaReportId)
                        .FirstOrDefault();

                    if (report == null)
                    {
                        if (firstToAdd.Value)
                        {
                            // First thread to access new trip report, create it.
                            this.Logger.InfoFormat("Found new trip report: {0}", wtaReportId);
                            report = this.CreateReport(wtaReportId, wtaReport);
                        }
                        else
                        {
                            this.Logger.DebugFormat("Waiting for other thread to create trip report: {0}.", wtaReportId);
                            await Task.Delay(ConcurrentTripReportDelay);
                        }
                    }
                }
                while (report == null);

                trail.TripReports.Add(report);
            }
        }

        /// <summary>
        /// Initialize caches and the maximum date of previously stored trip reports.
        /// </summary>
        /// <param name="context">Datastore context..</param>
        private void Initialize(MyTrailsContext context)
        {
            if (!this._initialized)
            {
                lock (this._initSyncObject)
                {
                    if (!this._initialized)
                    {
                        this.Logger.Debug("Initializing trip type dictionary");
                        this._tripTypeDictionary = context.TripTypes.ToDictionary(tt => tt.WtaId, tt => tt.Id);

                        this._initialized = true;
                    }
                }
            }
        }

        /// <summary>
        /// Retrieve the WTA ID for the trip report.
        /// </summary>
        /// <param name="report">The report to retrieve the ID of.</param>
        /// <returns>The unique ID of the trip report.</returns>
        private string ParseWtaReportId(WtaTripReport report)
        {
            return report.FullReportUrl.Segments.Last();
        }

        /// <summary>
        /// Create a new <see cref="TripReport"/> from a <see cref="WtaTripReport"/>
        /// </summary>
        /// <param name="wtaReportId">The WTA ID extracted from the trip report.</param>
        /// <param name="wtaReport">The <see cref="WtaTripReport"/> used to build the new report object.</param>
        /// <returns>An initialized <see cref="TripReport"/>.</returns>
        private TripReport CreateReport(string wtaReportId, WtaTripReport wtaReport)
        {
            int tripTypeId = this._tripTypeDictionary[wtaReport.HikeType];

            return new TripReport
            {
                WtaId = wtaReportId,
                Title = wtaReport.Title,
                Author = wtaReport.Author,
                Date = wtaReport.Date,
                Url = wtaReport.FullReportUrl,
                TripTypeId = tripTypeId,
                Text = wtaReport.BodyText,
            };
        }
    }
}
