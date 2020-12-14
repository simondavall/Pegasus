namespace PegasusApi.Models
{
    public class TokenModel
    {
        public string AccessToken { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public bool RequiresTwoFactor { get; set; }
    }

}
