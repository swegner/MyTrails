﻿namespace MyTrails.Importer.Wta
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Facade interface around the <see cref="HttpClient"/> class.
    /// </summary>
    public interface IHttpClient : IDisposable
    {
        /// <summary>
        /// Time to wait before the request times out.
        /// </summary>
        TimeSpan Timeout { get; set; }

        /// <summary>
        /// Send a GET request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <returns>A task for the asynchronous operation.</returns>
        Task<HttpResponseMessage> SendGetRequest();
    }
}
