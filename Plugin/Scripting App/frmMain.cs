using AdvancedDataGridView;
using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ScriptingApp
{
    public partial class frmMain : Form
    {
        #region Data Members Global
        List<DynamicClassProperties> DCObject = new List<DynamicClassProperties>();
        List<string> InitializeObjects = new List<string>();
        XElement exportRoot;
        XNamespace exportNs;
        bool HasTopRowAdded = false;
        #endregion

        #region Constructor
        public frmMain()
        {
            InitializeComponent();
        }
        #endregion

        #region XMLRead and AddNodes Function
        private void ReadXML(TreeGridView grid, string Type)
        {
            try
            {
                string TopNode = Type == "Input" ? "Input_0" : Type == "Output" ? "Output_0" : "";
                string filepath = ConfigurationManager.AppSettings["ReadFilePath"];
                // SECTION 1. Create a DOM Document and load the XML data into it.
                XmlDocument dom = new XmlDocument();
                dom.Load(filepath);
                // SECTION 2. Initialize the TreeView control.
                grid.Nodes.Clear();
                grid.Nodes.Add("Root");
                TreeGridNode tNode = new TreeGridNode();
                tNode = grid.Nodes[0];

                // SECTION 3. Populate the TreeView with the DOM nodes.
                AddNode(dom.DocumentElement, tNode, grid, TopNode);
            }
            catch (XmlException xmlEx)
            {
                MessageBox.Show(xmlEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddNode(XmlNode inXmlNode, TreeGridNode inTreeNode, TreeGridView grid, string Type)
        {
            XmlNode xNode;
            TreeGridNode tNode;
            XmlNodeList nodeList;
            int i;

            // Loop through the XML nodes until the leaf is reached.
            // Add the nodes to the TreeView during the looping process.
            if (inXmlNode.HasChildNodes)
            {
                nodeList = inXmlNode.ChildNodes;
                for (i = 0; i <= nodeList.Count - 1; i++)
                {
                    if (nodeList[i].Name != "xs:schema" && nodeList[i].Name != "XML_Converter" && nodeList[i].Name != "Root" && nodeList[i].Name != "Schema")
                    {

                        xNode = inXmlNode.ChildNodes[i];
                        inTreeNode.Nodes.Add(xNode.Attributes[0].Value);
                        inTreeNode.Nodes[i].Cells[0].Value = xNode.Attributes[0].Value;
                        inTreeNode.Nodes[i].Cells[1].Value = xNode.Attributes[1].Value.Replace("xs:", "");//FirstLetterToUpper(xNode.Attributes[1].Value.Replace("xs:", ""));
                        tNode = inTreeNode.Nodes[i];
                        AddNode(xNode, tNode, grid, Type);
                    }
                    else
                    {
                        if (nodeList[i].Name == "xs:schema")
                        {
                            if (!HasTopRowAdded)
                            {
                                xNode = inXmlNode.ChildNodes[i];
                                inTreeNode.Nodes.Add(Type);
                                inTreeNode.Nodes[i].Cells[0].Value = Type;
                                inTreeNode.Nodes[i].Cells[1].Value = "subtree";
                                tNode = inTreeNode.Nodes[i];
                                HasTopRowAdded = true;
                                AddNode(xNode, tNode, grid, Type);
                            }
                        }
                        else
                        {
                            xNode = inXmlNode.ChildNodes[i];
                            tNode = grid.Nodes[0];
                            AddNode(xNode, tNode, grid, Type);
                        }
                    }

                }
            }
            else
            {
                // Here you need to pull the data from the XmlNode based on the
                // type of node, whether attribute values are required, and so forth.
                //inTreeNode.Text = (inXmlNode.OuterXml).Trim();
            }
        }

        public string FirstLetterToUpper(string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }
        #endregion

        #region Node Function 
        private void AddNewNode(bool isParent)
        {
            frmAddNode addnode = new frmAddNode();
            if (addnode.ShowDialog() == DialogResult.OK)
            {
                TreeGridNode node;
                if (isParent)
                {
                    node = grOutput.CurrentNode.Parent.Nodes.Add(addnode.NodeName);
                    AddChildren(node, grOutput.CurrentNode);
                    MoveNode(node, grOutput.CurrentNode);
                }
                else
                    node = grOutput.CurrentNode.Nodes.Add(addnode.NodeName);

                node.Cells[1].Value = addnode.DataType;
            }
        }

        private void AddChildren(TreeGridNode parent, TreeGridNode child)
        {
            //TreeGridNode node = parent.Nodes.Add("Hello");
            TreeGridNode node = parent.Nodes.Add(child.Cells[0].Value.ToString());
            node.Cells[1].Value = child.Cells[1].Value;
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
        #endregion

        #region Create XML File
        private void CreateDefinition(TreeGridNode node, XElement element)
        {
            XElement el = null;
            if (node.Cells[0].Value.ToString().Contains("Output"))
            {
                el = new XElement(exportNs + "schema", new XAttribute("name", node.Cells[0].Value.ToString()), new XAttribute("type", "xs:" + node.Cells[1].Value.ToString().Replace("/", "")), new XAttribute("Alias", node.Cells[0].Value.ToString()));
                element.Add(el);
            }
            else
            {
                el = new XElement(exportNs + "element", new XAttribute("name", node.Cells[0].Value.ToString()), new XAttribute("type", "xs:" + node.Cells[1].Value.ToString().Replace("/", "")), new XAttribute("Alias", node.Cells[0].Value.ToString()));
                element.Add(el);
            }

            foreach (TreeGridNode subNode in node.Nodes)
            {
                CreateDefinition(subNode, el);
            }
        }
        #endregion

        #region XML to Move Functions
        bool isParentNode = false;
        private void CreateDefinitionMove(TreeGridNode node, XElement element, string Move, string CurrentNodeName, string CurrentNodeType, string MoveBeforNodeName, string MoveBeforNodeType)
        {
            string NodeName = "";
            string NodeType = "";
            XElement childNode;
            XElement el = null;
            string ChildNodeName = "";
            bool NodeNotAdd = false;
            if (Move == "Up")
            {
                if (node.Cells[0].Value.ToString() == MoveBeforNodeName && node.Cells[1].Value.ToString() == MoveBeforNodeType)
                {
                    if (!node.HasChildren)
                    {
                        NodeName = CurrentNodeName;
                        NodeType = CurrentNodeType;
                    }
                    else
                    {
                        isParentNode = true;
                        NodeName = MoveBeforNodeName;
                        NodeType = MoveBeforNodeType;

                        //foreach (TreeGridNode subNode in node.Nodes)
                        //{
                        //    ChildNodeName = LastChildofNode(subNode);
                        //}
                    }

                }
                else if (node.Cells[0].Value.ToString() == CurrentNodeName && node.Cells[1].Value.ToString() == CurrentNodeType)
                {
                    if (!isParentNode)
                    {
                        NodeName = MoveBeforNodeName;
                        NodeType = MoveBeforNodeType;
                    }
                    else
                    {
                        NodeNotAdd = true;

                    }
                }
                else
                {
                    NodeName = node.Cells[0].Value.ToString();
                    NodeType = node.Cells[1].Value.ToString();
                }
            }
            else if (Move == "Down")
            {
                if (node.Cells[0].Value.ToString() == MoveBeforNodeName && node.Cells[1].Value.ToString() == MoveBeforNodeType)
                {
                    NodeName = CurrentNodeName;
                    NodeType = CurrentNodeType;
                }
                else if (node.Cells[0].Value.ToString() == CurrentNodeName && node.Cells[1].Value.ToString() == CurrentNodeType)
                {
                    NodeName = MoveBeforNodeName;
                    NodeType = MoveBeforNodeType;
                }
                else
                {
                    NodeName = node.Cells[0].Value.ToString();
                    NodeType = node.Cells[1].Value.ToString();
                }
            }
            if (!NodeNotAdd)
            {
                el = new XElement(exportNs + "element", new XAttribute("name", NodeName), new XAttribute("type", "xs:" + NodeType.Replace("/", "").ToLower()), new XAttribute("Alias", NodeName));
                element.Add(el);
            }
            foreach (TreeGridNode subNode in node.Nodes)
            {
                CreateDefinitionMove(subNode, el, Move, CurrentNodeName, CurrentNodeType, MoveBeforNodeName, MoveBeforNodeType);
            }
        }

        private XDocument TreeViewConvertIntoXMLWithMoveNodes(TreeGridView tgv, string Move, string CurrentNodeName, string CurrentNodeType, string MoveBeforNodeName, string MoveBeforNodeType)
        {
            XDocument exportDoc = new XDocument(new XDeclaration("1.0", "utf-16", "yes"));
            exportNs = "http://www.w3.org/2001/XMLSchema";
            exportRoot = new XElement(exportNs + "schema", new XAttribute("attributeFormDefault", "unqualified"), new XAttribute("elementFormDefault", "qualified"), new XAttribute(XNamespace.Xmlns + "xs", exportNs));
            foreach (TreeGridNode node in tgv.Nodes[0].Nodes)
            {
                CreateDefinitionMove(node, exportRoot, Move, CurrentNodeName, CurrentNodeType, MoveBeforNodeName, MoveBeforNodeType);
            }
            exportDoc.Add(exportRoot);
            isParentNode = false;

            return exportDoc;
        }


        private string LastChildofNode(TreeGridNode node)
        {
            string NodeName = "";
            NodeName = node.Cells[0].Value.ToString();
            foreach (TreeGridNode subNode in node.Nodes)
            {
                LastChildofNode(subNode);
            }
            return NodeName;
        }
        #endregion

        #region ExpandNodes
        private void ExpandChildren(TreeGridNode parent)
        {
            parent.Expand();
            foreach (TreeGridNode node in parent.Nodes)
            {
                ExpandChildren(node);
            }
        }

        #endregion

        #region Create Input Object Class
        // Sampe 1
        private void CreateDefinitionData(TreeGridNode node, StringBuilder sb)
        {
            var n = (node as TreeGridNode);

            if (n != null)
                sb.AppendFormat("public string ")
                       .Append(n.Cells[0].Value).Append(" { get; set; }");
            foreach (TreeGridNode subNode in node.Nodes)
            {
                CreateDefinitionData(subNode, sb);
            }
        }

        private string CreateClassObject(TreeGridView tgv)
        {

            StringBuilder builder = new StringBuilder();

            var className = "DataInput";

            builder.AppendFormat("public class ").Append(className).Append(" { ");

            foreach (TreeGridNode node in tgv.Nodes[0].Nodes)
            {
                CreateDefinitionData(node, builder);
            }
            builder.Append(" } ");

            var compilerParameters = new CompilerParameters();

            compilerParameters.GenerateExecutable = false;
            compilerParameters.GenerateInMemory = true;

            var cCompiler = CSharpCodeProvider.CreateProvider("CSharp");
            var compileResult = cCompiler.CompileAssemblyFromSource(compilerParameters, builder.ToString());

            if (compileResult.Errors.HasErrors)
            {
                throw new Exception("There is error while building type");
            }
            var instanceObject = compileResult.CompiledAssembly.CreateInstance(className);

            return builder.ToString();
        }
        //

        // Sample 2--- Working Code
        private string CreateDynamicClassObjects(TreeGridView tgv)
        {
            string ClassesCode = "";
            DCObject = new List<DynamicClassProperties>();
            foreach (TreeGridNode node in tgv.Nodes[0].Nodes)
            {
                CreateDefinitionDynamicClassProperties(node, DCObject);
            }

            ClassesCode = CreateDynamicClasses().ToString();

            return ClassesCode;
        }

        private void CreateDefinitionDynamicClassProperties(TreeGridNode node, List<DynamicClassProperties> prop)
        {
            var n = (node as TreeGridNode);

            if (n != null)
            {
                DCObject.Add(new DynamicClassProperties
                {
                    PropertyName = node.Cells[0].Value.ToString(),
                    PropertyType = node.Cells[1].Value,
                    ParentNodeName = node.Parent.Cells[0].Value.ToString(),
                    ClassName = node.Cells[0].Value.ToString(),
                    IsParent = node.HasChildren
                });
            }
            foreach (TreeGridNode subNode in node.Nodes)
            {
                CreateDefinitionDynamicClassProperties(subNode, prop);
            }
        }

        private StringBuilder CreateDynamicClasses()
        {
            StringBuilder builder = new StringBuilder();
            // Get All Classess
            var classes = DCObject.Where(x => x.IsParent == true).ToList();
            string classNameType = classes[0].PropertyName;
            InitializeObjects.Add(classNameType);
            foreach (var c in classes)
            {
                // Set Class Name
                string className = "";
                if (classNameType == c.ClassName)
                    className = c.ClassName;
                else
                    className = classNameType + "_" + c.ClassName;

                // Create Class Object
                builder.AppendFormat("public class ").Append(className).Append(" { ");
                // Get Parent and Child
                var Parent_Child = DCObject.Where(x => x.ParentNodeName == c.PropertyName).ToList();

                foreach (var cp in Parent_Child)
                {
                    if (cp.IsParent)
                    {
                        builder.AppendFormat("public ").Append(classNameType + "_" + cp.PropertyName + " ").Append(cp.PropertyName).Append(" { get; set; }").Append(Environment.NewLine);
                    }
                    else
                    {
                        builder.AppendFormat("public ").Append(cp.PropertyType + " ").Append(cp.PropertyName).Append(" { get; set; }").Append(Environment.NewLine);
                    }
                }
                builder.Append(" } ");
                builder.Append(Environment.NewLine);
            }

            return builder;
        }
        //
        #endregion

        #region Log Method
        
        public string LogMethod()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"Log.txt";
            string text = "public static void Log(object Message)";
            text += "{";
            text += "StreamWriter sw = null;";
            text += "try";
            text += "{";
            text += "sw = new StreamWriter(@\"" + path + "\", true);";
            text += "sw.WriteLine(Message);";
            text += "sw.Flush();";
            text += "sw.Close();";
            text += "}";
            text += "catch";
            text += "{";
            text += "}";
            text += "}";
            return text;
        }
        #endregion

        #region Events
        // Form Events
        private void Form1_Load(object sender, EventArgs e)
        {
            // Bind Input TreeView
            ReadXML(grInput, "Input");
            ExpandChildren(grInput.Nodes[0]);
            HasTopRowAdded = false;
            // Bind OutPut Treeview
            ReadXML(grOutput, "Output");
            HasTopRowAdded = false;
            ExpandChildren(grOutput.Nodes[0]);
            // Assign Sample Code
            txtSample.Text = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "SampleCode.txt");


        }

        private void grOutput_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void grOutput_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
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
            DataGridViewComboBoxCell box = grOutput.Rows[e.RowIndex].Cells[1] as DataGridViewComboBoxCell;
            box.DataSource = lists;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddNewNode(false);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove this node and all its children?", "XML Converter", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
            {
                grOutput.CurrentNode.Parent.Nodes.Remove(grOutput.CurrentNode);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string filename = DateTime.Now.ToString("ddMMyyyyhhmmss") + ".xml";
            XDocument exportDoc = new XDocument(new XDeclaration("1.0", "utf-16", "yes"));
            exportNs = "http://www.w3.org/2001/XMLSchema";
            exportRoot = new XElement(exportNs + "Root", new XAttribute("attributeFormDefault", "unqualified"), new XAttribute("elementFormDefault", "qualified"), new XAttribute(XNamespace.Xmlns + "xs", exportNs));
            foreach (TreeGridNode node in grOutput.Nodes[0].Nodes)
            {
                CreateDefinition(node, exportRoot);
            }
            exportDoc.Add(exportRoot);
            exportDoc.Save(filename);

            MessageBox.Show("Schema file generated successfully!", "XML Converter", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            TreeGridNode currentNode = grOutput.CurrentNode;
            string Type = "Output_0";
            // Row Indexes
            int CurrentRowIndex = currentNode.Index;
            int NextRowIndex = currentNode.Index + 1;
            int TotalRowCount = currentNode.Parent.Nodes.Count;

            if (NextRowIndex != TotalRowCount)
            {
                string CurrentNodeName = currentNode.Parent.Nodes[CurrentRowIndex].Cells[0].Value.ToString();
                string CurrentNodeType = currentNode.Parent.Nodes[CurrentRowIndex].Cells[1].Value.ToString();
                string PreviousNodeName = currentNode.Parent.Nodes[NextRowIndex].Cells[0].Value.ToString();
                string PreviousNodeType = currentNode.Parent.Nodes[NextRowIndex].Cells[1].Value.ToString();

                XDocument doc = TreeViewConvertIntoXMLWithMoveNodes(grOutput, "Down", CurrentNodeName, CurrentNodeType, PreviousNodeName, PreviousNodeType);

                // Binding Treeview again
                XmlDocument dom = new XmlDocument();
                dom.Load(doc.CreateReader());

                // SECTION 2. Initialize the TreeView control.
                grOutput.Nodes.Clear();
                grOutput.Nodes.Add("Root");
                TreeGridNode tNode = new TreeGridNode();
                tNode = grOutput.Nodes[0];

                // SECTION 3. Populate the TreeView with the DOM nodes.
                AddNode(dom.DocumentElement, tNode, grOutput, Type);
                // Expand Nodes
                ExpandChildren(grOutput.Nodes[0]);
            }

        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            TreeGridNode currentNode = grOutput.CurrentNode;
            string Type = "Output_0";
            // Row Indexes
            int CurrentRowIndex = currentNode.Index;
            int PreviousRowIndex = currentNode.Index - 1;
            int TotalRowCount = currentNode.Parent.Nodes.Count;

            string CurrentNodeName = currentNode.Parent.Nodes[CurrentRowIndex].Cells[0].Value.ToString();
            if (CurrentNodeName != "Root")
            {
                string CurrentNodeType = currentNode.Parent.Nodes[CurrentRowIndex].Cells[1].Value.ToString();
                string PreviousNodeName = currentNode.Parent.Nodes[PreviousRowIndex].Cells[0].Value.ToString();
                string PreviousNodeType = currentNode.Parent.Nodes[PreviousRowIndex].Cells[1].Value.ToString();
                XDocument doc = TreeViewConvertIntoXMLWithMoveNodes(grOutput, "Up", CurrentNodeName, CurrentNodeType, PreviousNodeName, PreviousNodeType);

                // Binding Treeview again
                XmlDocument dom = new XmlDocument();
                dom.Load(doc.CreateReader());

                // SECTION 2. Initialize the TreeView control.
                grOutput.Nodes.Clear();
                grOutput.Nodes.Add("Root");
                TreeGridNode tNode = new TreeGridNode();
                tNode = grOutput.Nodes[0];

                // SECTION 3. Populate the TreeView with the DOM nodes.
                AddNode(dom.DocumentElement, tNode, grOutput, Type);
                // Expand Nodes
                ExpandChildren(grOutput.Nodes[0]);
                //currentNode.Parent.Nodes[CurrentRowIndex].Selected = true;
            }

        }

        private void btnFindError_Click(object sender, EventArgs e)
        {
            //TreeView Input Class Object
            string InpuntClassObject = CreateDynamicClassObjects(grInput);

            //TreeView Output Class Object
            string OutputClassObject = CreateDynamicClassObjects(grOutput);

            //Get Initialize Objects into String
            string inObject = "";
            foreach (var item in InitializeObjects)
            {
                inObject = inObject + item + " " + item + " = " + " new " + item + "();" + Environment.NewLine;
            }
            InitializeObjects = new List<string>();
            // Code literal  
            string code =
                @"using System;  
                  using System.Windows.Forms;  
                  using System.IO;
                  namespace WinFormCodeCompile  
                  {  
                    " + InpuntClassObject + @"
                    " + OutputClassObject + @"
                      public class Transform  
                      {  
                           " + inObject + @"
                        " + LogMethod() + @"
                           public void  UpdateText()  
                           {
                                " + txtScript.Text + @"  
                                
                           }  
                       }  
                   }";

            // Compile code  
            CSharpCodeProvider cProv = new CSharpCodeProvider();
            CompilerParameters cParams = new CompilerParameters();
            cParams.ReferencedAssemblies.Add("mscorlib.dll");
            cParams.ReferencedAssemblies.Add("System.dll");
            cParams.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            cParams.GenerateExecutable = false;
            cParams.GenerateInMemory = true;

            CompilerResults cResults = cProv.CompileAssemblyFromSource(cParams, code);
            txtCompileStatus.Text = "";
            // Check for errors  
            if (cResults.Errors.Count > 0)
            {
                foreach (CompilerError CompErr in cResults.Errors)
                {
                    //lblCompileStatus.ForeColor = Color.Red;
                    txtCompileStatus.ForeColor = Color.Red;
                    txtCompileStatus.Text = txtCompileStatus.Text +
                                //"Line number " + CompErr.Line +
                                " Error Number: " + CompErr.ErrorNumber +
                                ", '" + CompErr.ErrorText + ";" +
                                Environment.NewLine;
                }
                return;
            }
            else
            {
                //Successful Compile
                //lblCompileStatus.ForeColor = Color.Green;
                txtCompileStatus.ForeColor = Color.Green;
                txtCompileStatus.Text = "Success!";
            }
            Assembly loAssembly = cResults.CompiledAssembly;
            // Retrieve an obj ref - generic type only
            object loObject =
                   loAssembly.CreateInstance("WinFormCodeCompile.Transform");
            if (loObject == null)
            {
                MessageBox.Show("Couldn't load class.");
                return;
            }
            object[] loCodeParms = new object[1];
            loCodeParms[0] = "West Wind Technologies";
            try
            {
                object loResult = loObject.GetType().InvokeMember("UpdateText", BindingFlags.InvokeMethod,null, loObject, null);
            }
            catch (Exception loError)
            {
                MessageBox.Show(loError.Message, "Compiler Demo");
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }

        private void btnOk_Click(object sender, EventArgs e)
        {

        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtSample.SelectedText);
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            txtScript.Text += Clipboard.GetText();
        }
        #endregion


    }

    #region Dynamic Class Properties Object
    public class DynamicClassProperties
    {

        public string PropertyName { get; set; }

        public object PropertyType { get; set; }

        public string ClassName { get; set; }

        public string ParentNodeName { get; set; }

        public bool IsParent { get; set; }
    }
    #endregion

    

}
