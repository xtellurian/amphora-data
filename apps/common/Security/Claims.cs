namespace Amphora.Common.Security
{
    public static class Claims
    {
        public static string About => "about";
        public static string Email => "email";
        public static string EmailConfirmed => "email_confirmed";
        public static string FullName => "full_name";
        public static string GlobalAdmin => "global_admin";
        public static string UserName => "name";

        /// <summary>
        /// A claim that allows a token to purchase on behalf the user.
        /// </summary>
        public const string Purchase = "amphora.purchase";
    }
}
