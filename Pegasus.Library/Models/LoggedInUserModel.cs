namespace Pegasus.Library.Models
{
    public class LoggedInUserModel : ILoggedInUserModel
    {
        public string  Username { get; set; }
        public string Token { get; set; }
        public bool IsLoggedIn { get; set; }
    }
}
