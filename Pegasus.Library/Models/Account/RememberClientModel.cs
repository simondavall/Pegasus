namespace Pegasus.Library.Models.Account
{
    public class RememberClientModel
    {
        public string UserId { get; set; }
        public bool SupportsUserSecurityStamp { get; set; }
        public string SecurityStamp { get; set; }
    }
}
