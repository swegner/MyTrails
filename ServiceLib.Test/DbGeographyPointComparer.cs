namespace MyTrails.ServiceLib.Test
{
    using System;
    using System.Collections.Generic;
    using System.Data.Spatial;

    /// <summary>
    /// Equality comparer to check two <see cref="DbGeography"/> points for equality.
    /// </summary>
    public class DbGeographyPointComparer : EqualityComparer<DbGeography>
    {
        /// <summary>
        /// Check whether two <see cref="DbGeography"/> points are equal.
        /// </summary>
        /// <param name="x">The first <see cref="DbGeography"/> point.</param>
        /// <param name="y">The second <see cref="DbGeography"/> point.</param>
        /// <returns>True if the points are equal, or false otherwise.</returns>
        /// <seealso cref="EqualityComparer{T}.Equals(T,T)"/>
        public override bool Equals(DbGeography x, DbGeography y)
        {
            if (x == null || y == null)
            {
                return object.ReferenceEquals(x, y);
            }

            if (x.PointCount != 1 || y.PointCount != 1)
            {
                throw new InvalidOperationException("Comparer only support single-point DbGeography objects.");
            }

            return object.ReferenceEquals(x, y) ||
                (x.CoordinateSystemId == y.CoordinateSystemId &&
                    x.Latitude == y.Latitude);
        }

        /// <summary>
        /// Generate a hash-code for the <see cref="DbGeography"/> point.
        /// </summary>
        /// <param name="obj">The point to generate a hash code for.</param>
        /// <returns>A hash code for the object.</returns>
        /// <seealso cref="EqualityComparer{T}.GetHashCode(T)"/>
        public override int GetHashCode(DbGeography obj)
        {
            if (obj == null)
            {
                return 0;
            }

            if (obj.PointCount != 1)
            {
                throw new InvalidOperationException("Comparer only support single-point DbGeography objects.");
            }

            int hash = 23;
            hash = (hash * obj.CoordinateSystemId.GetHashCode()) + 13;
            hash = (hash * obj.Latitude.GetHashCode()) + 13;
            hash = (hash * obj.Longitude.GetHashCode()) + 13;

            return hash;
        }
    }
}