using System.Collections.Generic;
using System.ComponentModel;
using Designer.Converters;

namespace Designer
{
    public class UnitOfMeasure: INotifyPropertyChanged
    {
        private static readonly Dictionary<UnitType, PixelConverter> PixelConverters;

        static UnitOfMeasure()
        {
            PixelConverters = new Dictionary<UnitType, PixelConverter>()
            {
                {UnitType.Meter, new PixelToMeterConverter()},
                {UnitType.Centimeter, new PixelToCentimeterConverter()},
                {UnitType.Millimeter, new PixelToMillimeterConverter()}
            };
        }

        public static UnitType UnitType { get; set; } = UnitType.Millimeter;

        public static PixelConverter PixelConverter => PixelConverters[UnitType];

        public event PropertyChangedEventHandler PropertyChanged;

        //private static readonly object LockObject = new object();
        //private static UnitOfMeasure _current;
        //private readonly Dictionary<UnitType, PixelConverter> _pixelConverters;

        //private UnitOfMeasure()
        //{
        //    _pixelConverters = new Dictionary<UnitType, PixelConverter>()
        //    {
        //        {UnitType.Meter, new PixelToMeterConverter()},
        //        {UnitType.Centimeter, new PixelToCentimeterConverter()},
        //        {UnitType.Millimeter, new PixelToMillimeterConverter()}
        //    };
        //}

        //public UnitType UnitType { get; set; } = UnitType.Millimeter;

        //public PixelConverter Converter => _pixelConverters[UnitType];

        //public static UnitOfMeasure Current
        //{
        //    get
        //    {
        //        lock (LockObject)
        //        {
        //            if(_current == null)
        //                _current = new UnitOfMeasure();

        //            return _current;
        //        }
        //    }
        //}
    }
}
