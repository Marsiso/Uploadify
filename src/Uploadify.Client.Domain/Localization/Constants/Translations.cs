namespace Uploadify.Client.Domain.Localization.Constants;

public static class Translations
{
    public static class Common
    {
        public const string BaseCommon = "common";
        public const string AppName = $"{BaseCommon}.app_name";
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
        }

        public static class RoleSelector
        {
            public const string BaseRoleSelector = $"{BaseComponents}.role_selector";
        }
    }

    public static class Pages
    {
        public const string BasePages = "pages";

        public static class Dashboard
        {
            public const string BaseDashboard = $"{BasePages}.dashboard";
            public const string Title = $"{BaseDashboard}.title";
        }

        public static class Detail
        {
            public const string BaseDetail = $"{BasePages}.detail";
            public const string Title = $"{BaseDetail}.title";
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
}
