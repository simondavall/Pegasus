namespace PegasusApi.Models.Account
{
    public class ForgotPasswordModel : ApiModelBase
    {
        public string Email { get; set; }
        public string BaseUrl { get; set; }
    }
}
