namespace MyTrails.ServiceLib
{
    using System;

    /// <summary>
    /// Configuration for the trails importer.
    /// </summary>
    public interface IImporterConfiguration
    {
        /// <summary>
        /// The interval at which to send heartbeats.
        /// </summary>
        TimeSpan HeartbeatInterval { get; }
    }
}
