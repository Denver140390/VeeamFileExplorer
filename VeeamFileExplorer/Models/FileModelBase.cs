﻿using System;
using System.Drawing;
using VeeamFileExplorer.Helpers;

namespace VeeamFileExplorer.Models
{
    abstract class FileModelBase : PropertyChangedBase
    {
        private string _name;
        private string _fullPath;
        private DateTime _changedDate;
        private long _size;
        private string _extension;
        private Icon _icon;

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value, () => Name); }
        }

        public string FullPath
        {
            get { return _fullPath; }
            set { SetProperty(ref _fullPath, value, () => FullPath); }
        }

        //TODO It is calculated in a wrong way
        public DateTime ChangedDate
        {
            get { return _changedDate; }
            set { SetProperty(ref _changedDate, value, () => ChangedDate); }
        }

        public long Size
        {
            get { return _size; }
            set { SetProperty(ref _size, value, () => Size); }
        }

        public string Extension
        {
            get { return _extension; }
            set { SetProperty(ref _extension, value, () => Extension); }
        }

        //TODO Icon type in Model... Is not it View related?
        public Icon Icon
        {
            get { return _icon; }
            set { SetProperty(ref _icon, value, () => Icon); }
        }
    }
}
