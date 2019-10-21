using Designer.ExtensionMethods;

namespace Designer.Converters
{
    public class PixelToCentimeterConverter : PixelConverter
    {
        public override string Unit => UnitType.Centimeter.GetDescription();

        public override decimal Factor => (decimal)(MillimetersPerInch / DotsPerInch / 10);
    }
}
