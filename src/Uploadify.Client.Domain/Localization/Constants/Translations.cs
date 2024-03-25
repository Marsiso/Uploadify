namespace Uploadify.Client.Domain.Localization.Constants;

public static class Translations
{
    public static class Common
    {
        public const string BaseCommon = "common";
        public const string AppName = $"{BaseCommon}.app_name";
        public const string TranslationNotFound = $"{BaseCommon}.translation_not_found";
        public const string Folder = $"{BaseCommon}.folder";
        public const string File = $"{BaseCommon}.file";
    }

    public static class Components
    {
        public const string BaseComponents = "components";

        public static class AuthenticatedLayout
        {
            public const string BaseAuthenticatedLayout = $"{BaseComponents}.authenticated_layout";
            public const string ProfileLink = $"{BaseAuthenticatedLayout}.profile_link";
            public const string FullnameLabel = $"{BaseAuthenticatedLayout}.fullname_label";
            public const string EmailLabel = $"{BaseAuthenticatedLayout}.email_label";
            public const string LogOutButton = $"{BaseAuthenticatedLayout}.logout_button";
            public const string DashboardLink = $"{BaseAuthenticatedLayout}.dashboard_link";
            public const string SharedLink = $"{BaseAuthenticatedLayout}.shared_link";
            public const string AboutLink = $"{BaseAuthenticatedLayout}.about_link";
            public const string PrivacyLink = $"{BaseAuthenticatedLayout}.privacy_link";
            public const string ManagementLink = $"{BaseAuthenticatedLayout}.management_link";
            public const string MoreLink = $"{BaseAuthenticatedLayout}.more_link";
        }

        public static class PermissionDashboard
        {
            public const string BasePermissionDashboard = $"{BaseComponents}.permission_dashboard";
            public const string Title = $"{BasePermissionDashboard}.title";
            public const string RoleNameLabel = $"{BasePermissionDashboard}.role_name_label";
            public const string DateUpdatedLabel = $"{BasePermissionDashboard}.date_updated_label";
            public const string EditLabel = $"{BasePermissionDashboard}.edit_label";
            public const string RoleDetailTitle = $"{BasePermissionDashboard}.role_detail_title";

            public static class UserUpdatedBy
            {
                public const string BaseUserUpdatedBy = $"{BasePermissionDashboard}.user_updated_by";
                public const string UserNameLabel = $"{BaseUserUpdatedBy}.username_label";
            }
        }

        public static class PermissionSelector
        {
            public const string BasePermissionSelector = $"{BaseComponents}.permission_selector";
            public const string Label = $"{BasePermissionSelector}.label";
        }

        public static class RoleSelector
        {
            public const string BaseRoleSelector = $"{BaseComponents}.role_selector";
            public const string Label = $"{BaseRoleSelector}.label";
        }

        public static class PermissionAssignment
        {
            public const string BasePermissionAssignment = $"{BaseComponents}.permission_assignment";
            public const string Title = $"{BasePermissionAssignment}.title";
            public const string Description = $"{BasePermissionAssignment}.description";
            public const string SubmitButton = $"{BasePermissionAssignment}.submit_button";
        }

        public static class Files
        {
            public const string BaseFiles = $"{BaseComponents}.files";

            public static class Dialogs
            {
                public const string BaseDialogs = $"{BaseFiles}.dialogs";

                public static class Create
                {
                    public const string BaseCreate = $"{BaseDialogs}.create";
                    public const string Title = $"{BaseCreate}.title";
                    public const string NameLabel = $"{BaseCreate}.name_label";
                    public const string SubmitButton = $"{BaseCreate}.submit_button";
                }

                public static class Delete
                {
                    public const string BaseDelete = $"{BaseDialogs}.delete";
                    public const string Title = $"{BaseDelete}.title";
                    public const string Description = $"{BaseDelete}.description";
                    public const string SubmitButton = $"{BaseDelete}.submit_button";
                }

                public static class Move
                {
                    public const string BaseMove = $"{BaseDialogs}.move";
                    public const string Title = $"{BaseMove}.title";
                    public const string FolderSelectLabel = $"{BaseMove}.folder_select_label";
                    public const string SubmitButton = $"{BaseMove}.submit_button";
                }

                public static class Rename
                {
                    public const string BaseRename = $"{BaseDialogs}.rename";
                    public const string Title = $"{BaseRename}.title";
                    public const string NameLabel = $"{BaseRename}.name_label";
                    public const string SubmitButton = $"{BaseRename}.submit_button";
                }
            }

            public static class Inputs
            {
                public const string BaseInputs = $"{BaseFiles}.inputs";

                public static class DragAndDropWrapper
                {
                    public const string BaseDragAndDropWrapper = $"{BaseFiles}.drag_and_drop_wrapper";
                    public const string UploadSuccess = $"{BaseDragAndDropWrapper}.uplaod_success";
                    public const string UploadFailure = $"{BaseDragAndDropWrapper}.uplaod_failure";
                    public const string FileTooLarge = $"{BaseDragAndDropWrapper}.file_too_large";
                    public const string TooManyFiles = $"{BaseDragAndDropWrapper}.too_many_files";
                }
            }

            public static class DashboardRowOptions
            {
                public const string BaseDashboardRowOptions = $"{BaseFiles}.dashboard_row_options";
                public const string DetailLink = $"{BaseDashboardRowOptions}.detail_link";
                public const string OpenLink = $"{BaseDashboardRowOptions}.open_link";
                public const string DownloadLink = $"{BaseDashboardRowOptions}.download_link";
                public const string MoveLink = $"{BaseDashboardRowOptions}.move_link";
                public const string RenameLink = $"{BaseDashboardRowOptions}.rename_link";
                public const string DeleteLink = $"{BaseDashboardRowOptions}.delete_link";
                public const string ShareLink = $"{BaseDashboardRowOptions}.share_link";
                public const string HideLink = $"{BaseDashboardRowOptions}.hide_link";
                public const string PublishLink = $"{BaseDashboardRowOptions}.publish_link";
            }

            public static class Detail
            {
                public const string BaseDetail = $"{BaseFiles}.detail";
                public const string RenameButton = $"{BaseDetail}.rename_button";
                public const string DateCreatedLabel = $"{BaseDetail}.date_created_label";
                public const string UserCreatedByLabel = $"{BaseDetail}.user_created_by_label";
                public const string DateUpdatedLabel = $"{BaseDetail}.date_updated_label";
                public const string UserUpdatedByLabel = $"{BaseDetail}.user_updated_by_label";
                public const string DownloadLabel = $"{BaseDetail}.download_label";
                public const string DownloadButton = $"{BaseDetail}.download_button";
                public const string ResourceTypeLabel = $"{BaseDetail}.resource_type_label";
                public const string LocationLabel = $"{BaseDetail}.location_label";
                public const string MoveButton = $"{BaseDetail}.move_button";
                public const string ExtensionLabel = $"{BaseDetail}.extension_label";
                public const string MimeLabel = $"{BaseDetail}.mime_label";
                public const string SizeLabel = $"{BaseDetail}.size_label";
                public const string DriveSizeLabel = $"{BaseDetail}.drive_size_label";
                public const string DeleteButton = $"{BaseDetail}.delete_button";
                public const string FolderItemsCountLabel = $"{BaseDetail}.folder_items_count_label";
                public const string OpenSubfolderLabel = $"{BaseDetail}.open_subfolder_label";
                public const string SharedLabel = $"{BaseDetail}.shared_label";
                public const string ShareButton = $"{BaseDetail}.share_button";
                public const string FolderNameLabel = $"{BaseDetail}.folder_name_label";
                public const string FolderTypeText = $"{BaseDetail}.folder_type_text";
                public const string FileNameLabel = $"{BaseDetail}.file_name_label";
                public const string FileTypeText = $"{BaseDetail}.file_type_text";
                public const string ContactText = $"{BaseDetail}.contact_text";
                public const string PublicAccessibilityText = $"{BaseDetail}.public_accessibility_text";
                public const string PrivateAccessibilityText = $"{BaseDetail}.private_accessibility_text";
            }
        }
    }

    public static class Pages
    {
        public const string BasePages = "pages";

        public static class Dashboard
        {
            public const string BaseDashboard = $"{BasePages}.dashboard";
            public const string Title = $"{BaseDashboard}.title";
            public const string VisibilityChangedSuccess = $"{BaseDashboard}.visibility_changed.success";
            public const string VisibilityChangedFailure = $"{BaseDashboard}.visibility_changed.failure";
        }

        public static class Privacy
        {
            public const string BasePrivacy = $"{BasePages}.privacy";
            public const string Title = $"{BasePrivacy}.title";
            public const string Description = $"{BasePrivacy}.description";
        }

        public static class About
        {
            public const string BaseAbout = $"{BasePages}.about";
            public const string Title = $"{BaseAbout}.title";

            public static class Sections
            {
                public const string BaseAboutSections = $"{BaseAbout}.sections";
                public const string FirstSection = $"{BaseAboutSections}.first";
                public const string SecondSection = $"{BaseAboutSections}.second";
                public const string ThirdSection = $"{BaseAboutSections}.third";
                public const string FourthSection = $"{BaseAboutSections}.fourth";
                public const string FifthSection = $"{BaseAboutSections}.fifth";
            }
        }

        public static class Profile
        {
            public const string BaseProfile = $"{BasePages}.profile";
            public const string Title = $"{BaseProfile}.title";
            public const string Description = $"{BaseProfile}.description";
            public const string UserNameLabel = $"{BaseProfile}.username_label";
            public const string EmailLabel = $"{BaseProfile}.email_label";
            public const string GivenNameLabel = $"{BaseProfile}.given_name_label";
            public const string FamilyNameLabel = $"{BaseProfile}.family_name_label";
            public const string PhoneLabel = $"{BaseProfile}.phone_label";
            public const string ContactInformationLabel = $"{BaseProfile}.contact_information_label";
            public const string PersonalInformationLabel = $"{BaseProfile}.personal_information_label";
        }

        public static class Management
        {
            public const string BaseManagement = $"{BasePages}.management";
            public const string Title = $"{BaseManagement}.title";
            public const string Description = $"{BaseManagement}.description";
            public const string UsersLabel = $"{BaseManagement}.users.label";
            public const string UsersDescription = $"{BaseManagement}.users.description";
            public const string PermissionsLabel = $"{BaseManagement}.permissions.label";
            public const string PermissionsDescription = $"{BaseManagement}.permissions.description";
        }

        public static class Shared
        {
            public const string BaseShared = $"{BasePages}.shared";
            public const string Title = $"{BaseShared}.title";
            public const string Description = $"{BaseShared}.description";

            public const string DownloadButton = $"{BaseShared}.download.button";
            public const string FilenameLabel = $"{BaseShared}.filename_label";
            public const string SearchLabel = $"{BaseShared}.search_label";
            public const string MatchesNotFound = $"{BaseShared}.matches_not_found";
            public const string DownloadSuccess = $"{BaseShared}.download.success";
            public const string DownloadFailure = $"{BaseShared}.download.failure";
            public const string PaginationFormat = $"{BaseShared}.pagination_format";
        }

        public static class More
        {
            public const string BaseMore = $"{BasePages}.more";
            public const string Title = $"{BaseMore}.title";
            public const string SharedLink = $"{BaseMore}.shared_link";
            public const string SharedDescription = $"{BaseMore}.shared_description";
            public const string AboutLink = $"{BaseMore}.about_link";
            public const string AboutDescription = $"{BaseMore}.about_description";
            public const string ManagementLink = $"{BaseMore}.management_link";
            public const string ManagementDescription = $"{BaseMore}.management_description";
            public const string DashboardLink = $"{BaseMore}.dashboard_link";
            public const string DashboardDescription = $"{BaseMore}.dashboard_description";
            public const string PrivacyLink = $"{BaseMore}.privacy_link";
            public const string PrivacyDescription = $"{BaseMore}.privacy_description";
        }
    }

    public static class Authorization
    {
        public const string BaseAuthorization = "authorization";

        public static class Permissions
        {
            public const string BasePermissions = $"{BaseAuthorization}.permissions";
            public const string None = $"{BasePermissions}.none";
            public const string ViewRoles = $"{BasePermissions}.view_roles";
            public const string EditRoles = $"{BasePermissions}.edit_roles";
            public const string ViewPermissions = $"{BasePermissions}.view_permissions";
            public const string EditPermissions = $"{BasePermissions}.edit_permissions";
            public const string ViewUsers = $"{BasePermissions}.view_users";
            public const string EditUsers = $"{BasePermissions}.edit_users";
            public const string ViewFiles = $"{BasePermissions}.view_files";
            public const string EditFiles = $"{BasePermissions}.edit_files";
            public const string All = $"{BasePermissions}.all";
        }
    }

    public static class Validations
    {
        public const string BaseValidations = "validations";
        public const string PermissionRequired = $"{BaseValidations}.permission.required";
        public const string RoleNameRequired = $"{BaseValidations}.role_name.required";
        public const string FolderNameExists = $"{BaseValidations}.folder_name.exists";
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
}
