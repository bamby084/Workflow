using Designer.ExtensionMethods;

namespace Designer.Converters
{
    public class PixelToMeterConverter : PixelConverter
    {
        public override string Unit => UnitType.Meter.GetDescription();

        public override double Factor => MillimetersPerInch / DotsPerInch / 1000;
    }
}
