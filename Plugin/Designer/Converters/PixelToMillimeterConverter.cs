using Designer.ExtensionMethods;

namespace Designer.Converters
{
    public class PixelToMillimeterConverter : PixelConverter
    {
        public override string Unit => UnitType.Millimeter.GetDescription();

        public override decimal Factor => (decimal)(MillimetersPerInch / DotsPerInch);

    }
}
