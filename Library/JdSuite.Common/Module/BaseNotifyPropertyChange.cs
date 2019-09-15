using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JdSuite.Common
{
    public class NotifyPropertyChangeBase : INotifyPropertyChanged, INotifyPropertyChanging
    {


        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;


        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        protected virtual void OnPropertyChanging([CallerMemberName]string propertyName = null)
        {

            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        protected bool SetPropertry<T>(ref T Storage, T Value, [CallerMemberName] string Propertyname = null)
        {
            if (EqualityComparer<T>.Default.Equals(Storage, Value)) return false;
            Storage = Value;
            OnPropertyChanged(Propertyname);
            return true;
        }

        protected virtual bool PopertyChangedCallBack(string Propertyname)
        {
            return true;
        }


    }
}
