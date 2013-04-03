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

        /// <summary>
        /// Multiplier to apply when checking for recent heartbeats.
        /// </summary>
        double HeartbeatCheckMultiplier { get; }
    }
}
