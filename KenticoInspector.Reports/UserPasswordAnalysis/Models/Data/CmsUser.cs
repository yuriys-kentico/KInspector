using Newtonsoft.Json;

namespace KenticoInspector.Reports.UserPasswordAnalysis.Models.Data
{
    public class CmsUser
    {
        public int UserID { get; set; }

        public string UserName { get; set; } = null!;

        public string? Email { get; set; }

        [JsonIgnore]
        public string UserPassword { get; set; } = null!;

        [JsonIgnore]
        public string? UserPasswordFormat { get; set; }

        public string UserPrivilegeLevel { get; set; } = null!;

        [JsonIgnore]
        public string? FirstName { get; set; }

        [JsonIgnore]
        public string? MiddleName { get; set; }

        [JsonIgnore]
        public string? LastName { get; set; }

        public string? FullName { get; set; }
    }
}