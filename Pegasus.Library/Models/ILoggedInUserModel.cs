namespace Pegasus.Library.Models
{
    public interface ILoggedInUserModel
    {
        string Username { get; set; }
        string Token { get; set; }
        bool IsLoggedIn { get; set; }

    }
}