using System.Collections.Generic;

namespace Pegasus.Library.Models.Manage
{
    public class GenerateRecoveryCodesModel : ManageBaseModel
    {
        public IEnumerable<string> RecoveryCodes { get; set; }
    }
}
