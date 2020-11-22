using System.Collections.Generic;

namespace PegasusApi.Library.Models.Manage
{
    public class GenerateRecoveryCodesModel : ManageBaseModel
    {
        public IEnumerable<string> RecoveryCodes { get; set; }
    }
}
