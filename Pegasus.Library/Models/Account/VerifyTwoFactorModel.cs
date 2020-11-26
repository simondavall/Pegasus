namespace Pegasus.Library.Models.Account
{
    public class VerifyTwoFactorModel
    {
        public string UserId { get; set; }
        public string Code { get; set; }
        public bool Verified { get; set; }
        public bool RememberMachine { get; set; }
    }
}
