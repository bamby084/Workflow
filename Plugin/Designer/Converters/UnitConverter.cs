using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Designer.Converters
{
    public class UnitConverter : IMultiValueConverter
    {
        private readonly Dictionary<UnitType, PixelConverter> _pixelConverters = new Dictionary<UnitType, PixelConverter>();

        public UnitConverter()
        {
            _pixelConverters.Add(UnitType.Meter, new PixelToMeterConverter());
            _pixelConverters.Add(UnitType.Centimeter, new PixelToCentimeterConverter());
            _pixelConverters.Add(UnitType.Millimeter, new PixelToMillimeterConverter());
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            decimal value = System.Convert.ToDecimal(values[0]);
            UnitType unitType = (UnitType)values[1];

            if (!_pixelConverters.ContainsKey(unitType))
                return value;

            return _pixelConverters[unitType].Convert(value);
            
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            decimal val = System.Convert.ToDecimal(value);
            var unitType = UnitOfMeasure.Current.UnitType;

            if (!_pixelConverters.ContainsKey(unitType))
                return new[] { Binding.DoNothing, Binding.DoNothing };

            return new[] { (double) _pixelConverters[unitType].ConvertBack(val), Binding.DoNothing };
        }
    }
}
