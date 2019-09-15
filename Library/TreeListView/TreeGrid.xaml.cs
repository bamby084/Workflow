using JdSuite.Common.Module;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace JdSuite.Common.TreeListView
{
    /// <summary>
    /// Interaction logic for TreeGrid.xaml
    /// </summary>
    public partial class TreeGrid : UserControl
    {
        NLog.ILogger logger = NLog.LogManager.GetLogger(nameof(TreeGrid));

        DataItemFactory DataFactory { get; set; }
        public TreeGrid()
        {
            InitializeComponent();
        }

       

        public void LoadData(string xmlDataFile, string schemaFile)
        {
            DataItemFactory df = new DataItemFactory();
            df.LoadData(xmlDataFile, schemaFile);
            LoadData(df);
        }

        

        public void LoadData(DataItemFactory dataFactory)
        {
            this.DataFactory = dataFactory;

            TreeNodeX.RootNode = rootNode;
            TreeNodeX.LoadData(dataFactory.RootDataNode, this.rootNode);

            this.rootNode.textNodeHeader.Text =dataFactory.RootDataNode.Name;
            this.rootNode.grid.ColumnDefinitions[0].MinWidth = this.rootNode.DataItem.ReverseLevel * 30;
            rootNode.Expand(rootNode);
            rootNode.CurrentRecordNo = 1;
            rootNode.ShowNode(rootNode);
            rootNode.ShowRecordNo(rootNode.CurrentRecordNo);
        }

        private void TreeGridCtrl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
           // logger.Info("TreeNodeX.AdjustColumnWidth(rootNode, true);");//start from here

            TreeNodeX.AdjustColumnWidth(rootNode, true);
        }
    }
}
