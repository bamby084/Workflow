using System.Windows;

namespace Designer
{
    public class UnitOfMeasure : DependencyObject
    {
        private static readonly object LockObject = new object();
        private static UnitOfMeasure _current;

        public static readonly DependencyProperty UnitTypeProperty = DependencyProperty.Register(
            "UnitType",
            typeof(UnitType),
            typeof(UnitOfMeasure),
            new FrameworkPropertyMetadata(UnitType.Millimeter));

        private UnitOfMeasure()
        {
        }

        public UnitType UnitType
        {
            get => (UnitType)GetValue(UnitTypeProperty);
            set => SetValue(UnitTypeProperty, value);
        }

        public static UnitOfMeasure Current
        {
            get
            {
                lock (LockObject)
                {
                    if (_current == null)
                        _current = new UnitOfMeasure();

                    return _current;
                }
            }
        }
    }
}
