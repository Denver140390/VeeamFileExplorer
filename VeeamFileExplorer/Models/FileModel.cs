using System;

namespace VeeamFileExplorer.Models
{
    class FileModel : FileModelBase
    {
        private DateTime _changedDate;
        private int _size;

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
