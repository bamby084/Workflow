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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private FlowTablePresenter _table;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        private void SelectionToolClicked(object sender, RoutedEventArgs e)
        {
            Canvas.ActiveTool = new SelectionTool(Canvas);
        }

        private void BlockToolClicked(object sender, RoutedEventArgs e)
        {
            Canvas.ActiveTool = new DrawingBlockTool(Canvas);
        }
    }
}
