namespace MyTrails.DataAccess.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for the <see cref="MyTrailsContext"/> class.
    /// </summary>
    [TestClass]
    public class MyTrailsContextTests : IDisposable
    {
        /// <summary>
        /// The data context to test against.
        /// </summary>
        private MyTrailsContext _context;

        /// <summary>
        /// Whether the object has been disposed of.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initialize test helper objects.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this._context = new MyTrailsContext();
        }

        /// <summary>
        /// Verify that <see cref="MyTrailsContext.Trails"/> is accessible.
        /// </summary>
        [TestMethod]
        public void CanAccessTrails()
        {
            // Act
            this._context.Trails.FirstOrDefault();
        }

        /// <summary>
        /// Dispose of object references.
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
                    if (this._context != null)
                    {
                        this._context.Dispose();
                        this._context = null;
                    }
                }

                this._disposed = true;
            }
        }
    }
}
