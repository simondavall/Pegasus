using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Pegasus.Library.Models.Manage
{
    public class UserDetailsModel : ManageBaseModel
    {
        public string UserId  { get; set; }
        public string Username { get; set; }
        public IEnumerable<IdentityError> Errors { get; }


        [Display(Name = "Display Name")]
        public string  DisplayName { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
    }
}
