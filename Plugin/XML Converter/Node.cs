using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XML_Converter
{
    public class Node
    {
        private string name;
        private List<Node> children;

        public Node()
        {
            children = new List<Node>();
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public List<Node> Children
        {
            get
            {
                return children;
            }
            set
            {
                this.children = value;
            }
        }

        public string Type
        {
            get
            {
                DataGridViewComboBoxCell cell = new DataGridViewComboBoxCell();
                return "hi";
                //cell.
                //return new DataGridViewComboBoxCell()
            }
        }
    }
}
