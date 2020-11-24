namespace Pegasus.Library.Models.Account
{
    public class VerifyTwoFactorModel
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public bool Verified { get; set; }
    }
}
