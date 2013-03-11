namespace MyTrails.Importer.Extenders
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using log4net;
    using MyTrails.Contracts.Data;
    using MyTrails.DataAccess;
    using MyTrails.Importer.Wta;

    /// <summary>
    /// Associate trip reports with a trail.
    /// </summary>
    [Export(typeof(ITrailExtender))]
    public class TripReportExtender : ITrailExtender
    {
        /// <summary>
        /// Lock object to synchronize initialization.
        /// </summary>
        private readonly object _initSyncObject;

        /// <summary>
        /// Dictionary cache of looked-up trip reports.
        /// </summary>
        private readonly ConcurrentDictionary<string, TripReport> _cachedReports;

        /// <summary>
        /// Maximum date of previously stored trip reports.
        /// </summary>
        private DateTime _maxTripReportDate;

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
            this._cachedReports = new ConcurrentDictionary<string, TripReport>();
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
            this.InitializeMaxTripReportDate(context.TripReports);
            this.Logger.InfoFormat("Adding trip reports on or after {0}.", this._maxTripReportDate);

            string wtaTrailId = trail.WtaId;
            IList<WtaTripReport> reports = await this.WtaClient.FetchTripReports(wtaTrailId);

            IEnumerable<WtaTripReport> potentialReports = reports
                .Where(tr => tr.Date >= this._maxTripReportDate);

            foreach (WtaTripReport wtaReport in potentialReports)
            {
                string wtaReportId = this.GetWtaReportId(wtaReport);

                TripReport report;
                bool found = this._cachedReports.TryGetValue(wtaReportId, out report);
                if (!found)
                {
                    TripReport newReport = context.TripReports
                        .Where(tr => tr.WtaId == wtaReportId)
                        .FirstOrDefault();

                    if (newReport == null)
                    {
                        this.Logger.InfoFormat("Found new trip report: {0}", wtaReportId);
                        newReport = this.CreateReport(wtaReportId, wtaReport);
                    }

                    this._cachedReports.TryAdd(wtaReportId, newReport);
                    report = newReport;
                }

                trail.TripReports.Add(report);
            }
        }

        /// <summary>
        /// Initialize the maximum date of previously stored trip reports.
        /// </summary>
        /// <param name="tripReports">Registered trip reports.</param>
        private void InitializeMaxTripReportDate(DbSet<TripReport> tripReports)
        {
            if (!this._initialized)
            {
                lock (this._initSyncObject)
                {
                    if (!this._initialized)
                    {
                        this._maxTripReportDate = tripReports.Any() ?
                            tripReports.Max(tr => tr.Date) :
                            DateTime.MinValue;
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
        private string GetWtaReportId(WtaTripReport report)
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
            return new TripReport
            {
                WtaId = wtaReportId,
                Title = wtaReport.Title,
                Author = wtaReport.Author,
                Date = wtaReport.Date,
                Url = wtaReport.FullReportUrl,
                TripType = wtaReport.HikeType,
                Text = wtaReport.BodyText,
            };
        }
    }
}
