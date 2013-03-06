namespace MyTrails.Importer.Wta
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// User info categorizations from WTA website.
    /// </summary>
    [Flags]
    [SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames",
        Justification = "Info is plural.")]
    public enum WtaUserInfo
    {
        /// <summary>
        /// No user info provided about the trail.
        /// </summary>
        None = 0,

        /// <summary>
        /// Trail is good for kids.
        /// </summary>
        GoodForKids = 1 << 0,

        /// <summary>
        /// Dogs are allowed on-leash.
        /// </summary>
        DogsAllowedOnLeash = 1 << 1,

        /// <summary>
        /// Dogs are allowed without a leash.
        /// </summary>
        DogsAllowedWithoutLeash = 1 << 2,

        /// <summary>
        /// Dogs are not allowed.
        /// </summary>
        DogsNotAllowed = 1 << 3,

        /// <summary>
        /// May encounter pack animals on trail.
        /// </summary>
        MayEncounterPackAnimals = 1 << 4,

        /// <summary>
        /// May encounter mountain bikes on trail.
        /// </summary>
        MayEncounterMountainBikes = 1 << 5,

        /// <summary>
        /// May encounter motorized vehicles on trail.
        /// </summary>
        MayEncounterMotorizedVehicles = 1 << 6,

        /// <summary>
        /// A permit or pass is required.
        /// </summary>
        PermitOrPassRequired = 1 << 7
    }
}
