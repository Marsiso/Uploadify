namespace Uploadify.Server.Domain.Infrastructure.Localization.Constants;

public static class Translations
{
    public static class Validations
    {
        public const string BaseValidations = "validations";

        public const string UserNameRequired = $"{BaseValidations}.user_name.required";
        public const string UserNameMinLength = $"{BaseValidations}.user_name.min_length";
        public const string UserNameMaxLength = $"{BaseValidations}.user_name.max_length";
        public const string UserNameUnique= $"{BaseValidations}.user_name.duplicity";

        public const string EmailRequired = $"{BaseValidations}.email.required";
        public const string EmailMinLength = $"{BaseValidations}.email.min_length";
        public const string EmailMaxLength = $"{BaseValidations}.email.max_length";
        public const string EmailUnique= $"{BaseValidations}.email.duplicity";
        public const string EmailFormat= $"{BaseValidations}.email.invalid_format";

        public const string PhoneNumberRequired = $"{BaseValidations}.phone_number.required";
        public const string PhoneNumberMinLength = $"{BaseValidations}.phone_number.min_length";
        public const string PhoneNumberMaxLength = $"{BaseValidations}.phone_number.max_length";
        public const string PhoneNumberUnique= $"{BaseValidations}.phone_number.duplicity";
        public const string PhoneNumberFormat= $"{BaseValidations}.phone_number.invalid_format";

        public const string GivenNameRequired = $"{BaseValidations}.given_name.required";
        public const string GivenNameMinLength = $"{BaseValidations}.given_name.min_length";
        public const string GivenNameMaxLength = $"{BaseValidations}.given_name.max_length";

        public const string FamilyNameRequired = $"{BaseValidations}.family_name.required";
        public const string FamilyNameMinLength = $"{BaseValidations}.family_name.min_length";
        public const string FamilyNameMaxLength = $"{BaseValidations}.family_name.max_length";

        public const string PasswordRequired = $"{BaseValidations}.password.required";
        public const string PasswordMinLength = $"{BaseValidations}.password.min_length";
        public const string PasswordMaxLength = $"{BaseValidations}.password.max_length";
        public const string PasswordLowerCaseCharacter = $"{BaseValidations}.password.lower_case_character";
        public const string PasswordUpperCaseCharacter = $"{BaseValidations}.password.upper_case_character";
        public const string PasswordNumericCharacter = $"{BaseValidations}.password.numeric_character";
        public const string PasswordSpecialCharacter = $"{BaseValidations}.password.special_character";

        public const string PasswordRepeatMatch = $"{BaseValidations}.password_repeat.must_match";
        public const string PasswordRepeatRequired = $"{BaseValidations}.password_repeat.required";

        public const string UserLockedOut = BaseValidations + ".signin.locked_out";
        public const string InvalidLoginForm = BaseValidations + ".signin.invalid_user_name_or_password";

        public const string PermissionRequired = $"{BaseValidations}.permission.required";
        public const string PermissionIsNotInEnum = $"{BaseValidations}.permission.out_of_range";
        public const string Unauthorized= $"{BaseValidations}.permission.unauthorized";

        public const string RoleNameRequired = $"{BaseValidations}.role_name.required";
        public const string RoleNameMaxLength = $"{BaseValidations}.role_name.max_length";

        public const string FolderNameRequired = $"{BaseValidations}.folder_name.required";
        public const string FolderNameMaxLength = $"{BaseValidations}.folder_name.max_length";
        public const string FolderNameExists = $"{BaseValidations}.folder_name.exists";
        public const string FolderParentFolderIdRequired = $"{BaseValidations}.folder_parent_folder_id.required";

        public const string FileNameRequired = $"{BaseValidations}.file_name.required";
        public const string FileNameMaxLength = $"{BaseValidations}.file_name.max_length";
        public const string FileRequired= $"{BaseValidations}.file.required";
        public const string FileContentTypeRequired= $"{BaseValidations}.file_content_type.required";
        public const string FileContentTypeUnknown= $"{BaseValidations}.file_content_type.unknown";
        public const string FileLengthRequired= $"{BaseValidations}.file_length.required";
        public const string FileLengthTooLarge = $"{BaseValidations}.file_length.too_large";
        public const string FileNameExtensionRequired = $"{BaseValidations}.file_name_extension.required";
    }

    public static class RequestStatuses
    {
        public const string BaseRequestStatuses = "request_statuses";

        public const string BadRequest = $"{BaseRequestStatuses}.bad_request";
        public const string Forbidden = $"{BaseRequestStatuses}.forbidden";
        public const string NotFound = $"{BaseRequestStatuses}.not_found";
        public const string Unauthorized = $"{BaseRequestStatuses}.unauthorized";
        public const string ClientCancelledOperation = $"{BaseRequestStatuses}.client_canceled_operation";
        public const string InternalServerError = $"{BaseRequestStatuses}.internal_server_error";
    }

    public static class Pages
    {
        public const string BasePages = "pages";

        public static class Login
        {
            public const string BasePage = $"{BasePages}.login";
            public const string Title = $"{BasePage}.title";
            public const string SignInButton = $"{BasePage}.sign_in_button";
            public const string SignUpButton = $"{BasePage}.sign_up_button";
            public const string LoginLabel = $"{BasePage}.login_label";
            public const string PasswordLabel = $"{BasePage}.password_label";
        }

        public static class Logout
        {
            public const string BasePage = $"{BasePages}.logout";
            public const string Title = $"{BasePage}.title";
            public const string SubmitButton = $"{BasePage}.submit_button";
        }

        public static class Register
        {
            public const string BasePage= $"{BasePages}.register";
            public const string Title = $"{BasePage}.title";
            public const string SignInButton = $"{BasePage}.sign_in_button";
            public const string SignUpButton = $"{BasePage}.sign_up_button";
            public const string LoginLabel = $"{BasePage}.login_label";
            public const string PasswordLabel = $"{BasePage}.password_label";
            public const string ConfirmPasswordLabel = $"{BasePage}.password_repeat_label";
            public const string FirstNameLabel = $"{BasePage}.first_name_label";
            public const string LastNameLabel = $"{BasePage}.last_name_label";
            public const string PhoneNumberLabel = $"{BasePage}.phone_number_label";
            public const string EmailLabel = $"{BasePage}.email_label";
        }

        public static class Home
        {
            public const string BasePage= $"{BasePages}.home";
            public const string Title = $"{BasePage}.title";
            public const string SignInButton = $"{BasePage}.sign_in_button";
            public const string SignUpButton = $"{BasePage}.sign_up_button";
            public const string SignOutButton = $"{BasePage}.sign_out_button";
            public const string Greeting = $"{BasePage}.greeting";
        }

        public static class Authorize
        {
            public const string BasePage= $"{BasePages}.authorize";
            public const string Title = $"{BasePage}.title";
            public const string AcceptButton = $"{BasePage}.accept_button";
            public const string DenyButton = $"{BasePage}.deny_button";
        }
    }
}
