namespace MyTrails.Importer.Wta
{
    using System;

    /// <summary>
    /// Merchant which carries a guidebook returned from WTA.
    /// </summary>
    public class WtaGuidebookMerchant
    {
        /// <summary>
        /// Outlet name for the merchant.
        /// </summary>
        public string Outlet { get; set; }

        /// <summary>
        /// Link to the guidebook on the merchant website.
        /// </summary>
        public Uri Url { get; set; }
    }
}
