using KenticoInspector.Core.TokenExpressions.Models;

namespace KenticoInspector.Reports.SecuritySettingsAnalysis.Models
{
    public class Terms
    {
        public Term GlobalSiteName { get; set; } = null!;

        public RecommendationReasons RecommendationReasons { get; set; } = null!;

        public RecommendedValues RecommendedValues { get; set; } = null!;

        public Summaries Summaries { get; set; } = null!;

        public TableTitles TableTitles { get; set; } = null!;
    }

    public class RecommendedValues
    {
        public Term Empty { get; set; } = null!;

        public Term InvalidLogonAttempts { get; set; } = null!;

        public Term NoDangerousExtensions { get; set; } = null!;

        public Term NotEmpty { get; set; } = null!;

        public Term NotOn { get; set; } = null!;

        public Term NotSaUser { get; set; } = null!;

        public Term ResetPasswordInterval { get; set; } = null!;

        public Term PasswordMinimalLength { get; set; } = null!;

        public Term PasswordNumberOfNonAlphaNumChars { get; set; } = null!;

        public Term ReCaptcha { get; set; } = null!;
    }

    public class RecommendationReasons
    {
        public AppSettings AppSettings { get; set; } = null!;

        public ConnectionStrings ConnectionStrings { get; set; } = null!;

        public SettingsKeys SettingsKeys { get; set; } = null!;

        public SystemWebSettings SystemWebSettings { get; set; } = null!;
    }

    public class AppSettings
    {
        public Term CMSEnableCsrfProtection { get; set; } = null!;

        public Term CMSHashStringSalt { get; set; } = null!;

        public Term CMSRenewSessionAuthChange { get; set; } = null!;

        public Term CMSXFrameOptionsExcluded { get; set; } = null!;
    }

    public class ConnectionStrings
    {
        public Term SaUser { get; set; } = null!;
    }

    public class SettingsKeys
    {
        public Term CMSAutocompleteEnableForLogin { get; set; } = null!;

        public Term CMSCaptchaControl { get; set; } = null!;

        public Term CMSChatEnableFloodProtection { get; set; } = null!;

        public Term CMSFloodProtectionEnabled { get; set; } = null!;

        public Term CMSForumAttachmentExtensions { get; set; } = null!;

        public Term CMSMaximumInvalidLogonAttempts { get; set; } = null!;

        public Term CMSMediaFileAllowedExtensions { get; set; } = null!;

        public Term CMSPasswordExpiration { get; set; } = null!;

        public Term CMSPasswordExpirationBehaviour { get; set; } = null!;

        public Term CMSPasswordFormat { get; set; } = null!;

        public Term CMSPolicyMinimalLength { get; set; } = null!;

        public Term CMSPolicyNumberOfNonAlphaNumChars { get; set; } = null!;

        public Term CMSRegistrationEmailConfirmation { get; set; } = null!;

        public Term CMSResetPasswordInterval { get; set; } = null!;

        public Term CMSRESTServiceEnabled { get; set; } = null!;

        public Term CMSUploadExtensions { get; set; } = null!;

        public Term CMSUsePasswordPolicy { get; set; } = null!;

        public Term CMSUseSSLForAdministrationInterface { get; set; } = null!;
    }

    public class SystemWebSettings
    {
        public Term AuthenticationCookieless { get; set; } = null!;

        public Term CompilationDebug { get; set; } = null!;

        public Term CustomErrorsMode { get; set; } = null!;

        public Term HttpCookiesHttpOnlyCookies { get; set; } = null!;

        public Term PagesEnableViewState { get; set; } = null!;

        public Term PagesEnableViewStateMac { get; set; } = null!;

        public Term TraceEnabled { get; set; } = null!;
    }

    public class Summaries
    {
        public Term Warning { get; set; } = null!;

        public Term Good { get; set; } = null!;
    }

    public class TableTitles
    {
        public Term AdminSecuritySettings { get; set; } = null!;

        public Term WebConfigSecuritySettings { get; set; } = null!;
    }
}