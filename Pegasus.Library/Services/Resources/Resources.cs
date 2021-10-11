namespace Pegasus.Library.Services.Resources
{
    public static class Resources
    {
        public static class ControllerStrings
        {
            public static class AccountController
            {
                

                public static string CannotFind2FaUser => "Unable to load two-factor authentication user.";

                public static string InvalidAuthenticationCode => "Invalid authenticator code.";

                public static string InvalidLoginAttempt => "Invalid login attempt.";

                public static string InvalidRecoveryCode => "Invalid recovery code entered.";

            }
            public static class ManageController
            {
                public static string AuthenticatorAppVerified => "Your authenticator app has been verified.";

                public static string AuthenticatorResetSuccess =>
                    "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.";

                public static string CannotChangePassword => "Cannot change password at this time.";

                public static string CannotEnableAuthenticator => "Cannot enable Authenticator at this moment.";
                public static string CannotEnableAuthenticatorRecoveryCodes => "The was a problem checking recovery codes. Try repeating the process.";
                public static string CannotEnableAuthenticatorSet2FaEnabled => "Cannot enable Authenticator at the moment - Problem setting 2Fa enabled.";
                public static string CannotEnableAuthenticatorTokenInvalid => "Cannot enable Authenticator at the moment - Token invalid.";
                public static string CannotEnableAuthenticatorVerifyToken => "Cannot enable Authenticator at the moment - Problem verifying token.";

                public static string CannotGenerateRecoveryCodes => "Cannot generate recovery codes at the moment - Problem generating codes.";
                public static string CannotGenerateRecoveryCodes2FaCheck => "Cannot generate recovery codes at the moment - Problem checking 2Fa enabled.";
                public static string CannotGenerateRecoveryCodesNotEnabled => "Cannot generate recovery codes because 2FA is not enabled.";

                

                public static string CannotUpdateUserDetails => "Cannot update user details at the moment.";

                public static string CurrentBrowserForgotten =>  "The current browser has been forgotten. When you login again from this browser you will be prompted for your 2fa code.";

                public static string FailedToChangePassword => "Failed to change password.";
                public static string FailedToDisable2Fa => "Failed to disable Two Factor Authentication.";
                public static string FailedToResetAuthenticator => "Failed to reset Authenticator.";
                public static string FailedToSetPassword => "Failed to set password.";

                public static string PasswordChangedSuccess => "Your password has been changed.";
                public static string PasswordSetSuccess => "Your password has been set.";
                public static string TwoFactorNotEnabled => "Two factor authentication is not currently enabled.";

                public static string UserDetailsUpdated => "User details were updated.";
            }
        }






    }
}
