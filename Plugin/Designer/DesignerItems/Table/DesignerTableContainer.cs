using Designer.Converters;
using System;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using FigureLengthConverter = Designer.Converters.FigureLengthConverter;

namespace Designer.DesignerItems
{
    public class DesignerTableContainer: Figure
    {
        public DesignerTableContainer()
        {

        }

        public DesignerTableContainer(Block childBlock)
            :base(childBlock)
        {
            Init();
        }

        public DesignerTableContainer(Block childBlock, TextPointer insertionPosition)
            : base(childBlock, insertionPosition)
        {
            Init();
        }

        private void Init()
        {
            Margin = new Thickness(0);
            BorderThickness = new Thickness(1);
            SetBindingProperties();
        }

        private void SetBindingProperties()
        {
            var alignmentBinding = new Binding("Properties.Alignment");
            alignmentBinding.Source = this.Blocks.FirstBlock;
            alignmentBinding.Converter = new FigureHorizontalAnchorConverter();
            SetBinding(HorizontalAnchorProperty, alignmentBinding);

            var widthBinding = new Binding("Properties.WidthPercentage");
            widthBinding.Converter = new FigureLengthConverter();
            widthBinding.ConverterParameter = FigureUnitType.Page;
            widthBinding.Source = this.Blocks.FirstBlock;
            SetBinding(WidthProperty, widthBinding);

            var paddingBinding = new MultiBinding();
            paddingBinding.Converter = new ThicknessBindingConverter();
            paddingBinding.Bindings.Add(new Binding("Properties.SpaceLeft") { Source = this.Blocks.FirstBlock });
            paddingBinding.Bindings.Add(new Binding("Properties.SpaceTop") { Source = this.Blocks.FirstBlock });
            paddingBinding.Bindings.Add(new Binding("Properties.SpaceRight") { Source = this.Blocks.FirstBlock });
            paddingBinding.Bindings.Add(new Binding("Properties.SpaceBottom") { Source = this.Blocks.FirstBlock });
            SetBinding(PaddingProperty, paddingBinding);

            var borderBrushBinding = new Binding("Properties.IsSelected");
            borderBrushBinding.Source = this.Blocks.FirstBlock;
            borderBrushBinding.Converter = new BoolToBorderBrushConverter();
            SetBinding(BorderBrushProperty, borderBrushBinding);
        }

        private class BoolToBorderBrushConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                bool val = System.Convert.ToBoolean(value);
                if (val)
                    return new SolidColorBrush(Colors.Orange);

                return new SolidColorBrush(Colors.Purple);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }

    
}
