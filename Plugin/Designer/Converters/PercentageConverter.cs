using System;
using System.Globalization;
using System.Windows.Data;

namespace Designer.Converters
{
    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return 0d;

            if (parameter == null)
                return value;

            double percentage = System.Convert.ToDouble(parameter);
            double val = System.Convert.ToDouble(value);

            return percentage * val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
