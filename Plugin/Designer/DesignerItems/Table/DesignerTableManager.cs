using System;
using System.Linq;
using System.Collections.ObjectModel;
using Designer.ExtensionMethods;
using System.Xml;
using System.Reflection;
using System.Collections;

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

        /// <summary>
        /// Serializing tables to a xml writer to write to *.flo file
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Tables"); //<Tables Count="xx">
            writer.WriteAttributeString("Count", Tables.Count.ToString());

            foreach (var table in Tables)
            {
                WriteXml(writer, table);
            }

            writer.WriteEndElement(); //</Tables>
        }

        private void WriteXml(XmlWriter writer, object obj)
        {
            var xmlOut = obj.GetType().GetCustomAttribute<XmlOutAttribute>();
            string elementName = obj.GetType().ToString();
            if (xmlOut != null)
                elementName = xmlOut.Name;

            writer.WriteStartElement(elementName);

            var outProperties = obj.GetType().GetProperties().Where(prop => prop.GetCustomAttribute<XmlOutAttribute>() != null);
            var primitiveProperties = outProperties.Where(prop => !prop.PropertyType.IsEnumrable()).OrderBy(prop => prop.Name);
            var collectionProperties = outProperties.Where(prop => prop.PropertyType.IsEnumrable()).OrderBy(prop => prop.Name);

            foreach(var property in primitiveProperties)
            {
                string value = property.GetValue(obj)?.ToString();
                var outAttribute = property.GetCustomAttribute<XmlOutAttribute>();
                string propertyName = outAttribute.Name ?? property.Name;

                writer.WriteAttributeString(propertyName, value);
            }

            foreach(var property in collectionProperties)
            {
                string collectionName = property.GetCustomAttribute<XmlOutAttribute>().Name;
                if (!string.IsNullOrEmpty(collectionName))
                    writer.WriteStartElement(collectionName);

                var value = (IEnumerable)property.GetValue(obj);
                foreach (var child in value)
                {
                    WriteXml(writer, child);
                }

                if (!string.IsNullOrEmpty(collectionName))
                    writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
    }
}
