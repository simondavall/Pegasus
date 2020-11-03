﻿namespace Pegasus.Library.Models
{
    public class AuthenticatedUser
    {
        public string AccessToken { get; set; }
        public string Username { get; set; }
        public bool Succeeded { get; set; }
        public string FailedReason { get; set; }
    }
}
