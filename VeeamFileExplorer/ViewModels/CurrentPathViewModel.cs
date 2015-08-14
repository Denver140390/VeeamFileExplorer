using VeeamFileExplorer.Helpers;

namespace VeeamFileExplorer.ViewModels
{
    //TODO Allow input
    class CurrentPathViewModel : PropertyChangedBase
    {
        private string _value;
        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value, () => Value); }
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
