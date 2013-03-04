namespace MyTrails.Importer.Test
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Static methods and extensions for working with <see cref="Task"/>s.
    /// </summary>
    public static class TaskExt
    {
        /// <summary>
        /// Wrap a result object in an instantaneous task.
        /// </summary>
        /// <typeparam name="T">The task return type.</typeparam>
        /// <param name="resultLambda">Lambda with the result object to wrap.</param>
        /// <returns>An instantaneous task.</returns>
        /// <remarks>Using a lambda prevents static analysis from thinking the object is out of scope.</remarks>
        public static Task<T> WrapInTask<T>(Func<T> resultLambda)
        {
            if (resultLambda == null)
            {
                throw new ArgumentNullException("resultLambda");
            }

            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
            tcs.SetResult(resultLambda());

            return tcs.Task;
        }

        /// <summary>
        /// Create a no-op completion task.
        /// </summary>
        /// <returns>A no-op task which is completed.</returns>
        public static Task CreateNopOpTask()
        {
            return TaskExt.WrapInTask(() => (object)null);
        }
    }
}
