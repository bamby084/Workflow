using System;
using System.Linq;
using System.Collections.ObjectModel;
using Designer.ExtensionMethods;

namespace Designer.DesignerItems
{
    public class DesignerTableEventArgs : EventArgs
    {
        public TableProperties Table { get; set; }
    }

    public delegate void DesignerTableEventHandler(object sender, DesignerTableEventArgs e);

    public class DesignerTableManager
    {
        public event DesignerTableEventHandler TableAdded;
        public event DesignerTableEventHandler TableRemoved;

        private static object _lockObject = new object();
        private static DesignerTableManager _designerTableManager;

        public ObservableCollection<TableProperties> Tables { get; set; }

        private DesignerTableManager()
        {
            Tables = new ObservableCollection<TableProperties>();
        }

        public void AddTable(TableProperties table, bool fireEvent = true)
        {
            table.OnDeleted += OnTableDeleted;
            Tables.Add(table);
            
            if (TableAdded != null && fireEvent)
                TableAdded(this, new DesignerTableEventArgs() { Table = table });
        }

        private void OnTableDeleted(object sender, EventArgs e)
        {
            RemoveTable((TableProperties)sender);
        }

        public void RemoveTable(TableProperties table)
        {
            Tables.Remove(table);
            NotifyTableRemove(table);
        }

        public void NotifyTableRemove(TableProperties table)
        {
            TableRemoved?.Invoke(this, new DesignerTableEventArgs() { Table = table });
        }

        public bool TableExists(string tableName)
        {
            return Tables.Any(t => t.Name.EqualsIgnoreCase(tableName));
        }
        
        public static DesignerTableManager Instance
        {
            get
            {
                lock (_lockObject)
                {
                    if (_designerTableManager == null)
                        _designerTableManager = new DesignerTableManager();

                    return _designerTableManager;
                }
            }
        }
    }
}
