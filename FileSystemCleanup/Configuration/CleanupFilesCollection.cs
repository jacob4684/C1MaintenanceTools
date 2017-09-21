using System.Configuration;

namespace C1MaintenanceTools.FileSystemCleanup.Configuration
{
    public class CleanupFilesCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.AddRemoveClearMap;

        public CleanupFilesElement this[int index]
        {
            get => (CleanupFilesElement) BaseGet(index);
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public void Add(CleanupFilesElement element)
        {
            BaseAdd(element);
        }

        public void Clear()
        {
            BaseClear();
        }

        public void Remove(CleanupFilesElement element)
        {
            BaseRemove(element);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new CleanupFilesElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var e = (CleanupFilesElement) element;

            return e.Path;
        }
    }
}