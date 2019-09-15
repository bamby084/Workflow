using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace JdSuite.DataSorting
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            XMLSorter sorter = new XMLSorter();
            sorter.DataFile = @"E:\Canvas\Dropbox\BlockApp\Data\2_Customer_data_XML.xml";
            sorter.LoadData();
            sorter.Sort();

        }
    }
}
