using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace PegasusApi.Library.Models.Manage
{
    public class UserDetailsModel : ManageBaseModel
    {
        public string UserId  { get; set; }
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
        public string DisplayName { get; set; }
        public List<IdentityError> Errors { get; } = new List<IdentityError>();
    }
}
