using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Designer.DesignerItems
{
    public class BlockDocument: RichTextBox
    {
        public BlockDocument()
        {
            
        }

        public Paragraph GetNearestParagraphFromCurrentCaret(LogicalDirection direction)
        {
            FrameworkContentElement obj = CaretPosition.GetAdjacentElement(direction) as FrameworkContentElement;
            while (obj != null)
            {
                if (obj is Paragraph paragraph)
                    return paragraph;

                obj = obj.Parent as FrameworkContentElement;
            }

            return null;
        }
    }
}
