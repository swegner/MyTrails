namespace MyTrails.Importer.Extenders
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Threading.Tasks;
    using log4net;
    using Microsoft.Practices.TransientFaultHandling;
    using MyTrails.Contracts.Data;
    using MyTrails.DataAccess;
    using MyTrails.Importer.BingMaps;
    using MyTrails.Importer.BingMaps.Routing;
    using MyTrails.Importer.Retry;

    /// <summary>
    /// Trail extender which adds driving directions between each trail
    /// and registered user.
    /// </summary>
    [Export(typeof(ITrailExtender))]
    public class DrivingDistanceExtender : ITrailExtender
    {
        /// <summary>
        /// Bing maps API credentials.
        /// </summary>
        [Import]
        public IBingMapsCredentials BingMapsCredentials { get; set; }

        /// <summary>
        /// Factory for creating <see cref="IRouteService"/> instances. 
        /// </summary>
        [Import]
        public IRouteServiceFactory RouteServiceFactory { get; set; }

        /// <summary>
        /// Logging interface.
        /// </summary>
        [Import]
        public ILog Logger { get; set; }

        /// <summary>
        /// Add additional context to the trail.
        /// </summary>
        /// <param name="trail">The trail to extend.</param>
        /// <param name="context">Datastore context.</param>
        /// <returns>Task for asynchronous completion.</returns>
        /// <seealso cref="ITrailExtender.Extend"/>
        public async Task Extend(Trail trail, MyTrailsContext context)
        {
            if (trail.Location != null)
            {
                this.Logger.InfoFormat("Looking up driving directions for trail: {0}", trail.Name);

                Task[] addDirectionsTasks = context.Addresses
                    .Where(a => a.Directions.All(d => d.TrailId != trail.Id))
                    .ToList() // Needed to force the EF query.
                    .Select(a => this.AddDrivingDirections(a, trail))
                    .ToArray();

                await Task.WhenAll(addDirectionsTasks);
            }
        }

        /// <summary>
        /// Add driving directions for the given trail / address pair.
        /// </summary>
        /// <param name="address">The starting address.</param>
        /// <param name="trail">The trail ending address.</param>
        /// <returns>Task for asynchronous completion.</returns>
        private async Task AddDrivingDirections(Address address, Trail trail)
        {
            RouteRequest request = new RouteRequest
            {
                Credentials = new Credentials
                {
                    ApplicationId = this.BingMapsCredentials.ApplicationId,
                },
                Waypoints = new[]
                {
                    new Waypoint
                    {
                        Description = "Start",
                        Location = new Location
                        {
                            Latitude = address.Coordinate.Latitude.Value,
                            Longitude = address.Coordinate.Longitude.Value,
                        },
                    },
                    new Waypoint
                    {
                        Description = "End",
                        Location = new Location
                        {
                            Latitude = trail.Location.Latitude.Value,
                            Longitude = trail.Location.Longitude.Value,
                        },
                    },
                },
            };

            RouteResponse response;
            RetryPolicy policy = this.BuildRetryPolicy();
            IRouteService routeService = this.RouteServiceFactory.CreateRouteService();
            try
            {
                response = await policy.ExecuteAsync(() => routeService.CalculateRouteAsync(request));
            }
            finally
            {
                IDisposable disposable = routeService as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            address.Directions.Add(new DrivingDirections
            {
                Address = address,
                Trail = trail,
                DrivingTimeSeconds = (int)response.Result.Summary.TimeInSeconds,
            });
        }

        /// <summary>
        /// Build a retry policy for querying Bing Maps.
        /// </summary>
        /// <returns>An initialized retry policy.</returns>
        private RetryPolicy BuildRetryPolicy()
        {
            RetryStrategy strategy = new ExponentialBackoff(
                retryCount: 5, minBackoff: TimeSpan.FromMilliseconds(100), maxBackoff: TimeSpan.FromSeconds(5), deltaBackoff: TimeSpan.FromMilliseconds(500))
            {
                FastFirstRetry = true,
            };
            RetryPolicy policy = new RetryPolicy(new HttpErrorDetectionStrategy(), strategy);
            policy.Retrying += (sender, args) => this.Logger.WarnFormat("Retrying Bing Maps request due to exception: {0}", args.LastException);
            return policy;
        }
    }
}
