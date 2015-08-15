namespace VeeamFileExplorer.Models
{
    class FolderModel : FileModelBase
    {
        public new string Extension { get; } = "Folder";

        public bool HasSubfolders { get; set; }
        public bool IsAccessible { get; set; }
    }
}
