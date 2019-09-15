using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TestHarness
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //StartupUri="pack://application:,,,/Designer;component/MainWindow.xaml"

        [STAThread]
        public static void Main()
        {
            var designer = Designer.MainWindow.Instance;
            designer.ShowDialog();
        }
    }
}
