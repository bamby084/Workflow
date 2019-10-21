using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Designer.Converters
{
    public abstract class UnitConverter : IMultiValueConverter
    {
        protected readonly Dictionary<UnitType, PixelConverter> PixelConverters = new Dictionary<UnitType, PixelConverter>();

        protected UnitConverter()
        {
            PixelConverters.Add(UnitType.Meter, new PixelToMeterConverter());
            PixelConverters.Add(UnitType.Centimeter, new PixelToCentimeterConverter());
            PixelConverters.Add(UnitType.Millimeter, new PixelToMillimeterConverter());
        }

        public virtual object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public virtual object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
