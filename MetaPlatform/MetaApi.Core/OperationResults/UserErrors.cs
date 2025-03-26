using MetaApi.Core.OperationResults.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaApi.Core.OperationResults
{
    public static class UserErrors
    {
        public const string UserNotFoundCode = "User.NotFound";
        public const string UserIsBlockedCode = "User.UserIsBlocked";
        public const string AlreadyExistCode = "User.AlreadyExist";
        public const string FailedSaveCode = "User.FailedSave";
        public const string FailedLoginOrPasswordCode = "User.LoginFailOrPasswordFail";
        public const string DifferentPasswordCode = "User.DifferentPassword";
        public const string BlockedCode = "User.Blocked";
        public const string MissingUserIdCode = "User.MissingUserId";
        public const string EmailSendErrorCode = "User.EmailSendErrorCode";
        public const string EmailNotConfirmedCode = "User.EmailNotConfirmedCode";
        public const string EmailNotUniqueCode = "User.EmailNotUniqueCode";


        public static Error UserNotFound(string info) => new Error(UserNotFoundCode, $"{info} User not found");

        public static Error AlreadyExist() => new Error(AlreadyExistCode, "User already exists");

        public static Error UserIsBlocked() => new Error(UserIsBlockedCode, "User is blocked");

        public static Error FailedSave() => new Error(FailedSaveCode, "User failed save");

        public static Error FailedLoginOrPassword() => new Error(FailedLoginOrPasswordCode, "Invalid credentials");

        public static Error DifferentPassword() => new Error(DifferentPasswordCode, "Passwords are different");

        public static Error Blocked() => new Error(BlockedCode, "The user is blocked");

        public static Error MissingUserId() => new Error(MissingUserIdCode, "Missing user id in token");

        public static Error EmailSendError() => new Error(EmailSendErrorCode, $"Registration successful, but failed to send confirmation email. Please try to resend the confirmation email from your account settings");

        public static Error EmailNotConfirmed() => new Error(EmailNotConfirmedCode, $"Your mail is not confirmed");

        public static Error EmailNotUnique() => new Error(EmailNotUniqueCode, $"Email not unique");
    }
}
