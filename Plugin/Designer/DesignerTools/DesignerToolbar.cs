using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Designer.DesignerTools
{
    public class DesignerToolbar: Control
    {
        public ObservableCollection<DesignerTool> Tools { get; set; }

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation", typeof(Orientation), typeof(DesignerToolbar), new PropertyMetadata(default(Orientation)));

        public static readonly DependencyProperty SelectedToolProperty = DependencyProperty.Register(
            "SelectedTool", typeof(DesignerTool), typeof(DesignerToolbar), new PropertyMetadata(default(DesignerTool)));

        public Orientation Orientation
        {
            get => (Orientation) GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public DesignerTool SelectedTool
        {
            get { return (DesignerTool) GetValue(SelectedToolProperty); }
            set { SetValue(SelectedToolProperty, value); }
        }

        static DesignerToolbar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignerToolbar),
                new FrameworkPropertyMetadata(typeof(DesignerToolbar)));
        }

        public DesignerToolbar()
        {
            Tools = new ObservableCollection<DesignerTool>();

            Tools.Add(new SelectionTool(new DesignerCanvas()));
            Tools.Add(new DrawingBlockTool(new DesignerCanvas()));
        }
    }
}
