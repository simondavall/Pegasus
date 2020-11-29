namespace Pegasus.Library.Models.Account
{
    public class RedeemTwoFactorRecoveryCodeModel
    {
        public string UserId { get; set; }
        public string RecoveryCode { get; set; }
        public bool Succeeded { get; set; }
    }
}
