using System.Configuration;

namespace C1MaintenanceTools.FileSystemCleanup.Configuration
{
    public class CleanupEmptyFoldersElement : ConfigurationElement
    {
        [ConfigurationProperty("path", IsRequired = true)]
        public string Path
        {
            get => (string) this["path"];
            set => this["path"] = value;
        }
    }
}