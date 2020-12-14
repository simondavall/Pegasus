using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace PegasusApi.Library.Models.Manage
{
    public class ManageBaseModel
    {
        public List<IdentityError> Errors { get; set; } = new List<IdentityError>();
        public string UserId { get; set; }
    }
}
