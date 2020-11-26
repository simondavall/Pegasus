namespace Pegasus.Models.Account
{
    public class SignInResultModel
    {
        public bool Success { get; set; }
        public bool RequiresTwoFactor { get; set; }
    }
}
