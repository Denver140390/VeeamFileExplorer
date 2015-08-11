using VeeamFileExplorer.Helpers;

namespace VeeamFileExplorer.Models
{
    abstract class FileModelBase : PropertyChangedBase
    {
        private string _name;
        private string _path;

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value, () => Name); }
        }

        public string Path
        {
            get { return _path; }
            set { SetProperty(ref _path, value, () => Path); }
        }
    }
}
