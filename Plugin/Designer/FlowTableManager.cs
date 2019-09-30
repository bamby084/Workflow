using System;
using System.Collections.Generic;
using System.Linq;

namespace Designer
{
    public delegate void TableEventHandler(object sender, TableEventEventArgs e);
    public class TableEventEventArgs : EventArgs
    {
        public FlowTable Table { get; private set; }

        public TableEventEventArgs(FlowTable table)
        {
            Table = table;
        }
    }

    public class FlowTableManager
    {
        public event TableEventHandler TableAdded;
        public event TableEventHandler TableRemoved;

        private static object _lockObject = new object();
        private static FlowTableManager _flowTableManager;
        private readonly IList<FlowTable> _tables;

        private FlowTableManager()
        {
            _tables = new List<FlowTable>();
        }

        public static FlowTableManager Instance()
        {
            lock (_lockObject)
            {
                if(_flowTableManager == null)
                    _flowTableManager = new FlowTableManager();

                return _flowTableManager;
            }
        }

        public void Add(FlowTable table)
        {
            _tables.Add(table);
            TableAdded?.Invoke(this, new TableEventEventArgs(table));
        }

        public FlowTable GetById(Guid id)
        {
            return _tables.FirstOrDefault(t => t.Id == id);
        }

        public void Remove(Guid id)
        {
            var table = GetById(id);
            if (table == null)
                return;

            _tables.Remove(table);
            TableRemoved?.Invoke(this, new TableEventEventArgs(table));
        }
    }
}
