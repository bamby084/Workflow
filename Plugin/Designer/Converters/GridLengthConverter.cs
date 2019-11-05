using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Designer.Converters
{
    public class GridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var gridUnitType = GridUnitType.Star;
            if (parameter != null && parameter is GridUnitType)
                gridUnitType = (GridUnitType)parameter;

            return new GridLength((double)value * 100, gridUnitType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
