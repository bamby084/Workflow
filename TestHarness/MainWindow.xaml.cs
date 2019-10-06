using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using Designer;
using Designer.Adorners;


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

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(Button1);
            if (adornerLayer != null)
            {
                adornerLayer.Add(new DesignerItemAdorner(Button1));
            }
        }
    }
}
