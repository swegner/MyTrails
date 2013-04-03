﻿namespace MyTrails.ServiceLib
{
    using System;
    using System.ComponentModel.Composition;
    using System.Configuration;
    using System.Globalization;

    /// <summary>
    /// Configuration for the trails importer.
    /// </summary>
    [Export(typeof(IImporterConfiguration))]
    public class ImporterConfiguration : IImporterConfiguration
    {
        /// <summary>
        /// The interval at which to send heartbeats.
        /// </summary>
        /// <seealso cref="IImporterConfiguration.HeartbeatInterval"/>
        public TimeSpan HeartbeatInterval
        {
            get { return TimeSpan.Parse(ConfigurationManager.AppSettings["MyTrails.ServiceLib.HeartbeatInterval"], CultureInfo.InvariantCulture); }
        }

        /// <summary>
        /// Multiplier to apply when checking for recent heartbeats.
        /// </summary>
        /// <seealso cref="IImporterConfiguration.HeartbeatCheckMultiplier"/>
        public double HeartbeatCheckMultiplier
        {
            get { return double.Parse(ConfigurationManager.AppSettings["MyTrails.ServiceLib.HeartbeatCheckMultiplier"], CultureInfo.InvariantCulture); }
        }
    }
}