namespace MyTrails.Importer.Composition
{
    using System.ComponentModel.Composition;
    using CommandLine;
    using log4net;
    using log4net.Core;

    /// <summary>
    /// MEF exports to make available for composition.
    /// </summary>
    public static class Exports
    {
        /// <summary>
        /// Exported <see cref="ILog"/> instance.
        /// </summary>
        [Export(typeof(ILog))]
        public static ILog Logger
        {
            get { return LogManager.GetLogger("MyTrails.Importer"); }
        }
    }
}
