namespace Pegasus.Library.JwtAuthentication.Models
{
    public class AuthenticatedUser
    {
        public string AccessToken { get; set; }
        public bool Authenticated { get; set; }
        public string Username { get; set; }
        public string UserId { get; set; }
        public bool RequiresTwoFactor { get; set; }
        public string FailedReason { get; set; }
    }
}
