namespace PegasusApi.Library.Models.Manage
{
    public class AuthenticatorKeyModel : ManageBaseModel
    {
        public string SharedKey { get; set; }
        public string AuthenticatorUri { get; set; }
    }
}
