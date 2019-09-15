using JdSuite.Common.Module;
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
using System.Xml.Linq;

namespace DataOutput.Controls
{
    /// <summary>
    /// Interaction logic for XMLOutputControl.xaml
    /// </summary>
    public partial class XMLOutputControl : UserControl
    {
        private readonly RoutedUICommand changedIndex;

        NLog.ILogger logger = NLog.LogManager.GetLogger(nameof(XMLOutputControl));

        public Field Schema { set { DataView.Schema = value; } }
        public XDocument Data
        {
            set
            {
                DataView.Data = value;

            }
        }


        public XMLOutputControl()
        {
            InitializeComponent();

            // Binding with the ChangedIndexCommand on GridPaging.........................................
            // Create de Command.
            this.changedIndex = new RoutedUICommand("ChangedIndex", "ChangedIndex", typeof(XMLOutputControl));

            // Assing the command to GridPaging Command.
            pagingControl.ChangedIndexCommand = this.changedIndex;

            // Binding Command
            CommandBinding abinding = new CommandBinding { Command = this.changedIndex };

            // Binding Handler to executed.
            abinding.Executed += this.OnChangeIndexCommandHandler;
            this.CommandBindings.Add(abinding);
        }

        /// <summary>
        /// Get the change index event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnChangeIndexCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var pageIndex = pagingControl.PageIndex;
                var pageSize = pagingControl.PageSize;
                DataView.PageSize = pageSize;
                DataView.RecordIndex = pageSize * (pageIndex-1);
                
                DataView.RefreshData();
                // pagingControl.TotalCount = 100;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                MessageService.ShowError("Page data loading error", ex.Message);
            }
        }

        public void RefreshData()
        {
            try
            {
              
                this.pagingControl.TotalCount = DataView.RecordCount;
                this.pagingControl.PageSize = DataView.PageSize;
                this.pagingControl.IsControlVisible = true;
                this.pagingControl.HasNextPage = true;

                logger.Info($"Total Records: {this.pagingControl.TotalCount}");

                DataView.RefreshData();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                MessageService.ShowError("Data Loading Error", ex.Message);
            }
        }
    }
}
