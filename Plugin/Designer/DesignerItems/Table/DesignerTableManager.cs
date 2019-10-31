using System;

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

        private DesignerTableManager()
        {
            
        }

        public void NotifyTableAdded(DesignerTable table)
        {
            TableAdded?.Invoke(this, new DesignerTableEventArgs() { Table = table });
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
