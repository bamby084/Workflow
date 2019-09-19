using System.Windows.Documents;

namespace Designer.ExtensionMethods
{
    public static class TableExtensions
    {
        public static void AddEmptyCells(this TableRow row, int cellCount)
        {
            for (int i = 0; i < cellCount; i++)
            {
                var cell = new TableCell();
                row.Cells.Add(cell);
            }
        }

        public static void AddColumns(this Table table, int columns)
        {
            for (int i = 0; i < columns; i++)
            {
                var column = new TableColumn();
                table.Columns.Add(column);
            }
        }
    }
}
