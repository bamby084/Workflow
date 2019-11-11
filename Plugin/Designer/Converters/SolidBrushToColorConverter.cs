using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Designer.Converters
{
    public class SolidBrushToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Colors.White;

            if(value is SolidColorBrush brush)
            {
                return brush.Color;
            }

            throw new NotSupportedException("");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Brushes.White;

            if(value is Color color)
            {
                return new SolidColorBrush(color);
            }

            throw new NotSupportedException();
        }
    }
}
