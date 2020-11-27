using System.ComponentModel.DataAnnotations;

namespace Pegasus.Library.Models.Manage
{
    public class IndexModel : ManageBaseModel
    {
        public string Username { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
    }
}
