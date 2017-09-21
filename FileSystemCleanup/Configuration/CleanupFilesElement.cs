using System.Configuration;

namespace C1MaintenanceTools.FileSystemCleanup.Configuration
{
    public class CleanupFilesElement : ConfigurationElement
    {
        [ConfigurationProperty("path", IsRequired = true)]
        public string Path
        {
            get => (string) this["path"];
            set => this["path"] = value;
        }

        [ConfigurationProperty("extensions", IsRequired = false)]
        public string Extensions
        {
            get => (string) this["extensions"];
            set => this["extensions"] = value;
        }

        [ConfigurationProperty("ignore-extensions", IsRequired = false)]
        public string IgnoreExtensions
        {
            get => (string) this["ignore-extensions"];
            set => this["ignore-extensions"] = value;
        }

        [ConfigurationProperty("obsolescencedays", IsRequired = true)]
        public int DbsolescenceDays
        {
            get => (int) this["obsolescencedays"];
            set => this["obsolescencedays"] = value;
        }
    }
}