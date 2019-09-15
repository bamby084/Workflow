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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Designer
{
    /// <summary>
    /// Interaction logic for PPP_NewSelector.xaml
    /// </summary>
    public partial class PPP_NewSelector : Window 
    {
		public bool Successful = false;

        public PPP_NewSelector()
        {
            InitializeComponent();
        }

		private void ButtonOK_Click(object sender, RoutedEventArgs e)
		{
			if (Field.Text.Length <= 0) {
				ButtonCancel_Click(null, null);
				return;
			}
			Successful = true;
			Close();
		}

		private void ButtonCancel_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);

			if (e.Key == Key.Return) {
				ButtonOK_Click(null, null);
			}
			else if (e.Key == Key.Escape) {
				ButtonCancel_Click(null, null);
			}
		}
	}
}
