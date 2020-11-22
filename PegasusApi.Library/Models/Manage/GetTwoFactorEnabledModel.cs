namespace PegasusApi.Library.Models.Manage
{
    public class GetTwoFactorEnabledModel : ManageBaseModel
    {
        public string Email { get; set; }
        public bool Enabled { get; set; }
        public string UserId { get; set; }
    }
}
