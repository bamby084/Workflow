using Designer.ExtensionMethods;

namespace Designer.Converters
{
    public class PixelToInchConverter : PixelConverter
    {
        public override string Unit => UnitType.Inch.GetDescription();

        public override decimal Factor => (decimal)(1 / DotsPerInch);
    }
}
