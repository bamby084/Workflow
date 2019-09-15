using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XML_Converter
{
    public partial class frmNodeName : Form
    {
        private string nodeName;

        public frmNodeName()
        {
            InitializeComponent();
            nodeName = "";
            this.ActiveControl = txtNodeName;
        }
        
        public string NodeName
        {
            get
            {
                return nodeName;
            }
        }

        private void txtNodeName_TextChanged(object sender, EventArgs e)
        {
            btnSubmit.Enabled = txtNodeName.Text.Trim().Length > 0;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            this.nodeName = txtNodeName.Text;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
