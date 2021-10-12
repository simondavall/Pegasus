using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Pegasus.Library.Models.Manage
{
    public class ManageBaseModel
    {
        public IEnumerable<IdentityError> Errors { get; set; }
        public bool HasErrors { get { return Errors != null && Errors.Any(); }}
        public string StatusMessage { get; set; }
        public string UserId { get; set; }
    }
}
