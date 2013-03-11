namespace MyTrails.Importer.Test
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MyTrails.DataAccess;
    using MyTrails.Importer;
    using MyTrails.Importer.Wta;

    /// <summary>
    /// End-to-end tests for the importer.
    /// </summary>
    [TestClass]
    public class E2ETests
    {
        /// <summary>
        /// Initialize test context.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            using (MyTrailsContext context = new MyTrailsContext())
            {
                context.ClearDatabase();
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Verify that sample data can be imported without errors.
        /// </summary>
        [TestMethod, TestCategory(TestCategory.Integration)]
        public void CanImportSampleData()
        {
            // Arrange
            ExecutionOptions options = new ExecutionOptions
            {
                Modes = ImportModes.ImportOnly,
            };

            string contractToReplace = AttributedModelServices.GetContractName(typeof(IHttpClientFactory));

            using (ApplicationCatalog initialCatalog = Program.BuildCompositionCatalog())
            using (FilteredCatalog filteredCatalog = initialCatalog.Filter(d => !d.Exports(contractToReplace)))
            using (TypeCatalog replacementCatalog = new TypeCatalog(typeof(WtaResultFromEmbeddedResourceFactory)))
            using (AggregateCatalog aggregateCatalog = new AggregateCatalog(filteredCatalog, replacementCatalog))
            using (CompositionContainer container = new CompositionContainer(aggregateCatalog))
            {
                // Act
                Program p = container.GetExportedValue<Program>();
                p.Run(options);
            }
        }

        /// <summary>
        /// Retrieve WTA data from an embedded resource rather than dynamic web results.
        /// </summary>
        private sealed class WtaDataFromEmbeddedResource : IHttpClient
        {
            /// <summary>
            /// The resource to retrieve data from.
            /// </summary>
            private readonly string _resourceName;

            /// <summary>
            /// Construct a new <see cref="WtaDataFromEmbeddedResource"/> instance.
            /// </summary>
            /// <param name="resourceName">The resource to retrieve data from.</param>
            public WtaDataFromEmbeddedResource(string resourceName)
            {
                this._resourceName = resourceName;
            }

            /// <summary>
            /// Send a GET request to the specified Uri as an asynchronous operation.
            /// </summary>
            /// <returns>A task for the asynchronous operation.</returns>
            /// <seealso cref="IHttpClient.SendGetRequest"/>
            public Task<HttpResponseMessage> SendGetRequest()
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream resourceStream = assembly.GetManifestResourceStream(this._resourceName);

                Task<HttpResponseMessage> returnTask;
                try
                {
                    HttpResponseMessage response = new HttpResponseMessage();

                    try
                    {
                        response.StatusCode = HttpStatusCode.OK;
                        response.Content = new StreamContent(resourceStream);

                        returnTask = TaskExt.WrapInTask(() => response);
                    }
                    catch
                    {
                        response.Dispose();
                        throw;
                    }
                }
                catch
                {
                    resourceStream.Dispose();
                    throw;
                }

                return returnTask;
            }

            /// <summary>
            /// Do nothing.
            /// </summary>
            /// <seealso cref="IDisposable.Dispose"/>
            public void Dispose()
            {
            }
        }

        /// <summary>
        /// Factory for creating <see cref="WtaDataFromEmbeddedResource"/> instances.
        /// </summary>
        [Export(typeof(IHttpClientFactory))]
        private class WtaResultFromEmbeddedResourceFactory : IHttpClientFactory
        {
            /// <summary>
            /// Trip report resource files.
            /// </summary>
            private static readonly string[] TripReportResources = new[]
            {
                "MyTrails.Importer.Test.SampleData.tripreports1.js",
                "MyTrails.Importer.Test.SampleData.tripreports2.js",
                "MyTrails.Importer.Test.SampleData.tripreports3.js",
                "MyTrails.Importer.Test.SampleData.tripreports4.js",
                "MyTrails.Importer.Test.SampleData.tripreports5.js",
            };

            /// <summary>
            /// Create a new <see cref="IHttpClient"/> instance for the given endpoint.
            /// </summary>
            /// <param name="endpoint">The endpoint for the client to retrieve.</param>
            /// <returns>An initialized <see cref="IHttpClient"/> instance.</returns>
            /// <seealso cref="IHttpClientFactory.CreateClient"/>
            public IHttpClient CreateClient(Uri endpoint)
            {
                if (endpoint == null)
                {
                    throw new ArgumentNullException("endpoint");
                }

                string resourceName;
                if (endpoint == WtaClient.SearchEndpoint)
                {
                    resourceName = "MyTrails.Importer.Test.SampleData.trails.js";
                }
                else if (endpoint.AbsoluteUri.Contains(WtaClient.TripReportsEndpointFormat))
                {
                    int resourceIndex = Math.Abs(endpoint.GetHashCode()) % TripReportResources.Length;
                    resourceName = TripReportResources[resourceIndex];
                }
                else
                {
                    throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture,
                        "Endpoint not supported: {0}", WtaClient.SearchEndpoint));
                }

                return new WtaDataFromEmbeddedResource(resourceName);
            }
        }
    }
}
