namespace MyTrails.Importer.Extenders
{
    using System.Threading.Tasks;
    using MyTrails.Contracts.Data;

    /// <summary>
    /// Extends trail definitions by adding additional context.
    /// </summary>
    public interface ITrailExtender
    {
        /// <summary>
        /// Add additional context to the trail.
        /// </summary>
        /// <param name="trail">The trail to extend.</param>
        /// <param name="context">Registered context.</param>
        /// <returns>Task for asynchronous completion.</returns>
        Task Extend(Trail trail, TrailContext context);
    }
}
