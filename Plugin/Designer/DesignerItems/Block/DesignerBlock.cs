using System.Windows;

namespace Designer.DesignerItems
{
    public class DesignerBlock: DesignerItem
    {
        private ControlPropertiesViewModel _properties;

        static DesignerBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignerBlock), new FrameworkPropertyMetadata(typeof(DesignerBlock)));
        }

        public override ControlPropertiesViewModel Properties => _properties;

        public DesignerBlock()
        {
            _properties = new BlockProperties();
            this.DataContext = _properties;
        }
    }
}
