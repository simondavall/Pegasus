namespace PegasusApi.Library.Models.Manage
{
    public class ChangePasswordModel : ManageBaseModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public bool Succeeded { get; set; }
    }
}
