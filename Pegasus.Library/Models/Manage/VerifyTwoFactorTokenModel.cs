namespace Pegasus.Library.Models.Manage
{
    public class VerifyTwoFactorTokenModel : ManageBaseModel
    {
        public string Email { get; set; }
        public string VerificationCode { get; set; }
        public bool Is2FaTokenValid { get; set; }
    }
}
