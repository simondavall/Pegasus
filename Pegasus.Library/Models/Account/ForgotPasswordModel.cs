using System.ComponentModel.DataAnnotations;

namespace Pegasus.Library.Models.Account
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string BaseUrl { get; set; }
    }
}
