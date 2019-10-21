using System;
using System.Globalization;
using System.Windows.Data;

namespace Designer.Converters
{
    public class UnitToDecimalConverter: UnitConverter
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            decimal value = System.Convert.ToDecimal(values[0]);
            UnitType unitType = (UnitType)values[1];

            if (!PixelConverters.ContainsKey(unitType))
                return value;

            return PixelConverters[unitType].Convert(value);
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            decimal val = System.Convert.ToDecimal(value);
            var unitType = UnitOfMeasure.Current.UnitType;

            if (!PixelConverters.ContainsKey(unitType))
                return new[] { Binding.DoNothing, Binding.DoNothing };

            return new[] {(double) PixelConverters[unitType].ConvertBack(val), Binding.DoNothing};
        }
    }
}
