
using System.ComponentModel;

namespace Designer
{
    public enum UnitType
    {
        [Description("m")]
        Meter,

        [Description("cm")]
        Centimeter,

        [Description("mm")]
        Millimeter,

        [Description("″")]
        Inch,

        [Description("pt")]
        Point,

        [Description("px")]
        Pixel
    }
}
