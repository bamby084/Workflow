using System;
using System.Collections.ObjectModel;

namespace Designer.DesignerItems
{
    public class DesignerTableEventArgs : EventArgs
    {
        public DesignerTable Table { get; set; }
    }

    public delegate void DesignerTableEventHandler(object sender, DesignerTableEventArgs e);

    public class DesignerTableManager
    {
        public event DesignerTableEventHandler TableAdded;
        public event DesignerTableEventHandler TableRemoved;

        private static object _lockObject = new object();
        private static DesignerTableManager _designerTableManager;

        public ObservableCollection<DesignerTable> Tables { get; set; }

        private DesignerTableManager()
        {
            Tables = new ObservableCollection<DesignerTable>();
        }

        public void AddTable(DesignerTable table, bool fireEvent = true)
        {
            Tables.Add(table);

            if (TableAdded != null && fireEvent)
                TableAdded(this, new DesignerTableEventArgs() { Table = table });
        }

        public void NotifyTableRemove(DesignerTable table)
        {
            TableRemoved?.Invoke(this, new DesignerTableEventArgs() { Table = table });
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
