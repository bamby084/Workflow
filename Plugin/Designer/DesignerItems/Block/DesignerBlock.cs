using System.Windows;

namespace Designer.DesignerItems
{
    public class DesignerBlock: DesignerItem
    {
        private static readonly object LockObject = new object();
        private static int CurrentIndex = 0;
        private ControlPropertiesViewModel _properties;
       
        static DesignerBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignerBlock), new FrameworkPropertyMetadata(typeof(DesignerBlock)));
        }

        public override ControlPropertiesViewModel Properties => _properties;

        public DesignerBlock()
        {
            _properties = new BlockProperties();
            _properties.Name = $"Block {GetNextIndex()}";
            this.DataContext = _properties;
        }

        private int GetNextIndex()
        {
            lock(LockObject)
            {
                CurrentIndex ++;
                return CurrentIndex;
            }
        }
    }
}
