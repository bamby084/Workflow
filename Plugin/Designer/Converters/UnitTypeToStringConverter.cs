using System;
using System.Globalization;
using System.Windows.Data;
using Designer.ExtensionMethods;

namespace Designer.Converters
{
    public class UnitTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            return ((UnitType)value).GetDescription();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
