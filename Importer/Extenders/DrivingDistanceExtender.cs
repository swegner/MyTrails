namespace MyTrails.Importer.Extenders
{
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;
    using MyTrails.Contracts.Data;
    using MyTrails.DataAccess;

    /// <summary>
    /// Trail extender which adds driving directions between each trail
    /// and registered user.
    /// </summary>
    [Export(typeof(ITrailExtender))]
    public class DrivingDistanceExtender : ITrailExtender
    {
        /// <summary>
        /// Add additional context to the trail.
        /// </summary>
        /// <param name="trail">The trail to extend.</param>
        /// <param name="context">Datastore context.</param>
        /// <returns>Task for asynchronous completion.</returns>
        /// <seealso cref="ITrailExtender.Extend"/>
        public async Task Extend(Trail trail, MyTrailsContext context)
        {
        }
    }
}
