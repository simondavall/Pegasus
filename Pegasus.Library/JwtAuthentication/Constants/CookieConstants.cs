namespace Pegasus.Library.JwtAuthentication.Constants
{
    /// <summary>
    /// Represents all the options you can use to configure the cookies middleware used by the identity system.
    /// </summary>
    public class CookieConstants
    {
        private static readonly string CookiePrefix = "PegasusAuth";
        /// <summary>
        /// The scheme used to identify application authentication cookies.
        /// </summary>
        public static readonly string ApplicationScheme = CookiePrefix + ".Application";

        /// <summary>
        /// The scheme used to identify external authentication cookies.
        /// </summary>
        public static readonly string ExternalScheme = CookiePrefix + ".External";

        /// <summary>
        /// The scheme used to identify Two Factor authentication cookies for saving the Remember Me state.
        /// </summary>
        public static readonly string TwoFactorRememberMeScheme = CookiePrefix + ".TwoFactorRememberMe";

        /// <summary>
        /// The scheme used to identify Two Factor authentication cookies for round tripping user identities.
        /// </summary>
        public static readonly string TwoFactorUserIdScheme = CookiePrefix + ".TwoFactorUserId";

        /// <summary>
        /// The cookie name used to identify user settings.
        /// </summary>
        public static readonly string UserSettings = CookiePrefix + ".UserSettings";
    }
}
