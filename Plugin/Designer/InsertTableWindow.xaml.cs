using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Designer
{
    /// <summary>
    /// Interaction logic for InsertTableWindow.xaml
    /// </summary>
    public partial class InsertTableWindow : Window, INotifyPropertyChanged
    {
        public InsertTableWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private int _columns = 1;
        public int Columns
        {
            get => _columns;
            set
            {
                _columns = value;
                NotifyPropertyChanged();
            }
        }

        private int _headerRows;
        public int HeaderRows
        {
            get => _headerRows;
            set
            {
                _headerRows = value;
                NotifyPropertyChanged();
            }
        }

        private int _bodyRows = 1;
        public int BodyRows
        {
            get => _bodyRows;
            set
            {
                _bodyRows = value;
                NotifyPropertyChanged();
            }
        }

        private int _footerRows;
        public int FooterRows
        {
            get => _footerRows;
            set
            {
                _footerRows = value;
                NotifyPropertyChanged();
            }
        }

        private bool _repeatBody;
        public bool RepeatBody
        {
            get => _repeatBody;
            set
            {
                _repeatBody = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void cbVariable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
