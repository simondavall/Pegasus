using System.Collections.Generic;

namespace PegasusApi.Library.Models.Manage
{
    public class UserDetailsModel : ManageBaseModel
    {
        public string UserId  { get; set; }
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
        public IList<string> Errors { get; } = new List<string>();
    }
}
