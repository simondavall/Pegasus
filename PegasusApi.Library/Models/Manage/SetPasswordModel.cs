namespace PegasusApi.Library.Models.Manage
{
    public class SetPasswordModel : ManageBaseModel
    {
        public string NewPassword { get; set; }
        public bool Succeeded { get; set; }
    }
}
