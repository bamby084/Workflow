using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Designer.DesignerTools
{
    public class DesignerToolBarItem: Button
    {
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
           "IsSelected", typeof(bool), typeof(DesignerToolBarItem), new PropertyMetadata(false, new PropertyChangedCallback(OnSelectedChanged)));

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public DesignerToolBarItem()
        {
            Background = Brushes.Transparent;
            BorderThickness = new Thickness(0);
            this.Click += OnClick;
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            IsSelected = true;
        }

        private void HandleSelectedChanged(bool value)
        {
            if(value)
            {
                //Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF3843C4"));
                Background = SystemColors.GradientActiveCaptionBrush;
            }
            else
            {
                Background = Brushes.Transparent;
            }
        }

        private static void OnSelectedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((DesignerToolBarItem)sender).HandleSelectedChanged((bool)e.NewValue);
        }
    }
}
