using System;
using VeeamFileExplorer.Helpers;

namespace VeeamFileExplorer.Models
{
    abstract class FileModelBase : PropertyChangedBase
    {
        private string _name;
        private string _path;
        private DateTime _changedDate;
        private int _size;

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

        public DateTime ChangedDate
        {
            get { return _changedDate; }
            set { SetProperty(ref _changedDate, value, () => ChangedDate); }
        }

        public int Size
        {
            get { return _size; }
            set { SetProperty(ref _size, value, () => Size); }
        }
    }
}
