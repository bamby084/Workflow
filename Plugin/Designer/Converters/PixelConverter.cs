using System;
using System.Globalization;
using System.Windows.Data;

namespace Designer.Converters
{
    public abstract class PixelConverter :  IValueConverter
    {
        protected const double DotsPerInch = 96d;
        protected const double MillimetersPerInch = 25.4d;

        public abstract string Unit { get; }

        public virtual double Factor { get; } = 1.0d;

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return 0;

            double val = (double) value;
            return val * Factor;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return 0;

            double val = System.Convert.ToDouble(value);
            return val / Factor;
        }
    }
}
