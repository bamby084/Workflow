﻿
namespace Designer.Converters
{
    public abstract class PixelConverter
    {
        protected const double DotsPerInch = 96d;
        protected const double MillimetersPerInch = 25.4d;

        public abstract string Unit { get; }

        public virtual decimal Factor { get; } = 1;

        public virtual decimal Convert(decimal value)
        {
            return value * Factor;
        }

        public virtual decimal ConvertBack(decimal value)
        {
            return value / Factor;
        }
    }
}
