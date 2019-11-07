using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;


namespace Designer.DesignerItems
{
    public class BlockDocument: RichTextBox
    {
        public BlockDocument()
        {
            IsUndoEnabled = false;
        }

        public Paragraph GetCaretContainer(LogicalDirection direction)
        {
            FrameworkContentElement obj = CaretPosition.GetAdjacentElement(direction) as FrameworkContentElement;
            while (obj != null)
            {
                if (obj is Paragraph container)
                    return container;

                obj = obj.Parent as FrameworkContentElement;
            }

            return null;
        }
    }
}
