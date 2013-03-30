namespace MyTrails.ServiceLib.Test.Retry
{
    using System;
    using Microsoft.Practices.TransientFaultHandling;

    /// <summary>
    /// Stub error detection strategy for use during testing.
    /// </summary>
    public class StubErrorDetectionStrategy : ITransientErrorDetectionStrategy
    {
        /// <summary>
        /// Returns false, always.
        /// </summary>
        /// <param name="ex">The parameter is not used.</param>
        /// <returns>False, always.</returns>
        /// <seealso cref="ITransientErrorDetectionStrategy.IsTransient"/>
        public bool IsTransient(Exception ex)
        {
            return false;
        }
    }
}