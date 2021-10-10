namespace Pegasus.Library.Services.Resources
{
    public static class Resources
    {
        public static class ControllerStrings
        {
            public static class ManageController
            {
                public static string AuthenticatorAppVerified => "Your authenticator app has been verified.";
                public static string CannotChangePassword => "Cannot change password at this time.";

                public static string CannotEnableAuthenticator => "Cannot enable Authenticator at this moment.";
                public static string CannotEnableAuthenticatorRecoveryCodes => "The was a problem checking recovery codes. Try repeating the process.";
                public static string CannotEnableAuthenticatorSet2FaEnabled => "Cannot enable Authenticator at the moment - Problem setting 2Fa enabled.";
                public static string CannotEnableAuthenticatorTokenInvalid => "Cannot enable Authenticator at the moment - Token invalid.";
                public static string CannotEnableAuthenticatorVerifyToken => "Cannot enable Authenticator at the moment - Problem verifying token.";

                public static string CannotGenerateRecoveryCodes => "Cannot generate recovery codes at the moment - Problem generating codes.";
                public static string CannotGenerateRecoveryCodes2FaCheck => "Cannot generate recovery codes at the moment - Problem checking 2Fa enabled.";
                public static string CannotGenerateRecoveryCodesNotEnabled => "Cannot generate recovery codes because 2FA is not enabled.";

                public static string CannotUpdateUserDetails = "Cannot update user details at the moment.";
                public static string FailedToChangePassword => "Failed to change password.";
                public static string FailedToDisable2Fa => "Failed to disable Two Factor Authentication.";


                public static string PasswordChangedSuccess => "Your password has been changed.";

                public static string TwoFactorNotEnabled => "Two factor authentication is not currently enabled.";
            }
        }






    }
}
