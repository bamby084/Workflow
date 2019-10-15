using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;

namespace Designer.DesignerTools
{
    public abstract class DesignerTool: INotifyPropertyChanged
    {
        public DesignerCanvas Canvas { get; set; }

        public virtual Cursor Cursor
        {
            get => Cursors.Arrow;
        }

        public virtual  ImageSource Image { get; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                NotifyPropertyChanged();
            }
        }

        public DesignerTool()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string properyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(properyName));
        }

        public virtual void HandleMouseLeftButtonDown(MouseButtonEventArgs e)
        {
        }

        public virtual void HandleMouseLeftButtonUp(MouseButtonEventArgs e)
        {
        }

        public virtual void HandleMouseMove(MouseEventArgs e)
        {

        }

        public virtual void HandleKeyDown(KeyEventArgs e)
        {

        }
    }
}
