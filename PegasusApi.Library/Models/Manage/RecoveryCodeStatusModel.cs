using System.Collections.Generic;

namespace PegasusApi.Library.Models.Manage
{
    public class RecoveryCodeStatusModel : ManageBaseModel
    {
        public bool IsUpdated { get; set; }
        public IEnumerable<string> RecoveryCodes { get; set; }
     }
}
