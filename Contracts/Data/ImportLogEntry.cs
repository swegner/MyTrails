namespace MyTrails.Contracts.Data
{
    using System;

    /// <summary>
    /// Log entry for an import run.
    /// </summary>
    public class ImportLogEntry
    {
        /// <summary>
        /// Data store ID of the log entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Start time of the run.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Time that the run completed.
        /// </summary>
        public DateTime CompletedTime { get; set; }

        /// <summary>
        /// Number of trails in the datastore at the start of the import.
        /// </summary>
        public int StartTrailsCount { get; set; }

        /// <summary>
        /// Numberof trails in the datastore on completion.
        /// </summary>
        public int CompletedTrailsCount { get; set; }

        /// <summary>
        /// Number of trip reports in the datastore at the start of the import.
        /// </summary>
        public int StartTripReportsCount { get; set; }

        /// <summary>
        /// Number of trip reports in the datastore on completion.
        /// </summary>
        public int CompletedTripReportsCount { get; set; }

        /// <summary>
        /// Number of errors encountered while importing new or updated trails.
        /// </summary>
        public int ErrorsCount { get; set; }
        
        /// <summary>
        /// Exception string from execution, or null if no errors were encountered.
        /// </summary>
        public string ErrorString { get; set; }
    }
}
