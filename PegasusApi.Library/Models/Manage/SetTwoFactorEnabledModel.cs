namespace PegasusApi.Library.Models.Manage
{
    public class SetTwoFactorEnabledModel : ManageBaseModel
    {
        public string Email { get; set; }
        public bool Enabled { get; set; }
        public string UserId { get; set; }
        public bool Succeeded { get; set; }
    }
}
