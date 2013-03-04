namespace MyTrails.DataAccess.Test
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MyTrails.Contracts.Data;

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
        /// Verify that <see cref="MyTrailsContext.Regions"/> is accessible.
        /// </summary>
        [TestMethod]
        public void CanAccessRegions()
        {
            // Act
            this._context.Regions.FirstOrDefault();
        }

        /// <summary>
        /// Verify that <see cref="MyTrailsContext.Regions"/> is seeded with data.
        /// </summary>
        [TestMethod]
        public void RegionsHasSeedData()
        {
            // Act
            Region region = this._context.Regions
                .Where(r => r.Name == "Central Cascades")
                .FirstOrDefault();

            // Assert
            Assert.IsNotNull(region);

            // Act
            Guid expectedGuid = Guid.Parse("637634387ca38685f89162475c7fc1d2");
            Region subRegion = region.SubRegions
                .Where(sr => sr.WtaId == expectedGuid)
                .FirstOrDefault();

            // Assert
            Assert.IsNotNull(subRegion);
            Assert.AreEqual("Stevens Pass - West", subRegion.Name);
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
