using System;
using System.Globalization;
using Designer.ExtensionMethods;

namespace Designer.Converters
{
    public class UnitToStringConverter: UnitConverter
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double value = System.Convert.ToDouble(values[0]);
            UnitType unitType = (UnitType)values[1];

            if (!PixelConverters.ContainsKey(unitType))
                return value;

            var convertedValue = Math.Round(PixelConverters[unitType].Convert((decimal) value), 2);
            return $"{convertedValue} {unitType.GetDescription()}";
        }
    }
}
