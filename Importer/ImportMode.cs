namespace MyTrails.Importer
{
    using System;

    /// <summary>
    /// The run mode in which to run the importer.
    /// </summary>
    [Flags]
    public enum ImportMode
    {
        /// <summary>
        /// Default value, don't import.
        /// </summary>
        None = 0,

        /// <summary>
        /// Import new trails, don't update existing.
        /// </summary>
        ImportOnly = 1 << 0,

        /// <summary>
        /// Update existing trails only, don't import new trails.
        /// </summary>
        UpdateOnly = 1 << 1,

        /// <summary>
        /// Import new trails and update existing.
        /// </summary>
        ImportAndUpdate = ImportOnly | UpdateOnly,
    }
}