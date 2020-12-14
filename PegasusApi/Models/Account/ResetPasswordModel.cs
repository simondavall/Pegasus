using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace PegasusApi.Models.Account
{
    public class ResetPasswordModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ResetCode { get; set; }
        public string UserId { get; set; }

        public IEnumerable<IdentityError> Errors { get; set; }
        public bool Succeeded { get; set; }

    }
}
