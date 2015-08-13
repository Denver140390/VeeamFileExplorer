namespace VeeamFileExplorer.Models
{
    class FolderModel : FileModelBase
    {
        public bool HasSubfolders { get; set; }
        public bool IsAccessible { get; set; }
    }
}
