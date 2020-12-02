using System.ComponentModel.DataAnnotations;

namespace Pegasus.Library.Models.Manage
{
    public class UserDetailsModel : ManageBaseModel
    {
        public string Username { get; set; }


        [Display(Name = "Display Name")]
        public string  DisplayName { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
    }
}
