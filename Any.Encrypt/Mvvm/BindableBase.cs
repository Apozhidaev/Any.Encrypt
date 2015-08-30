using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Ap.Encrypt.Mvvm
{
    public abstract class BindableBase : INotifyPropertyChanged
    {
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;
            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            RaisePropertyChanged(propertyName);
        }

        protected void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            RaisePropertyChanged(ExtractPropertyName<T>(propertyExpression));
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler changedEventHandler = PropertyChanged;
            if (changedEventHandler == null)
                return;
            changedEventHandler(this, new PropertyChangedEventArgs(propertyName));
        }

        private static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");
            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("NotMemberAccessExpression");
            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
                throw new ArgumentException("ExpressionNotProperty");
            if (propertyInfo.GetMethod.IsStatic)
                throw new ArgumentException("StaticExpression");
            return memberExpression.Member.Name;
        }
    }
}
