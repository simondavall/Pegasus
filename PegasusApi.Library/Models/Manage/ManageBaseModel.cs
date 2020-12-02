using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace PegasusApi.Library.Models.Manage
{
    public class ManageBaseModel
    {
        public IEnumerable<IdentityError> Errors { get; set; }
        public string UserId { get; set; }
    }
}
