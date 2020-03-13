using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JMS.Setting
{
    public class Messages
    {
        public const string Intialization = "System Has been intialized.";
        public const string InvalidLoginAttempt = "Invalid email or password.";
        public const string TenantNotAvailiable = "Journal path is not availiable.";
        public const string EmailNotAvailiable = "Email is already in use.";
        public const string AlphaNumericValidationMessage = "Only Alphabets and Numbers allowed.";
        public const string SuccessSettingMessage = "Settings have been saved successfully";
        public const string SuccessProfileMessage = "Profile has been saved successfully.";
        public const string SuccessPasswordRecoverEmailMessage = "Password recovery link has been emailed. Click the link in the email to recover your password";
        public const string SuccessPasswordChangeMessage = "Password has been changed successfully.";
    }
}
