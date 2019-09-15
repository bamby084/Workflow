using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Designer
{
    /// <summary>
    /// Interaction logic for CustomColorPicker.xaml
    /// </summary>
    public partial class CustomColorPicker : UserControl
    {
        public CustomColorPicker()
        {
            InitializeComponent();
        }


        public Brush SelectedColor
        {
            get { return (Brush)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Brush), typeof(CustomColorPicker), new UIPropertyMetadata(null));
       
        
    }
}
