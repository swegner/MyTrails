namespace MyTrails.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Validation;
    using System.Globalization;
    using System.Linq;
    using log4net;

    /// <summary>
    /// Extension methods for working with entity sets.
    /// </summary>
    public static class EntityExtensions
    {
        /// <summary>
        /// Remove all entity from a data table.
        /// </summary>
        /// <typeparam name="T">The entity type of the dataset.</typeparam>
        /// <param name="this">The data table to truncate.</param>
        public static void Truncate<T>(this DbSet<T> @this)
            where T : class
        {
            if (@this == null)
            {
                throw new ArgumentNullException("this");
            }

            foreach (T entity in @this)
            {
                @this.Remove(entity);
            }
        }

        /// <summary>
        /// Save entity changes, and log any validation errors to the logger.
        /// </summary>
        /// <param name="this">The data context to save changes from.</param>
        /// <param name="logger">Logging interface.</param>
        public static void SaveChanges(this DbContext @this, ILog logger)
        {
            if (@this == null)
            {
                throw new ArgumentNullException("this");
            }

            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            try
            {
                @this.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                const int maxValidationErrorsToLog = 3;

                IEnumerable<string> validationErrorsForLogging = ex.EntityValidationErrors
                    .SelectMany(eve => eve.ValidationErrors.Select(ve => string.Format(CultureInfo.InvariantCulture,
                        "{0}.{1}: {2}", eve.Entry.GetType(), ve.PropertyName, ve.ErrorMessage)))
                    .Take(maxValidationErrorsToLog);

                logger.ErrorFormat("Validation error while saving entity changes:\n{0}",
                    string.Join(Environment.NewLine, validationErrorsForLogging));

                throw;
            }
        }
    }
}
