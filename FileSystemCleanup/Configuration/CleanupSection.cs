using System.Configuration;

namespace C1MaintenanceTools.FileSystemCleanup.Configuration
{
    public class CleanupSection : ConfigurationSection
    {
        private const string ConfigPath = "c1MaintenanceTools/fileSystemCleanup";

        [ConfigurationProperty("files", IsRequired = false)]
        public CleanupFilesCollection Files
        {
            get => (CleanupFilesCollection) this["files"];
            set => this["files"] = value;
        }

        [ConfigurationProperty("emptyfolders", IsRequired = false)]
        public CleanupEmptyFoldersCollection EmptyFolders
        {
            get => (CleanupEmptyFoldersCollection) this["emptyfolders"];
            set => this["emptyfolders"] = value;
        }

        public static CleanupSection GetSection()
        {
            return ConfigurationManager.GetSection(ConfigPath) as CleanupSection;
        }
    }
}