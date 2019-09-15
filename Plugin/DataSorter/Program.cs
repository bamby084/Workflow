using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdSuite.DataSorting
{
    class Program
    {
        //[STAThread]
        static void Main2(string[] args)
        {
            //Application a = new Application();
            //a.StartupUri = new Uri("MainWindow.xaml", System.UriKind.Relative);
            //a.Run();

            XMLSorter sorter = new XMLSorter();
            sorter.DataFile = @"E:\Canvas\Dropbox\BlockApp\Data\2_Customer_data_XML.xml";
            sorter.OutputFileName = @"E:\Canvas\Dropbox\BlockApp\Data\multi_level_zip_order8.xml";

            //sorter.DataFile = @"E:\Canvas\Dropbox\BlockApp\Data\two_level_xml.xml";
            sorter.LoadData();
            sorter.Sort();
            sorter.Save();
        }
    }
}
