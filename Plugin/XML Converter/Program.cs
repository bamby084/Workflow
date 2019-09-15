using JdSuite.Common.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Xml.Linq;

[assembly: DisableDpiAwareness]
namespace XML_Converter
{
    public static class Program
    {
		public static frmMain form;

		static Program() {}

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(ModuleState state)
        {
            Application.EnableVisualStyles();
			form = new frmMain(state);
			form.ShowDialog();
        }
    }
}
