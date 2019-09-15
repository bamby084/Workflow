using JdSuite.Common.Logging;
using JdSuite.Common.Logging.Enums;
using JdSuite.Common.Module;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Designer
{
    public class RowData : INotifyPropertyChanged, IXmlSerializable
    {
        private string _ConstVal = "";
        private int _Index = 0;
        private ComboBoxItem _SelectedVariable;
        private ObservableRangeCollection<ComboBoxItem> _Variables = new ObservableRangeCollection<ComboBoxItem>();

        public string ConstVal
        {
            get => _ConstVal;
            set
            {
                _ConstVal = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ConstVal"));
            }
        }

        public int Index
        {
            get => _Index;
            set
            {
                _Index = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Index"));
            }
        }

        public ComboBoxItem SelectedVariable
        {
            get => _SelectedVariable;
            set
            {
                _SelectedVariable = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedVariable"));
            }
        }

        public ObservableRangeCollection<ComboBoxItem> Variables
        {
            get => _Variables;
            set
            {
                _Variables = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Variables"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private string XmlSelectedVariable = null;


        /// <summary>
        /// XML initialization only.
        /// </summary>
        public RowData() { }

        public RowData(int index, ObservableRangeCollection<ComboBoxItem> variables)
        {
            Index = index;
            Variables = variables;
            SelectedVariable = variables[0];
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();

            if (!int.TryParse(reader.GetAttribute("Index"), out _Index))
            {
                Logger.Log(Severity.ERROR, LogCategory.CONTROL, "XML deserialization for sheet name row index failed");
            }
            else
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Index"));
            }

            XmlSelectedVariable = reader.GetAttribute("SelectedVariable");
            if (XmlSelectedVariable.Contains(SheetNameDataGrid.CONTROL_CHAR_XML))
            {
                XmlSelectedVariable = XmlConvert.DecodeName(XmlSelectedVariable);
            }
            ConstVal = reader.GetAttribute("ConstantValue");

            reader.Skip();

            PropertyChanged += RowDataXml_PropertyChanged;
        }

        private void RowDataXml_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Variables")
            {
                return;
            }

            foreach (var cbi in _Variables)
            {
                if (cbi.Content as string != XmlSelectedVariable)
                {
                    continue;
                }

                XmlSelectedVariable = null;
                SelectedVariable = cbi;
                PropertyChanged -= RowDataXml_PropertyChanged;
                break;
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Index", _Index.ToString());
            string selVar = SelectedVariable.Content as string;
            if (selVar.Contains(SheetNameDataGrid.CONTROL_CHAR))
            {
                selVar = XmlConvert.EncodeName(selVar);
            }
            writer.WriteAttributeString("SelectedVariable", selVar);
            writer.WriteAttributeString("ConstantValue", _ConstVal);
        }
    }

    /// <summary>
    /// Interaction logic for SheetNameDataGrid.xaml
    /// </summary>
    public partial class SheetNameDataGrid : UserControl, INotifyPropertyChanged, IXmlSerializable
    {
        public static readonly string CONTROL_CHAR = '\u0005'.ToString();
        public static readonly string CONTROL_CHAR_XML = XmlConvert.EncodeName(CONTROL_CHAR);
        public static readonly string CONTROL_VARIABLE_EMPTY = CONTROL_CHAR + "Empty";
        private ObservableCollection<RowData> _GridDataset = new ObservableCollection<RowData>();
        private ObservableRangeCollection<ComboBoxItem> Variables = new ObservableRangeCollection<ComboBoxItem>();

        public ObservableCollection<RowData> GridDataset
        {
            get => _GridDataset;
            set
            {
                _GridDataset = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GridDataset"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public SheetNameDataGrid()
        {
            InitializeComponent();

            Field schema = MainWindow.Instance.Schema;

            {
                var cbi = new ComboBoxItem();
                cbi.Content = CONTROL_VARIABLE_EMPTY;
                Variables.Add(cbi);
            }

                string name = schema.Name;
                string type = schema.Type;
                //if (name == null || type == null || schema.Count > 0) { continue; }

                var item = new ComboBoxItem();
                item.Content = name;
                Variables.Add(item);

            ButtonAdd_Click(null, null);
        }

        /// <summary>
        /// Gets the name of a sheet based on index.
        /// </summary>
        /// <param name="index">The desired index.</param>
        /// <returns>Either the selected variable name, the constant value, or an empty string if not found.</returns>
        public string GetSheetName(int index)
        {
            foreach (var row in GridDataset)
            {
                if (row.Index == index)
                {
                    var selStr = row.SelectedVariable.Content as string;
                    if (selStr != CONTROL_VARIABLE_EMPTY)
                    {
                        return selStr;
                    }
                    else
                    {
                        return row.ConstVal;
                    }
                }
            }
            return "";
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();

            reader.ReadStartElement();
            var serializer = new XmlSerializer(typeof(ObservableCollection<RowData>));
            GridDataset = serializer.Deserialize(reader) as ObservableCollection<RowData>;
            reader.ReadEndElement();

            foreach (var data in _GridDataset)
            {
                data.Variables = Variables;
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            var serializer = new XmlSerializer(typeof(ObservableCollection<RowData>));
            serializer.Serialize(writer, _GridDataset);
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            GridDataset.Add(new RowData(GridDataset.Count, Variables));
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            if (GridDataset.Count <= 0)
            {
                return;
            }

            if (DataGridSheet.SelectedItem is RowData data)
            {
                GridDataset.Remove(data);
            }
            else
            {
                GridDataset.Remove(GridDataset[GridDataset.Count - 1]);
            }
        }
    }
}