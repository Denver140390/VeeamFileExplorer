using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace VeeamFileExplorer.Helpers
{
    //    public abstract class ModelBase : INotifyPropertyChanged
    //    {
    //        public event PropertyChangedEventHandler PropertyChanged;
    //        
    //        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    //        {
    //            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //        }
    //    }

    public abstract class PropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;

            propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T backingField, T value, Expression<Func<T>> propertyExpression)
        {
            var changed = !EqualityComparer<T>.Default.Equals(backingField, value);

            if (changed)
            {
                backingField = value;
                RaisePropertyChanged(ExtractPropertyName(propertyExpression));
            }

            return changed;
        }

        private static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            var memberExp = propertyExpression.Body as MemberExpression;

            if (memberExp == null)
            {
                throw new ArgumentException("Expression must be a MemberExpression.", nameof(propertyExpression));
            }

            return memberExp.Member.Name;
        }
    }
}
