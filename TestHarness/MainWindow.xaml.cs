using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using Designer;
using Designer.Adorners;
using Designer.DesignerTools;


namespace TestHarness
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private FlowTablePresenter _table;
        public MainWindow()
        {
            InitializeComponent();
            UnitOfMeasure.Current.UnitType = UnitType.Centimeter;
            this.DataContext = this;

            Tools = new ObservableCollection<DesignerTool>();
            Tools.Add(new SelectionTool());
            Tools.Add(new DrawingBlockTool());

            this.Loaded += MainWindow_Loaded;
            Canvas.SelectedItemsChanged += SelectedItemsChanged;
        }

        private void SelectedItemsChanged(object sender, SelectedItemsChangedEventArgs e)
        {
            if (e.SelectedItems.Count == 0)
                return;

            SelectedControlProperties = e.SelectedItems[0].Properties;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Tools[0].IsSelected = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<DesignerTool> Tools { get; set; }

        private ControlPropertiesViewModel _selectedControlProperties;
        public ControlPropertiesViewModel SelectedControlProperties {
            get => _selectedControlProperties;
            set
            {
                _selectedControlProperties = value;
                NotifyPropertyChanged();
            }
        }

        private UnitType _selectedUnit;
        public UnitType SelectedUnit
        {
            get => _selectedUnit;
            set
            {
                _selectedUnit = value;
                NotifyPropertyChanged();
                UnitOfMeasure.Current.UnitType = value;
            }
        }
    }
}
