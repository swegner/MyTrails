namespace MyTrails.Importer.Wta
{
    using System;
    using System.ComponentModel.Composition;

    /// <summary>
    /// Factory for creating <see cref="IHttpClient"/> instances.
    /// </summary>
    [Export(typeof(IHttpClientFactory))]
    public class HttpClientFactory : IHttpClientFactory
    {
        /// <summary>
        /// Create a new <see cref="IHttpClient"/> instance for the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint for the client to retrieve.</param>
        /// <returns>An initialized <see cref="IHttpClient"/> instance.</returns>
        /// <seealso cref="IHttpClientFactory.CreateClient"/>
        public IHttpClient CreateClient(Uri endpoint)
        {
            return new HttpClientAdapter(endpoint);
        }
    }
}