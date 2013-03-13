namespace MyTrails.DataAccess.Test
{
    using System;
    using System.Data.Entity;
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
            this.TestAccess(this._context.Trails);
        }

        /// <summary>
        /// Verify that <see cref="MyTrailsContext.Regions"/> is accessible.
        /// </summary>
        [TestMethod]
        public void CanAccessRegions()
        {
            // Act
            this.TestAccess(this._context.Regions);
        }

        /// <summary>
        /// Verify that <see cref="MyTrailsContext.Guidebooks"/> is accessible.
        /// </summary>
        [TestMethod]
        public void CanAccessGuidebooks()
        {
            // Act
            this.TestAccess(this._context.Guidebooks);
        }

        /// <summary>
        /// Verify that <see cref="MyTrailsContext.Passes"/> is accessible.
        /// </summary>
        [TestMethod]
        public void CanAccessPasses()
        {
            // Act
            this.TestAccess(this._context.Passes);
        }

        /// <summary>
        /// Verify that <see cref="MyTrailsContext.TrailFeatures"/> is accessible.
        /// </summary>
        [TestMethod]
        public void CanAccessTrailFeatures()
        {
            // Act
            this.TestAccess(this._context.TrailFeatures);
        }

        /// <summary>
        /// Verify that <see cref="MyTrailsContext.TrailCharacteristics"/> is accessible.
        /// </summary>
        [TestMethod]
        public void CanAccessTrailCharacteristics()
        {
            // Act
            this.TestAccess(this._context.TrailCharacteristics);
        }

        /// <summary>
        /// Verify that <see cref="MyTrailsContext.TripReports"/> is accessible.
        /// </summary>
        [TestMethod]
        public void CanAccessTripReports()
        {
            // Act
            this.TestAccess(this._context.TripReports);
        }

        /// <summary>
        /// Verify that <see cref="MyTrailsContext.TripTypes"/> is accessible.
        /// </summary>
        [TestMethod]
        public void CanAccessTripTypes()
        {
            // Act
            this.TestAccess(this._context.TripTypes);
        }

        /// <summary>
        /// Verify that <see cref="MyTrailsContext.Users"/> is accessible.
        /// </summary>
        [TestMethod]
        public void CanAccessUsers()
        {
            // Act
            this.TestAccess(this._context.Users);
        }

        /// <summary>
        /// Verify that <see cref="MyTrailsContext.Addresses"/> is accessible.
        /// </summary>
        [TestMethod]
        public void CanAccessAddresses()
        {
            // Act
            this.TestAccess(this._context.Addresses);
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
        /// Verify that <see cref="MyTrailsContext.Passes"/> is seeded with data.
        /// </summary>
        [TestMethod]
        public void PassesHasSeedData()
        {
            // Act
            RequiredPass pass = this._context.Passes
                .Where(rp => rp.Name == "Discover Pass")
                .FirstOrDefault();

            // Assert
            Assert.IsNotNull(pass);
        }

        /// <summary>
        /// Verify that <see cref="MyTrailsContext.TrailFeatures"/> is seeded with data.
        /// </summary>
        [TestMethod]
        public void TrailFeaturesHasSeedData()
        {
            // Act
            TrailFeature feature = this._context.TrailFeatures
                .Where(tf => tf.Description == "Mountain Views")
                .FirstOrDefault();

            // Assert
            Assert.IsNotNull(feature);
        }

        /// <summary>
        /// Verify that <see cref="MyTrailsContext.TrailCharacteristics"/> is seeded with data.
        /// </summary>
        [TestMethod]
        public void TrailCharacteristicsHasSeedData()
        {
            // Act
            TrailCharacteristic characteristic = this._context.TrailCharacteristics
                .Where(tf => tf.Description == "Dogs Allowed Without Leash")
                .FirstOrDefault();

            // Assert
            Assert.IsNotNull(characteristic);
        }

        /// <summary>
        /// Verify that <see cref="MyTrailsContext.TripTypes"/> is seeded with data.
        /// </summary>
        [TestMethod]
        public void TripTypesHasSeedData()
        {
            // Act
            TripType tripType = this._context.TripTypes
                .Where(tt => tt.WtaId == "overnight")
                .FirstOrDefault();

            // Assert
            Assert.IsNotNull(tripType);
            Assert.AreEqual("Overnight Backpack", tripType.Description);
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

        /// <summary>
        /// Test that the given <see cref="DbSet{TEntity}"/> is accessible.
        /// </summary>
        /// <typeparam name="T">The entity type for the dataset.</typeparam>
        /// <param name="dataSet">The dataset to test.</param>
        private void TestAccess<T>(DbSet<T> dataSet)
            where T : class
        {
            dataSet.Load();
        }
    }
}
