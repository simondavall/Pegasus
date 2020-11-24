using System.Collections.Generic;

namespace Pegasus.Library.Models.Manage
{
    public class RecoveryCodeStatusModel : ManageBaseModel
    {
        public string Email { get; set; }
        public bool NeededReset { get; set; }
        public IEnumerable<string> RecoveryCodes { get; set; }
     }
}
