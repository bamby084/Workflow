using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Designer.Converters
{
    public class FigureLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var unitType = FigureUnitType.Page;
            if (parameter != null && parameter is FigureUnitType)
                unitType = (FigureUnitType)parameter;

            return new FigureLength((double)value / 100, unitType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
