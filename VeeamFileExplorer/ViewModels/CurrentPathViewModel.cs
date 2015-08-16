using System;
using System.Diagnostics;
using VeeamFileExplorer.Helpers;

namespace VeeamFileExplorer.ViewModels
{
    class CurrentPathViewModel : PropertyChangedBase
    {
        private string _value;
        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value, () => Value); }
        }

        public void OpenInWindowsExplorer()
        {
            Process.Start(_value);
        }

        public void LoadNewPath(string newPath)
        {
            Value = newPath;
            RaiseOnPathChangedEvent();
        }

        public delegate void OnPathChangedEventHandler(object sender, EventArgs e);
        public event OnPathChangedEventHandler OnPathChangedEvent;
        protected virtual void RaiseOnPathChangedEvent()
        {
            OnPathChangedEvent?.Invoke(this, new EventArgs());
        }

        #region Singleton

        private static CurrentPathViewModel _instance;

        private CurrentPathViewModel()
        {

        }

        public static CurrentPathViewModel Instance => _instance ?? (_instance = new CurrentPathViewModel());

        #endregion Singleton
    }
}
