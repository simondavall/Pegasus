namespace PegasusApi.Library.Models.Manage
{
    public class ResetAuthenticatorModel : ManageBaseModel
    {
        public string Email { get; set; }
        public string UserId { get; set; }
    }
}
