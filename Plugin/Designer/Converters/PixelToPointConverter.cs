using Designer.ExtensionMethods;
using System;

namespace Designer.Converters
{
    public class PixelToPointConverter : PixelConverter
    {
        public override string Unit => UnitType.Point.GetDescription();

        public override decimal Factor => 0.75m;
    }
}
