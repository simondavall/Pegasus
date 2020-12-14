namespace PegasusApi.Library.Models.Manage
{
    public class VerifyTwoFactorTokenModel : ManageBaseModel
    {
        public string VerificationCode { get; set; }
        public bool IsTokenValid { get; set; }
    }
}
