namespace PegasusApi.Library.Models.Manage
{
    public class SetTwoFactorEnabledModel : ManageBaseModel
    {
        public bool SetEnabled { get; set; }
        public bool Succeeded { get; set; }
    }
}
