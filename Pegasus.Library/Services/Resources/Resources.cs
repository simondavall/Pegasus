namespace Pegasus.Library.Services.Resources
{
    public static class Resources
    {
        public static class ControllerStrings
        {
            public static class AccountController
            {
                public const string CannotFind2FaUser = "Unable to load two-factor authentication user.";
                public const string InvalidAuthenticationCode = "Invalid authenticator code.";
                public const string InvalidLoginAttempt = "Invalid login attempt.";
                public const string InvalidRecoveryCode = "Invalid recovery code entered.";
                public const string NoCodeForPasswordReset = "A code must be supplied for password reset.";
            }
            public static class ManageController
            {
                public const string AuthenticatorAppVerified = "Your authenticator app has been verified.";
                public const string AuthenticatorResetSuccess = "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.";
                public const string CannotChangePassword = "Cannot change password at this time.";
                public const string CannotEnableAuthenticator = "Cannot enable Authenticator at this moment.";
                public const string CannotEnableAuthenticatorRecoveryCodes = "The was a problem checking recovery codes. Try repeating the process.";
                public const string CannotEnableAuthenticatorSet2FaEnabled = "Cannot enable Authenticator at the moment - Problem setting 2Fa enabled.";
                public const string CannotEnableAuthenticatorTokenInvalid = "Cannot enable Authenticator at the moment - Token invalid.";
                public const string CannotEnableAuthenticatorVerifyToken = "Cannot enable Authenticator at the moment - Problem verifying token.";
                public const string CannotGenerateRecoveryCodes = "Cannot generate recovery codes at the moment - Problem generating codes.";
                public const string CannotGenerateRecoveryCodes2FaCheck = "Cannot generate recovery codes at the moment - Problem checking 2Fa enabled.";
                public const string CannotGenerateRecoveryCodesNotEnabled = "Cannot generate recovery codes because 2FA is not enabled.";
                public const string CannotUpdateUserDetails = "Cannot update user details at the moment.";
                public const string CurrentBrowserForgotten =  "The current browser has been forgotten. When you login again from this browser you will be prompted for your 2fa code.";
                public const string FailedToChangePassword = "Failed to change password.";
                public const string FailedToDisable2Fa = "Failed to disable Two Factor Authentication.";
                public const string FailedToResetAuthenticator = "Failed to reset Authenticator.";
                public const string FailedToSetPassword = "Failed to set password.";
                public const string PasswordChangedSuccess = "Your password has been changed.";
                public const string PasswordSetSuccess = "Your password has been set.";
                public const string TwoFactorNotEnabled = "Two factor authentication is not currently enabled.";
                public const string UserDetailsUpdated = "User details were updated.";
            }

            public static class TaskListController
            {
                public const string CannotCloseWithOpenSubTasks = "Update Failed: Cannot complete a task that still has open sub tasks.";
            }
        }

        public static class ViewStrings
        {
            public static class TaskListStrings
            {
                public const string NoTasksFound = "No Tasks Found";
            }
        }
    }
}
