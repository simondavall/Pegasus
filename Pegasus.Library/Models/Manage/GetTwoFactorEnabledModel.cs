namespace Pegasus.Library.Models.Manage
{
    public class GetTwoFactorEnabledModel : ManageBaseModel
    {
        public bool Enabled { get; set; }
        public string UserId { get; set; }
    }
}
