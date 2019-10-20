
namespace Designer.Converters
{
    public class PixelToMillimeterConverter : PixelConverter
    {
        public override string Unit => "mm";

        public override double Factor => MillimetersPerInch / DotsPerInch;

    }
}
