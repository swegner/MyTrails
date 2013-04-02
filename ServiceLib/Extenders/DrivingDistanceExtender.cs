namespace MyTrails.ServiceLib.Extenders
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.ServiceModel;
    using System.Threading.Tasks;
    using log4net;
    using Microsoft.Practices.TransientFaultHandling;
    using MyTrails.Contracts.Data;
    using MyTrails.DataAccess;
    using MyTrails.ServiceLib.BingMaps;
    using MyTrails.ServiceLib.BingMaps.Routing;
    using MyTrails.ServiceLib.Retry;

    /// <summary>
    /// Trail extender which adds driving directions between each trail
    /// and registered user.
    /// </summary>
    [Export(typeof(ITrailExtender))]
    public class DrivingDistanceExtender : ITrailExtender
    {
        /// <summary>
        /// Settings for Bing Maps API.
        /// </summary>
        [Import]
        public IBingMapsConfiguration Configuration { get; set; }

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

                List<Address> addresses = context.Addresses
                    .Where(a => a.Directions.All(d => d.TrailId != trail.Id))
                    .ToList(); // Force EF query to avoid multiple active results sets on enumeration.

                foreach (Address address in addresses)
                {
                    await this.AddDrivingDirections(address, trail);
                }
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
                    ApplicationId = this.Configuration.ApplicationId,
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

            ResponseSummary summary;
            RouteResult result;
            RetryPolicy policy = this.BuildRetryPolicy();
            IRouteService routeService = this.RouteServiceFactory.CreateRouteService();
            try
            {
                RouteResponse response = await policy.ExecuteAsync(() => routeService.CalculateRouteAsync(request));
                summary = response.ResponseSummary;
                result = response.Result;
            }
            catch (FaultException<ResponseSummary> ex)
            {
                summary = ex.Detail;
                result = null;
            }
            finally
            {
                IDisposable disposable = routeService as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            if (summary.StatusCode != ResponseStatusCode.Success)
            {
                if (summary.StatusCode == ResponseStatusCode.BadRequest && 
                    summary.FaultReason.Contains("No route was found for the waypoints provided."))
                {
                    this.Logger.WarnFormat("No route found between trail add address: '{0}', '{1}'", trail, address);
                }
                else
                {
                    throw new ApplicationException(string.Format("Routing service call failed. {0}: {1}", summary.StatusCode, summary.FaultReason));
                }
            }
            else
            {
                address.Directions.Add(new DrivingDirections
                {
                    Address = address,
                    Trail = trail,
                    DrivingTimeSeconds = (int)result.Summary.TimeInSeconds,
                });
            }
        }

        /// <summary>
        /// Build a retry policy for querying Bing Maps.
        /// </summary>
        /// <returns>An initialized retry policy.</returns>
        private RetryPolicy BuildRetryPolicy()
        {
            RetryStrategy strategy = new ExponentialBackoff(
                retryCount: this.Configuration.RetryCount,
                minBackoff: this.Configuration.RetryMinBackOff, 
                maxBackoff: this.Configuration.RetryMaxBackOff, 
                deltaBackoff: this.Configuration.RetryDeltaBackOff)
            {
                FastFirstRetry = true,
            };
            RetryPolicy policy = new RetryPolicy(new HttpErrorDetectionStrategy(), strategy);
            policy.Retrying += (sender, args) => this.Logger.WarnFormat("Retrying Bing Maps request due to exception: {0}", args.LastException);
            return policy;
        }
    }
}
