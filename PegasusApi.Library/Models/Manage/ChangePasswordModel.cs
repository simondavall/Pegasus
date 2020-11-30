using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace PegasusApi.Library.Models.Manage
{
    public class ChangePasswordModel : ManageBaseModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public bool Succeeded { get; set; }
        public IEnumerable<IdentityError> Errors { get; set; }
        public string UserId { get; set; }
        public bool UserNotFound { get; set; }
    }
}
