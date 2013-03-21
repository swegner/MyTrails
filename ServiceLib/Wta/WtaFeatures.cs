namespace MyTrails.ServiceLib.Wta
{
    using System;

    /// <summary>
    /// Trail features from the WTA website.
    /// </summary>
    [Flags]
    public enum WtaFeatures
    {
        /// <summary>
        /// No feature information provided.
        /// </summary>
        None = 0,

        /// <summary>
        /// Trail has views of coast.
        /// </summary>
        Coast = 1 << 0,

        /// <summary>
        /// Trail has view of rivers.
        /// </summary>
        Rivers = 1 << 1,

        /// <summary>
        /// Trail has view of lakes.
        /// </summary>
        Lakes = 1 << 2,

        /// <summary>
        /// Trail has views of waterfalls.
        /// </summary>
        Waterfalls = 1 << 3,

        /// <summary>
        /// Trail has view of old growth.
        /// </summary>
        OldGrowth = 1 << 4,

        /// <summary>
        /// Trail has view of fall foliage.
        /// </summary>
        FallFoliage = 1 << 5,

        /// <summary>
        /// Trail has view of wildflowers/meadows.
        /// </summary>
        WildflowersMeadows = 1 << 6,

        /// <summary>
        /// Trail has view of mountains.
        /// </summary>
        MountainViews = 1 << 7,

        /// <summary>
        /// Trail has view from summits.
        /// </summary>
        Summits = 1 << 8,

        /// <summary>
        /// Trail may have views of wildlife.
        /// </summary>
        Wildlife = 1 << 9,

        /// <summary>
        /// Trail has ridge passes.
        /// </summary>
        RidgesPasses = 1 << 10,

        /// <summary>
        /// Trail has established campsites.
        /// </summary>
        EstablishedCampsites = 1 << 11,
    }
}
