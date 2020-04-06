namespace KenticoInspector.Core.Models
{
    public class DatabaseSettings
    {
        public string Database { get; set; } = null!;

        public bool IntegratedSecurity { get; set; }

        public string Password { get; set; } = null!;

        public string Server { get; set; } = null!;

        public string User { get; set; } = null!;
    }
}