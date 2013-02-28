namespace Importer.Test
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MyTrails.Contracts.Data;
    using MyTrails.Importer;
    using MyTrails.Importer.Wta;

    /// <summary>
    /// Unit tests for the <see cref="TrailFactory"/> class.
    /// </summary>
    [TestClass]
    public class TrailFactoryTests
    {
        /// <summary>
        /// Sample WTA ID to use during testing.
        /// </summary>
        private const string AnyWtaId = "any-wta-id";

        /// <summary>
        /// "Normal" trail with complete data to be filled in.
        /// </summary>
        private static readonly TestData NormalTrail = new TestData
        {
            Input = new WtaTrail
            {
                Uid = AnyWtaId,
            },
            ExpectedOutput = new Trail
            {
                WtaId = AnyWtaId,
            }
        };

        /// <summary>
        /// <see cref="TrailFactory"/> instance to test against.
        /// </summary>
        private TrailFactory _factory;

        /// <summary>
        /// Initialize test helper objects.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this._factory = new TrailFactory();
        }

        /// <summary>
        /// Verify that the factory assigns the <see cref="Trail.WtaId"/>
        /// </summary>
        [TestMethod]
        public void AssignsWtaId()
        {
            // Act / Assert
            this.TestFactoryMethod(NormalTrail, t => t.WtaId);
        }

        /// <summary>
        /// Call <see cref="TrailFactory.CreateTrail"/> on the test data and compare the property
        /// selected in <see cref="propertySelector"/> to the expected value.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property to select for verification.</typeparam>
        /// <param name="testData">The test data set.</param>
        /// <param name="propertySelector">Method to select the output property to verify.</param>
        private void TestFactoryMethod<TProperty>(TestData testData, Func<Trail, TProperty> propertySelector)
        {
            // Act
            Trail actual = this._factory.CreateTrail(testData.Input);

            // Assert
            TProperty expectedProperty = propertySelector(testData.ExpectedOutput);
            TProperty actualProperty = propertySelector(actual);
            Assert.AreEqual(expectedProperty, actualProperty);
        }

        /// <summary>
        /// Helper class to hold sample data sets.
        /// </summary>
        private class TestData
        {
            /// <summary>
            /// The input <see cref="WtaTrail"/>
            /// </summary>
            public WtaTrail Input { get; set; }

            /// <summary>
            /// The expected <see cref="Trail"/> output.
            /// </summary>
            public Trail ExpectedOutput { get; set; }
        }
    }
}
