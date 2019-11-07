using System;
using System.Globalization;
using System.Windows.Data;

namespace Designer.Converters
{
    public class IncreaseIntegerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val = System.Convert.ToInt32(value);
            int increasement = System.Convert.ToInt32(parameter);

            return val + increasement;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
