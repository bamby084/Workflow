namespace XML_Converter
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tbcMain = new System.Windows.Forms.TabControl();
            this.tabProperties = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tblInput = new System.Windows.Forms.TableLayoutPanel();
            this.lblTextEncoding = new System.Windows.Forms.Label();
            this.lblInputFile = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtInputFile = new System.Windows.Forms.TextBox();
            this.btnInputBrowse = new System.Windows.Forms.Button();
            this.txtTextEncoding = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.grpDTD = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.btnGenDTD = new System.Windows.Forms.Button();
            this.btnGenReadDTD = new System.Windows.Forms.Button();
            this.btnExportDTD = new System.Windows.Forms.Button();
            this.btnReadDTD = new System.Windows.Forms.Button();
            this.grpXML = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.btnGenReadXML = new System.Windows.Forms.Button();
            this.btnExportXML = new System.Windows.Forms.Button();
            this.btnReadXML = new System.Windows.Forms.Button();
            this.btnIgnoreXML = new System.Windows.Forms.CheckBox();
            this.gridNodes = new AdvancedDataGridView.TreeGridView();
            this.NodeName = new AdvancedDataGridView.TreeGridColumn();
            this.NodeType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.NodeOptionality = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.NodeChange = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.NodeXMLName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NodeFieldConverter = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.removeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addChildMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addParentMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabScripts = new System.Windows.Forms.TabPage();
            this.inputFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.nodeBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tbcMain.SuspendLayout();
            this.tabProperties.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tblInput.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.grpDTD.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.grpXML.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridNodes)).BeginInit();
            this.panel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nodeBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // tbcMain
            // 
            this.tbcMain.Controls.Add(this.tabProperties);
            this.tbcMain.Controls.Add(this.tabScripts);
            this.tbcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbcMain.Location = new System.Drawing.Point(0, 0);
            this.tbcMain.Margin = new System.Windows.Forms.Padding(5);
            this.tbcMain.Name = "tbcMain";
            this.tbcMain.SelectedIndex = 0;
            this.tbcMain.Size = new System.Drawing.Size(1069, 617);
            this.tbcMain.TabIndex = 1;
            // 
            // tabProperties
            // 
            this.tabProperties.Controls.Add(this.tableLayoutPanel2);
            this.tabProperties.Location = new System.Drawing.Point(4, 30);
            this.tabProperties.Margin = new System.Windows.Forms.Padding(5);
            this.tabProperties.Name = "tabProperties";
            this.tabProperties.Padding = new System.Windows.Forms.Padding(5);
            this.tabProperties.Size = new System.Drawing.Size(1061, 583);
            this.tabProperties.TabIndex = 0;
            this.tabProperties.Text = "Properties";
            this.tabProperties.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.tblInput, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.gridNodes, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.panel1, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.menuStrip1, 0, 3);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(5, 5);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 82F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 119F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1051, 573);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // tblInput
            // 
            this.tblInput.ColumnCount = 2;
            this.tblInput.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tblInput.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.tblInput.Controls.Add(this.lblTextEncoding, 0, 1);
            this.tblInput.Controls.Add(this.lblInputFile, 0, 0);
            this.tblInput.Controls.Add(this.tableLayoutPanel1, 1, 0);
            this.tblInput.Controls.Add(this.txtTextEncoding, 1, 1);
            this.tblInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblInput.Location = new System.Drawing.Point(3, 4);
            this.tblInput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tblInput.Name = "tblInput";
            this.tblInput.RowCount = 2;
            this.tblInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblInput.Size = new System.Drawing.Size(1045, 74);
            this.tblInput.TabIndex = 0;
            // 
            // lblTextEncoding
            // 
            this.lblTextEncoding.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTextEncoding.Location = new System.Drawing.Point(3, 37);
            this.lblTextEncoding.Name = "lblTextEncoding";
            this.lblTextEncoding.Size = new System.Drawing.Size(203, 37);
            this.lblTextEncoding.TabIndex = 2;
            this.lblTextEncoding.Text = "Text Encoding:";
            this.lblTextEncoding.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblInputFile
            // 
            this.lblInputFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblInputFile.Location = new System.Drawing.Point(3, 0);
            this.lblInputFile.Name = "lblInputFile";
            this.lblInputFile.Size = new System.Drawing.Size(203, 37);
            this.lblInputFile.TabIndex = 0;
            this.lblInputFile.Text = "Input File: ";
            this.lblInputFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.Controls.Add(this.txtInputFile, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnInputBrowse, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(212, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(830, 31);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // txtInputFile
            // 
            this.txtInputFile.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.txtInputFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInputFile.Location = new System.Drawing.Point(3, 3);
            this.txtInputFile.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.txtInputFile.Name = "txtInputFile";
            this.txtInputFile.ReadOnly = true;
            this.txtInputFile.Size = new System.Drawing.Size(727, 29);
            this.txtInputFile.TabIndex = 0;
            // 
            // btnInputBrowse
            // 
            this.btnInputBrowse.Location = new System.Drawing.Point(730, 2);
            this.btnInputBrowse.Margin = new System.Windows.Forms.Padding(0, 2, 3, 1);
            this.btnInputBrowse.Name = "btnInputBrowse";
            this.btnInputBrowse.Size = new System.Drawing.Size(97, 27);
            this.btnInputBrowse.TabIndex = 1;
            this.btnInputBrowse.Text = "Browse";
            this.btnInputBrowse.UseVisualStyleBackColor = true;
            this.btnInputBrowse.Click += new System.EventHandler(this.btnInputBrowse_Click);
            // 
            // txtTextEncoding
            // 
            this.txtTextEncoding.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTextEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.txtTextEncoding.FormattingEnabled = true;
            this.txtTextEncoding.Location = new System.Drawing.Point(212, 43);
            this.txtTextEncoding.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.txtTextEncoding.Name = "txtTextEncoding";
            this.txtTextEncoding.Size = new System.Drawing.Size(830, 29);
            this.txtTextEncoding.TabIndex = 3;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.grpDTD, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.grpXML, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 102);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(3, 20, 3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 96F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1045, 96);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // grpDTD
            // 
            this.grpDTD.Controls.Add(this.tableLayoutPanel4);
            this.grpDTD.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpDTD.Location = new System.Drawing.Point(3, 3);
            this.grpDTD.Name = "grpDTD";
            this.grpDTD.Size = new System.Drawing.Size(516, 90);
            this.grpDTD.TabIndex = 0;
            this.grpDTD.TabStop = false;
            this.grpDTD.Text = "DTD";
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.btnGenDTD, 1, 1);
            this.tableLayoutPanel4.Controls.Add(this.btnGenReadDTD, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.btnExportDTD, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.btnReadDTD, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 25);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(510, 62);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // btnGenDTD
            // 
            this.btnGenDTD.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnGenDTD.Location = new System.Drawing.Point(258, 34);
            this.btnGenDTD.Name = "btnGenDTD";
            this.btnGenDTD.Size = new System.Drawing.Size(249, 25);
            this.btnGenDTD.TabIndex = 3;
            this.btnGenDTD.Text = "Generate";
            this.btnGenDTD.UseVisualStyleBackColor = true;
            // 
            // btnGenReadDTD
            // 
            this.btnGenReadDTD.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnGenReadDTD.Location = new System.Drawing.Point(3, 34);
            this.btnGenReadDTD.Name = "btnGenReadDTD";
            this.btnGenReadDTD.Size = new System.Drawing.Size(249, 25);
            this.btnGenReadDTD.TabIndex = 2;
            this.btnGenReadDTD.Text = "Generate and Read";
            this.btnGenReadDTD.UseVisualStyleBackColor = true;
            // 
            // btnExportDTD
            // 
            this.btnExportDTD.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnExportDTD.Location = new System.Drawing.Point(258, 3);
            this.btnExportDTD.Name = "btnExportDTD";
            this.btnExportDTD.Size = new System.Drawing.Size(249, 25);
            this.btnExportDTD.TabIndex = 1;
            this.btnExportDTD.Text = "Export";
            this.btnExportDTD.UseVisualStyleBackColor = true;
            // 
            // btnReadDTD
            // 
            this.btnReadDTD.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnReadDTD.Location = new System.Drawing.Point(3, 3);
            this.btnReadDTD.Name = "btnReadDTD";
            this.btnReadDTD.Size = new System.Drawing.Size(249, 25);
            this.btnReadDTD.TabIndex = 0;
            this.btnReadDTD.Text = "Read";
            this.btnReadDTD.UseVisualStyleBackColor = true;
            // 
            // grpXML
            // 
            this.grpXML.Controls.Add(this.tableLayoutPanel5);
            this.grpXML.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpXML.Location = new System.Drawing.Point(525, 3);
            this.grpXML.Name = "grpXML";
            this.grpXML.Size = new System.Drawing.Size(517, 90);
            this.grpXML.TabIndex = 1;
            this.grpXML.TabStop = false;
            this.grpXML.Text = "XML Schema";
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Controls.Add(this.btnGenReadXML, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.btnExportXML, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.btnReadXML, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.btnIgnoreXML, 1, 1);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 25);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(511, 62);
            this.tableLayoutPanel5.TabIndex = 1;
            // 
            // btnGenReadXML
            // 
            this.btnGenReadXML.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnGenReadXML.Location = new System.Drawing.Point(3, 34);
            this.btnGenReadXML.Name = "btnGenReadXML";
            this.btnGenReadXML.Size = new System.Drawing.Size(249, 25);
            this.btnGenReadXML.TabIndex = 2;
            this.btnGenReadXML.Text = "Generate and Read";
            this.btnGenReadXML.UseVisualStyleBackColor = true;
            this.btnGenReadXML.Click += new System.EventHandler(this.btnGenReadXML_Click);
            // 
            // btnExportXML
            // 
            this.btnExportXML.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnExportXML.Location = new System.Drawing.Point(258, 3);
            this.btnExportXML.Name = "btnExportXML";
            this.btnExportXML.Size = new System.Drawing.Size(250, 25);
            this.btnExportXML.TabIndex = 1;
            this.btnExportXML.Text = "Export";
            this.btnExportXML.UseVisualStyleBackColor = true;
            this.btnExportXML.Click += new System.EventHandler(this.btnExportXML_Click);
            // 
            // btnReadXML
            // 
            this.btnReadXML.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnReadXML.Location = new System.Drawing.Point(3, 3);
            this.btnReadXML.Name = "btnReadXML";
            this.btnReadXML.Size = new System.Drawing.Size(249, 25);
            this.btnReadXML.TabIndex = 0;
            this.btnReadXML.Text = "Read";
            this.btnReadXML.UseVisualStyleBackColor = true;
            this.btnReadXML.Click += new System.EventHandler(this.btnReadXML_Click);
            // 
            // btnIgnoreXML
            // 
            this.btnIgnoreXML.AutoSize = true;
            this.btnIgnoreXML.Location = new System.Drawing.Point(258, 37);
            this.btnIgnoreXML.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.btnIgnoreXML.Name = "btnIgnoreXML";
            this.btnIgnoreXML.Size = new System.Drawing.Size(129, 22);
            this.btnIgnoreXML.TabIndex = 3;
            this.btnIgnoreXML.Text = "Ignore Types";
            this.btnIgnoreXML.UseVisualStyleBackColor = true;
            // 
            // gridNodes
            // 
            this.gridNodes.AllowUserToAddRows = false;
            this.gridNodes.AllowUserToDeleteRows = false;
            this.gridNodes.BackgroundColor = System.Drawing.Color.White;
            this.gridNodes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NodeName,
            this.NodeType,
            this.NodeOptionality,
            this.NodeChange,
            this.NodeXMLName,
            this.NodeFieldConverter});
            this.gridNodes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridNodes.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.gridNodes.ImageList = null;
            this.gridNodes.Location = new System.Drawing.Point(3, 204);
            this.gridNodes.MultiSelect = false;
            this.gridNodes.Name = "gridNodes";
            this.gridNodes.Size = new System.Drawing.Size(1045, 286);
            this.gridNodes.TabIndex = 2;
            this.gridNodes.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridNodes_CellContentClick);
            // 
            // NodeName
            // 
            this.NodeName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.NodeName.DefaultNodeImage = null;
            this.NodeName.HeaderText = "Name";
            this.NodeName.Name = "NodeName";
            this.NodeName.ReadOnly = true;
            this.NodeName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // NodeType
            // 
            this.NodeType.HeaderText = "Type";
            this.NodeType.Items.AddRange(new object[] {
            "Element",
            "Attribute",
            "PCData"});
            this.NodeType.MinimumWidth = 50;
            this.NodeType.Name = "NodeType";
            this.NodeType.Width = 150;
            // 
            // NodeOptionality
            // 
            this.NodeOptionality.HeaderText = "Optionality";
            this.NodeOptionality.Items.AddRange(new object[] {
            "One",
            "Zero or one",
            "Zero or more",
            "One or more"});
            this.NodeOptionality.MinimumWidth = 50;
            this.NodeOptionality.Name = "NodeOptionality";
            this.NodeOptionality.Width = 150;
            // 
            // NodeChange
            // 
            this.NodeChange.HeaderText = "Change";
            this.NodeChange.Items.AddRange(new object[] {
            "None",
            "Ignore",
            "Flatten"});
            this.NodeChange.MinimumWidth = 50;
            this.NodeChange.Name = "NodeChange";
            this.NodeChange.Width = 150;
            // 
            // NodeXMLName
            // 
            this.NodeXMLName.HeaderText = "XML Name";
            this.NodeXMLName.MinimumWidth = 70;
            this.NodeXMLName.Name = "NodeXMLName";
            this.NodeXMLName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.NodeXMLName.Width = 150;
            // 
            // NodeFieldConverter
            // 
            this.NodeFieldConverter.HeaderText = "Data Type";
            this.NodeFieldConverter.Items.AddRange(new object[] {
            "String",
            "Int16",
            "Int32",
            "Int64",
            "Boolean",
            "Date/Time",
            "Double",
            "Single"});
            this.NodeFieldConverter.MinimumWidth = 50;
            this.NodeFieldConverter.Name = "NodeFieldConverter";
            this.NodeFieldConverter.Width = 150;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 526);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1045, 44);
            this.panel1.TabIndex = 3;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnOK.Location = new System.Drawing.Point(373, 11);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(115, 30);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.Location = new System.Drawing.Point(498, 11);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(115, 30);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeMenuItem,
            this.addChildMenuItem,
            this.addParentMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 493);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.menuStrip1.ShowItemToolTips = true;
            this.menuStrip1.Size = new System.Drawing.Size(1051, 30);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // removeMenuItem
            // 
            this.removeMenuItem.Image = global::XML_Converter.Properties.Resources.if_x_circle_2561211;
            this.removeMenuItem.Name = "removeMenuItem";
            this.removeMenuItem.Size = new System.Drawing.Size(44, 26);
            this.removeMenuItem.ToolTipText = "Remove Node";
            this.removeMenuItem.Click += new System.EventHandler(this.removeMenuItem_Click);
            // 
            // addChildMenuItem
            // 
            this.addChildMenuItem.Image = global::XML_Converter.Properties.Resources.if_check_square_2561354;
            this.addChildMenuItem.Name = "addChildMenuItem";
            this.addChildMenuItem.Size = new System.Drawing.Size(44, 26);
            this.addChildMenuItem.ToolTipText = "Add Child Node";
            this.addChildMenuItem.Click += new System.EventHandler(this.addChildMenuItem_Click);
            // 
            // addParentMenuItem
            // 
            this.addParentMenuItem.Image = global::XML_Converter.Properties.Resources._new;
            this.addParentMenuItem.Name = "addParentMenuItem";
            this.addParentMenuItem.Size = new System.Drawing.Size(44, 26);
            this.addParentMenuItem.ToolTipText = "Add Parent Node";
            this.addParentMenuItem.Click += new System.EventHandler(this.addParentMenuItem_Click);
            // 
            // tabScripts
            // 
            this.tabScripts.Location = new System.Drawing.Point(4, 30);
            this.tabScripts.Margin = new System.Windows.Forms.Padding(5);
            this.tabScripts.Name = "tabScripts";
            this.tabScripts.Padding = new System.Windows.Forms.Padding(5);
            this.tabScripts.Size = new System.Drawing.Size(1061, 583);
            this.tabScripts.TabIndex = 1;
            this.tabScripts.Text = "Scripts";
            this.tabScripts.UseVisualStyleBackColor = true;
            // 
            // inputFileDialog
            // 
            this.inputFileDialog.Filter = "XML files|*.xml";
            this.inputFileDialog.Title = "XML Converter";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "XML Files|*.xml";
            this.saveFileDialog.Title = "XML Converter";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1069, 617);
            this.Controls.Add(this.tbcMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmMain";
            this.Text = "XML Converter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.tbcMain.ResumeLayout(false);
            this.tabProperties.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tblInput.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.grpDTD.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.grpXML.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridNodes)).EndInit();
            this.panel1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nodeBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.TabControl tbcMain;
        internal System.Windows.Forms.TabPage tabProperties;
        internal System.Windows.Forms.TableLayoutPanel tblInput;
        internal System.Windows.Forms.Label lblInputFile;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox txtInputFile;
        private System.Windows.Forms.Button btnInputBrowse;
        internal System.Windows.Forms.TabPage tabScripts;
        internal System.Windows.Forms.Label lblTextEncoding;
        private System.Windows.Forms.ComboBox txtTextEncoding;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.GroupBox grpDTD;
        private System.Windows.Forms.GroupBox grpXML;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Button btnGenDTD;
        private System.Windows.Forms.Button btnGenReadDTD;
        private System.Windows.Forms.Button btnExportDTD;
        private System.Windows.Forms.Button btnReadDTD;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Button btnGenReadXML;
        private System.Windows.Forms.Button btnExportXML;
        private System.Windows.Forms.Button btnReadXML;
        private System.Windows.Forms.CheckBox btnIgnoreXML;
        private System.Windows.Forms.BindingSource nodeBindingSource;
        private AdvancedDataGridView.TreeGridView gridNodes;
        private System.Windows.Forms.OpenFileDialog inputFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem addParentMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addChildMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeMenuItem;
        private AdvancedDataGridView.TreeGridColumn NodeName;
        private System.Windows.Forms.DataGridViewComboBoxColumn NodeType;
        private System.Windows.Forms.DataGridViewComboBoxColumn NodeOptionality;
        private System.Windows.Forms.DataGridViewComboBoxColumn NodeChange;
        private System.Windows.Forms.DataGridViewTextBoxColumn NodeXMLName;
        private System.Windows.Forms.DataGridViewComboBoxColumn NodeFieldConverter;
    }
}

