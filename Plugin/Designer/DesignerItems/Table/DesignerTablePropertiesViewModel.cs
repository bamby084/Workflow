
namespace Designer.DesignerItems
{
    public class DesignerTablePropertiesViewModel: ControlPropertiesViewModel
    {
        public int Columns { get; set; }
        public int HeaderRows { get; set; }
        public int FooterRows { get; set; }
        public int BodyRows { get; set; }
        public double WidthPercentage { get; set; } = 1.0;
        public double MinWidth { get; set; }
        public DesignerTableAlignment Alignment { get; set; }
    }
}
