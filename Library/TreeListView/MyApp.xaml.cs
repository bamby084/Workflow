using System;
using System.Windows;
using System.Data;
using System.Xml;
using System.Configuration;


namespace JdSuite.Common.TreeListView
{
    /// <summary>
    /// Interaction logic for MyApp.xaml
    /// </summary>

    public partial class MyApp : Application
    {

        void AppStartup(object sender, StartupEventArgs args)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

    }
}