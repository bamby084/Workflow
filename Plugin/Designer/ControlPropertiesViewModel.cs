using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Designer
{
    public class ControlPropertiesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private double _left;
        public double Left
        {
            get => _left;
            set
            {
                _left = value;
                NotifyPropertyChanged();
            }
        }

        private double _top;
        public double Top
        {
            get => _top;
            set
            {
                _top = value;
                NotifyPropertyChanged();
            }
        }

        private double _width;
        public double Width
        {
            get => _width;
            set
            {
                _width = value;
                NotifyPropertyChanged();
            }
        }

        private double _height;
        public double Height
        {
            get => _height;
            set
            {
                _height = value;
                NotifyPropertyChanged();
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }
    }
}
