namespace Pegasus.Library.JwtAuthentication.Models
{
    public sealed class AuthUrlOptions
    {
        public string AccessDeniedPath { get; set; }
        public string LoginPath { get; set; }
        public string LogoutPath { get; set; }
        public string ReturnUrlParameter { get; set; }
    }
}
