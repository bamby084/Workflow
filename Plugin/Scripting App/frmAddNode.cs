using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScriptingApp
{
    
    public partial class frmAddNode : Form
    {
        private string nodeName;
        private string datatype;

        public frmAddNode()
        {
            InitializeComponent();
            nodeName = "";
            datatype = "";
            this.ActiveControl = txtNodeName;
        }
        public string NodeName
        {
            get
            {
                return nodeName;
            }
        }

        public string DataType
        {
            get
            {
                return datatype;
            }
        }

        private void frmAddNode_Load(object sender, EventArgs e)
        {
            List<string> lists = new List<string>();
            lists.Add("int");
            lists.Add("string");
            lists.Add("long");
            lists.Add("float");
            lists.Add("decimal");
            lists.Add("DateTime");
            lists.Add("double");
            lists.Add("Boolean");
            lists.Add("subtree");
            cmbType.DataSource = lists;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            this.nodeName = txtNodeName.Text;
            this.datatype = cmbType.SelectedValue.ToString();

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtNodeName_TextChanged(object sender, EventArgs e)
        {
            btnSubmit.Enabled = txtNodeName.Text.Trim().Length > 0;
        }
    }
}

    
