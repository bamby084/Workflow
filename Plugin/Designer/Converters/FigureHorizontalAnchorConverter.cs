using Designer.DesignerItems;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Designer.Converters
{
    public class FigureHorizontalAnchorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.GetType() != typeof(DesignerTableAlignment))
                return Binding.DoNothing;

            var alignment = (DesignerTableAlignment)value;
            return (FigureHorizontalAnchor)((int)alignment);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
