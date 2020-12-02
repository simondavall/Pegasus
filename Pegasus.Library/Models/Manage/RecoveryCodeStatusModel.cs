using System.Collections.Generic;

namespace Pegasus.Library.Models.Manage
{
    public class RecoveryCodeStatusModel : ManageBaseModel
    {
        public bool IsUpdated { get; set; }
        public IEnumerable<string> RecoveryCodes { get; set; }
     }
}
