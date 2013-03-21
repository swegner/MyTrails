namespace MyTrails.ServiceLib.Retry
{
    using System;
    using Microsoft.Practices.TransientFaultHandling;

    /// <summary>
    /// Strategy for detecting transient HTTP errors.
    /// </summary>
    public class HttpErrorDetectionStrategy : ITransientErrorDetectionStrategy
    {
        /// <summary>
        /// Determines whether the specified exception represents a transient failure that can be compensated by a retry.
        /// </summary>
        /// <param name="ex">The exception object to be verified.</param>
        /// <returns>True if the specified exception is considered as transient, otherwise false.</returns>
        /// <seealso cref="ITransientErrorDetectionStrategy.IsTransient"/>
        public bool IsTransient(Exception ex)
        {
            bool isTransient;
            if (ex is TimeoutException)
            {
                isTransient = true;
            }
            else
            {
                isTransient = false;
            }

            return isTransient;
        }
    }
}
