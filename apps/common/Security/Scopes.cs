namespace Amphora.Common.Security
{
    public static class Scopes
    {
        /// <summary>
        /// The main amphora scope.
        /// </summary>
        public static string AmphoraScope => "amphora";

        /// <summary>
        /// Enables an application to purchase amphorae on behalf of a user.
        /// </summary>
        public static string PurchaseScope => "amphora.purchase";
    }
}