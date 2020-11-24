namespace PegasusApi.Models.Account
{
    public class ForgotPasswordModel
    {
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string Email { get; set; }
        }

        public string BaseUrl { get; set; }
    }
}
