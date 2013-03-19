namespace MyTrails.Importer.Wta
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Adapter class for working with <see cref="HttpClient"/> through the <see cref="IHttpClient"/> interface.
    /// </summary>
    public class HttpClientAdapter : IHttpClient
    {
        /// <summary>
        /// The endpoint to retrieve.
        /// </summary>
        private readonly Uri _endpoint;

        /// <summary>
        /// The <see cref="HttpClient"/> instance to wrap.
        /// </summary>
        private HttpClient _innerClient;

        /// <summary>
        /// Whether the object has been disposed of.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Construct a new <see cref="HttpClientAdapter"/> for the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint to retrieve.</param>
        public HttpClientAdapter(Uri endpoint)
        {
            this._endpoint = endpoint;

            this._innerClient = new HttpClient();
        }

        /// <summary>
        /// Time to wait before the request times out.
        /// </summary>
        /// <seealso cref="IHttpClient.Timeout"/>
        public TimeSpan Timeout
        {
            get { return this._innerClient.Timeout; }
            set { this._innerClient.Timeout = value; }
        }

        /// <summary>
        /// Send a GET request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <returns>A task for the asynchronous operation.</returns>
        /// <seealso cref="IHttpClient.SendGetRequest"/>
        public async Task<HttpResponseMessage> SendGetRequest()
        {
            HttpResponseMessage response;
            try
            {
                response = await this._innerClient.GetAsync(this._endpoint);
            }
            catch (TaskCanceledException ex)
            {
                throw new TimeoutException("HTTP request timed out", ex);
            }

            return response;
        }

        /// <summary>
        /// Dispose of object resources.
        /// </summary>
        /// <seealso cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose of object resources.
        /// </summary>
        /// <param name="disposing">Whether it is safe to reference managed objects.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    this._innerClient.Dispose();
                    this._innerClient = null;
                }

                this._disposed = true;
            }
        }
    }
}