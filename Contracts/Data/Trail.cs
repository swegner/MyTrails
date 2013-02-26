namespace MyTrails.Contracts.Data
{
    /// <summary>
    /// A hiking trail.
    /// </summary>
    public class Trail
    {
        /// <summary>
        /// Data store ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The WTA ID of the trail.
        /// </summary>
        public string WtaId { get; set; }

        /// <summary>
        /// Name of the hike.
        /// </summary>
        public string Name { get; set; }
    }
}
