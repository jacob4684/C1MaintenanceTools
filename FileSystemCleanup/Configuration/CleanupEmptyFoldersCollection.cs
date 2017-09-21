using System.Configuration;

namespace C1MaintenanceTools.FileSystemCleanup.Configuration
{
    public class CleanupEmptyFoldersCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.AddRemoveClearMap;

        public CleanupEmptyFoldersElement this[int index]
        {
            get => (CleanupEmptyFoldersElement) BaseGet(index);
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public void Add(CleanupEmptyFoldersElement element)
        {
            BaseAdd(element);
        }

        public void Clear()
        {
            BaseClear();
        }

        public void Remove(CleanupEmptyFoldersElement element)
        {
            BaseRemove(element);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new CleanupEmptyFoldersElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var e = (CleanupEmptyFoldersElement) element;

            return e.Path;
        }
    }
}