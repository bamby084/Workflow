using System;
using System.Collections.Generic;
using System.Linq;
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

namespace JdSuite.Common.TreeListView
{
    /// <summary>
    /// This class is used in testing only, it is not intended to be used in any production code
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string f1 = @"E:\Canvas2\Dropbox\BlockApp\Data\Customer_data_XML_1.xml";//For testing only
            string f2 = @"E:\Canvas2\Dropbox\BlockApp\Data\PassedSchema.xml";//
            treeGrid.LoadData(f1, f2);
            treeGrid.rootNode.ShowRecordNo(1);
        }
    }
}
