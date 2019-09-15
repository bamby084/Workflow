using AdvancedDataGridView;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using JdSuite.Common.Module;

namespace XML_Converter
{
    public partial class frmMain : Form
    {
        public bool isCancel;
        private Dictionary<string, string> encoding;
        public ModuleState State { get; set; }

        public frmMain(ModuleState state)
        {
            this.State = state;
            InitializeComponent();
            isCancel = false;
            encoding = new Dictionary<string, string>();
            encoding.Add("Unicode", "utf-16");
            encoding.Add("Unicode (Big endian)", "unicodeFFFE");
            encoding.Add("Western European (Windows)", "Windows-1252");
            encoding.Add("Unicode (UTF-32)", "utf-32");
            encoding.Add("Unicode (UTF-32 Big endian)", "utf-32BE");
            encoding.Add("US-ASCII", "us-ascii");
            encoding.Add("Western European (ISO)", "iso-8859-1");
            encoding.Add("Unicode (UTF-7)", "utf-7");
            encoding.Add("Unicode (UTF-8)", "utf-8");

            foreach (String field in encoding.Keys)
            {
                txtTextEncoding.Items.Add(field);
            }

            if (this.State.InputPath.Count() > 0)
            {
                SetInputFilePath(this.State.InputPath);
            }

            if (this.State.Schema != null)
            {
                SetTreeFromSchema(this.State.Schema);
            }
        }

        private void SetInputFilePath(string path)
        {
            txtInputFile.Text = path;
            this.State.InputPath = path;
        }

        private void OpeningChildren(XmlNodeList parent, TreeGridNode parentN)
        {
            try
            {
                int count = 0;
                foreach (XmlNode child in parent)
                {
                    if (child.Name == "xml" || child.Name == "Root")
                        OpeningChildren(child.ChildNodes, parentN);
                    else
                    {
                        TreeGridNode node = parentN.Nodes.Add(child.Name);
                        if (child.Name != "Root")
                        {
                            parentN.Nodes[count].Cells[1].Value = child.Attributes[0].Value;
                            parentN.Nodes[count].Cells[2].Value = child.Attributes[1].Value;
                            parentN.Nodes[count].Cells[3].Value = child.Attributes[2].Value;
                            parentN.Nodes[count].Cells[4].Value = child.Attributes[3].Value;
                            parentN.Nodes[count].Cells[5].Value = child.Attributes[4].Value;
                            count++;
                        }

                        if (child.HasChildNodes)
                            OpeningChildren(child.ChildNodes, parentN.Nodes[parentN.Nodes.IndexOf(node)]);
                    }
                }
            }
            catch (Exception e)
            {
                SetInputFilePath("");
                this.State.Schema = null;
                MessageBox.Show("Could not load previous session!", "XML Converter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                gridNodes.Nodes.Clear();
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            if (this.State.TextEncoding.Count() > 0)
            {
                for (int i = 0; i < txtTextEncoding.Items.Count; i++)
                {
                    string text = txtTextEncoding.Items[i] as string;
                    if (text == this.State.TextEncoding)
                    {
                        txtTextEncoding.SelectedIndex = i;
                        break;
                    }
                }
            }
            else
            {
                txtTextEncoding.SelectedIndex = 0;
            }

            if (this.State.Schema != null)
            {
                ExpandTree();
            }
        }

        private void btnInputBrowse_Click(object sender, EventArgs e)
        {
            DialogResult result = inputFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (System.IO.Path.GetExtension(inputFileDialog.FileName).ToLower() == ".xml")
                {
                    this.State.InputIsSchema = false;
                    SetInputFilePath(inputFileDialog.FileName);
                    SetTreeFromInputFile();
                }
                else
                {
                    MessageBox.Show("Selected file is not an XML file", "XML Converter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SetTreeFromInputFile()
        {
            gridNodes.Nodes.Clear();

            TreeGridNode node = gridNodes.Nodes.Add("Root");
            node.Cells[1].ReadOnly = true;
            node.Cells[2].ReadOnly = true;
            node.Cells[3].ReadOnly = true;
            node.Cells[4].ReadOnly = true;
            node.Cells[5].ReadOnly = true;

            XmlDocument doc = new XmlDocument();
            doc.Load(txtInputFile.Text);
            GetChildren(doc.ChildNodes, node);
        }

        private void SetTreeFromSchema(Field schema)
        {
            gridNodes.Nodes.Clear();
            TreeGridNode node = gridNodes.Nodes.Add("Root");
            node.Cells[1].ReadOnly = true;
            node.Cells[2].ReadOnly = true;
            node.Cells[3].ReadOnly = true;
            node.Cells[4].ReadOnly = true;
            node.Cells[5].ReadOnly = true;
            SchemaChildren(schema, node);
        }

        private void GetChildren(XmlNodeList parent, TreeGridNode parentN)
        {
            int count = 0;
            foreach (XmlNode child in parent)
            {
                bool contains = false;
                foreach (TreeGridNode node in parentN.Nodes)
                {
                    if (node.Cells[0].Value.ToString() == child.Name)
                    {
                        contains = true;
                        break;
                    }
                }

                if (!contains)
                {
                    if (child.Name != "#text" && child.Name != "xml")
                    {
                        TreeGridNode node = parentN.Nodes.Add(child.Name);
                        parentN.Nodes[count].Cells[1].Value = "Element";
                        parentN.Nodes[count].Cells[2].Value = "One";
                        parentN.Nodes[count].Cells[3].Value = "Flatten";
                        parentN.Nodes[count].Cells[4].Value = child.Name;
                        parentN.Nodes[count].Cells[5].Value = "String";
                        count++;

                        if (child.HasChildNodes)
                            GetChildren(child.ChildNodes, parentN.Nodes[parentN.Nodes.IndexOf(node)]);
                    }
                }
            }
        }

        private Field GenerateSchema()
        {
            Field _schema = null;
            foreach (TreeGridNode node in gridNodes.Nodes[0].Nodes)
            {
                CreateDefinition(node, ref _schema);
            }
            return _schema;
        }

        private void CreateDefinition(TreeGridNode node, ref Field element)
        {
            Field el = new Field();
            el.Name = node.Cells[0].Value.ToString();
            el.Type = node.Cells[1].Value.ToString();
            el.DataType = node.Cells[5].Value.ToString();
            el.Use = node.Cells[2].Value.ToString();
            el.Change = node.Cells[3].Value.ToString();
            el.Alias = node.Cells[4].Value.ToString();
            if (element == null)
                element = el;
            else
                element.ChildNodes.Add(el);
            foreach (TreeGridNode subNode in node.Nodes)
            {
                CreateDefinition(subNode, ref el);
            }
        }

        private void ExportFile(string filename)
        {
            this.State.Schema = GenerateSchema();
            this.State.Schema.Save(filename);
            MessageBox.Show("Schema file saved successfully!", "XML Converter", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnExportXML_Click(object sender, EventArgs e)
        {
            DialogResult result = saveFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                ExportFile(saveFileDialog.FileName);
            }
        }

        private void ExpandChildren(TreeGridNode parent)
        {
            parent.Expand();
            foreach (TreeGridNode node in parent.Nodes)
            {
                ExpandChildren(node);
            }
        }

        private void btnGenReadXML_Click(object sender, EventArgs e)
        {
            ExpandTree();
        }

        private void ExpandTree()
        {
            ExpandChildren(gridNodes.Nodes[0]);
        }

        private void ClosingChildren(TreeGridNode node, XElement element)
        {
            try
            {
                XElement el = new XElement(node.Cells[0].Value.ToString(), new XAttribute("type", node.Cells[1].Value.ToString()), new XAttribute("optionality", node.Cells[2].Value.ToString()), new XAttribute("change", node.Cells[3].Value.ToString()), new XAttribute("XMLName", node.Cells[4].Value.ToString()), new XAttribute("DataType", node.Cells[5].Value.ToString()));
                element.Add(el);
                foreach (TreeGridNode subNode in node.Nodes)
                {
                    ClosingChildren(subNode, el);
                }
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(ex.Message + " ;; " + element.Name);
            }
        }

        public void SetTextEncoding(string encoding)
        {

        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isCancel)
            {
                if (gridNodes.Nodes.Count <= 0)
                {
                    return;
                }
                this.State.Schema = GenerateSchema();
                this.State.TextEncoding = txtTextEncoding.Text;
            }
        }

        private void gridNodes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
                                      Color.Black, 0, ButtonBorderStyle.Inset,
                                      Color.Black, 1, ButtonBorderStyle.Solid,
                                      Color.Black, 0, ButtonBorderStyle.Inset,
                                      Color.Black, 0, ButtonBorderStyle.Inset);
        }

        private void AddChildren(TreeGridNode parent, TreeGridNode child)
        {
            //TreeGridNode node = parent.Nodes.Add("Hello");
            TreeGridNode node = parent.Nodes.Add(child.Cells[0].Value.ToString());
            node.Cells[1].Value = child.Cells[1].Value;
            node.Cells[2].Value = child.Cells[2].Value;
            node.Cells[3].Value = child.Cells[3].Value;
            node.Cells[4].Value = child.Cells[4].Value;
            node.Cells[5].Value = child.Cells[5].Value;
            foreach (TreeGridNode sub in child.Nodes)
            {
                AddChildren(node, sub);
            }
        }

        private void MoveNode(TreeGridNode node, TreeGridNode prevNode)
        {
            List<TreeGridNode> allNode = new List<TreeGridNode>();
            foreach (TreeGridNode sub in node.Parent.Nodes)
            {
                allNode.Add(sub);
            }
            List<TreeGridNode> removed = new List<TreeGridNode>();
            try
            {
                bool startDelete = false;
                foreach (TreeGridNode sub in allNode)
                {
                    if (sub == prevNode)
                        startDelete = true;

                    if (sub == node)
                        startDelete = false;

                    if (startDelete)
                    {
                        if (sub != prevNode)
                            removed.Add(sub);
                        node.Parent.Nodes.Remove(sub);
                    }
                }

                foreach (TreeGridNode sub in removed)
                {
                    node.Parent.Nodes.Add(sub);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void addNewNode(bool isParent)
        {
            frmNodeName nameForm = new frmNodeName();
            if (nameForm.ShowDialog() == DialogResult.OK)
            {
                TreeGridNode node;
                if (isParent)
                {
                    node = gridNodes.CurrentNode.Parent.Nodes.Add(nameForm.NodeName);
                    AddChildren(node, gridNodes.CurrentNode);
                    MoveNode(node, gridNodes.CurrentNode);
                }
                else
                    node = gridNodes.CurrentNode.Nodes.Add(nameForm.NodeName);

                node.Cells[1].Value = "Element";
                node.Cells[2].Value = "One";
                node.Cells[3].Value = "Flatten";
                node.Cells[4].Value = nameForm.NodeName;
                node.Cells[5].Value = "String";
                ExpandTree();
            }
        }

        private void addChildMenuItem_Click(object sender, EventArgs e)
        {
            addNewNode(false);
        }

        private void addParentMenuItem_Click(object sender, EventArgs e)
        {
            if (gridNodes.CurrentNode.Parent.Cells[0].Value == null)
                MessageBox.Show("Cannot add parent to Root", "XML Converter", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                addNewNode(true);
        }

        private void removeMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove this node and all its children?", "XML Converter", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
            {
                gridNodes.CurrentNode.Parent.Nodes.Remove(gridNodes.CurrentNode);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            isCancel = true;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public string FirstLetterToUpper(string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }

        private void SchemaChildren(Field parent, TreeGridNode parentN)
        {
            int count = 0;
            string name = parent.Name;
            TreeGridNode node = parentN.Nodes.Add(name);
            node.Cells[0].Value = FirstLetterToUpper(parent.Name);
            node.Cells[1].Value = parent.Type;
            node.Cells[2].Value = parent.Use;
            node.Cells[3].Value = parent.Change;
            node.Cells[4].Value = parent.Alias;
            node.Cells[5].Value = FirstLetterToUpper(parent.DataType);
            count++;

            if (parent.ChildNodes.Count > 0)
            {
                foreach (var item in parent.ChildNodes)
                {
                    SchemaChildren(item, parentN.Nodes[parentN.Nodes.IndexOf(node)]);
                }
            }
        }

        private void btnReadXML_Click(object sender, EventArgs e)
        {
            DialogResult result = inputFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (System.IO.Path.GetExtension(inputFileDialog.FileName).ToLower() == ".xml")
                {
                    SetInputFilePath(inputFileDialog.FileName);
                    Field field = Field.Parse(inputFileDialog.FileName);
                    SetTreeFromSchema(field);
                    this.State.InputIsSchema = true;
                    ExpandTree();
                }
                else
                {
                    MessageBox.Show("Selected file is not an XML file", "XML Converter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }
    }
}