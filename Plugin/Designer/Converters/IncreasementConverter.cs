using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Designer.Converters
{
    public class IncreasementConverter : IValueConverter
    {
        private readonly Dictionary<UnitType, PixelConverter> _pixelConverters = new Dictionary<UnitType, PixelConverter>();

        public IncreasementConverter()
        {
            _pixelConverters.Add(UnitType.Meter, new PixelToMeterConverter());
            _pixelConverters.Add(UnitType.Centimeter, new PixelToCentimeterConverter());
            _pixelConverters.Add(UnitType.Millimeter, new PixelToMillimeterConverter());
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var unitType = (UnitType) value;
            decimal increaseInPixel = System.Convert.ToDecimal(parameter);

            if (!_pixelConverters.ContainsKey(unitType))
                return 1;

            return _pixelConverters[unitType].Convert(increaseInPixel);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
