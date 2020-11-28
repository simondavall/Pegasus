using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pegasus.Library.Models.Manage
{
    public class UserDetailsModel : ManageBaseModel
    {
        public string UserId  { get; set; }
        public string Username { get; set; }
        public IList<string> Errors { get; } = new List<string>();


        [Display(Name = "Display Name")]
        public string  DisplayName { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
    }
}
