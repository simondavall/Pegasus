namespace Pegasus.Library.JwtAuthentication.Models
{

    /// <summary>
    ///     A simple structure to store the configured login/logout
    ///     paths and the name of the return url parameter
    /// </summary>
    public sealed class AuthUrlOptions
    {
        /// <summary>
        ///     The login path to redirect the user to in case of unauthenticated requests.
        ///     Default is "/Account/Login"
        /// </summary>
        public string LoginPath { get; set; }

        /// <summary>
        ///     The path to redirect the user to once they have logged out.
        ///     Default is "/Account/Logout"
        /// </summary>
        public string LogoutPath { get; set; }

        /// <summary>
        ///     The path to redirect the user to following a successful authentication attempt.
        ///     Default is "returnUrl"
        /// </summary>
        public string ReturnUrlParameter { get; set; }
    }
}
