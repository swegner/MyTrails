namespace MyTrails.ServiceLib.Wta
{
    using System;

    /// <summary>
    /// Factory for creating <see cref="IHttpClient"/> instances.
    /// </summary>
    public interface IHttpClientFactory
    {
        /// <summary>
        /// Create a new <see cref="IHttpClient"/> instance for the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint for the client to retrieve.</param>
        /// <returns>An initialized <see cref="IHttpClient"/> instance.</returns>
        IHttpClient CreateClient(Uri endpoint);
    }
}