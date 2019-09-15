namespace ScriptingApp
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.lblDataInput = new System.Windows.Forms.Label();
            this.grInput = new AdvancedDataGridView.TreeGridView();
            this.Structure = new AdvancedDataGridView.TreeGridColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grOutput = new AdvancedDataGridView.TreeGridView();
            this.treeGridColumn1 = new AdvancedDataGridView.TreeGridColumn();
            this.dataGridViewComboBoxColumn1 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.lblDataOutput = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lblDataInputCount = new System.Windows.Forms.Label();
            this.txtdatainputcount = new System.Windows.Forms.NumericUpDown();
            this.txtdataoutputcount = new System.Windows.Forms.NumericUpDown();
            this.lblDataoutputcount = new System.Windows.Forms.Label();
            this.txtSheetoutputcount = new System.Windows.Forms.NumericUpDown();
            this.lblSheetoutputcount = new System.Windows.Forms.Label();
            this.txtSheetinputcount = new System.Windows.Forms.NumericUpDown();
            this.lblSheetinputcount = new System.Windows.Forms.Label();
            this.chkindexsheet = new System.Windows.Forms.CheckBox();
            this.chkpreprocess = new System.Windows.Forms.CheckBox();
            this.tblLayoutPane1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtSample = new System.Windows.Forms.RichTextBox();
            this.lblScript = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnFindError = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabSample = new System.Windows.Forms.TabPage();
            this.btnCopy = new System.Windows.Forms.Button();
            this.tabScript = new System.Windows.Forms.TabPage();
            this.txtScript = new System.Windows.Forms.RichTextBox();
            this.txtCompileStatus = new System.Windows.Forms.RichTextBox();
            this.lblCompileStatus = new System.Windows.Forms.Label();
            this.btnPaste = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.grInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grOutput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtdatainputcount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtdataoutputcount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSheetoutputcount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSheetinputcount)).BeginInit();
            this.tblLayoutPane1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabSample.SuspendLayout();
            this.tabScript.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblDataInput
            // 
            this.lblDataInput.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblDataInput.AutoSize = true;
            this.lblDataInput.Location = new System.Drawing.Point(3, 0);
            this.lblDataInput.Name = "lblDataInput";
            this.lblDataInput.Size = new System.Drawing.Size(71, 16);
            this.lblDataInput.TabIndex = 0;
            this.lblDataInput.Text = "Data input:";
            // 
            // grInput
            // 
            this.grInput.AllowUserToAddRows = false;
            this.grInput.AllowUserToDeleteRows = false;
            this.grInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grInput.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grInput.BackgroundColor = System.Drawing.Color.White;
            this.grInput.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Structure,
            this.Type});
            this.grInput.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.grInput.ImageList = null;
            this.grInput.Location = new System.Drawing.Point(3, 20);
            this.grInput.Name = "grInput";
            this.grInput.RowHeadersVisible = false;
            this.grInput.Size = new System.Drawing.Size(386, 155);
            this.grInput.TabIndex = 1;
            // 
            // Structure
            // 
            this.Structure.DefaultNodeImage = null;
            this.Structure.HeaderText = "Structure";
            this.Structure.Name = "Structure";
            this.Structure.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Structure.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Type
            // 
            this.Type.HeaderText = "Type";
            this.Type.Name = "Type";
            this.Type.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // grOutput
            // 
            this.grOutput.AllowDrop = true;
            this.grOutput.AllowUserToAddRows = false;
            this.grOutput.AllowUserToDeleteRows = false;
            this.grOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grOutput.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grOutput.BackgroundColor = System.Drawing.Color.White;
            this.grOutput.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.treeGridColumn1,
            this.dataGridViewComboBoxColumn1});
            this.grOutput.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnF2;
            this.grOutput.ImageList = null;
            this.grOutput.Location = new System.Drawing.Point(3, 213);
            this.grOutput.Name = "grOutput";
            this.grOutput.RowHeadersVisible = false;
            this.grOutput.Size = new System.Drawing.Size(386, 438);
            this.grOutput.TabIndex = 3;
            this.grOutput.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.grOutput_DataError);
            this.grOutput.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.grOutput_RowsAdded);
            // 
            // treeGridColumn1
            // 
            this.treeGridColumn1.DefaultNodeImage = null;
            this.treeGridColumn1.HeaderText = "Structure";
            this.treeGridColumn1.Name = "treeGridColumn1";
            this.treeGridColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewComboBoxColumn1
            // 
            this.dataGridViewComboBoxColumn1.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.dataGridViewComboBoxColumn1.DisplayStyleForCurrentCellOnly = true;
            this.dataGridViewComboBoxColumn1.HeaderText = "Type";
            this.dataGridViewComboBoxColumn1.Name = "dataGridViewComboBoxColumn1";
            // 
            // lblDataOutput
            // 
            this.lblDataOutput.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblDataOutput.AutoSize = true;
            this.lblDataOutput.Location = new System.Drawing.Point(3, 186);
            this.lblDataOutput.Name = "lblDataOutput";
            this.lblDataOutput.Size = new System.Drawing.Size(79, 16);
            this.lblDataOutput.TabIndex = 2;
            this.lblDataOutput.Text = "Data output:";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.BackgroundImage = global::ScriptingApp.Properties.Resources.Save;
            this.btnSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSave.Location = new System.Drawing.Point(357, 662);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(35, 39);
            this.btnSave.TabIndex = 7;
            this.btnSave.Tag = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdd.BackColor = System.Drawing.Color.Transparent;
            this.btnAdd.BackgroundImage = global::ScriptingApp.Properties.Resources.Add;
            this.btnAdd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnAdd.FlatAppearance.BorderSize = 0;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnAdd.Location = new System.Drawing.Point(161, 662);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(35, 39);
            this.btnAdd.TabIndex = 6;
            this.btnAdd.Tag = "Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnUp
            // 
            this.btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUp.BackColor = System.Drawing.Color.Transparent;
            this.btnUp.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnUp.BackgroundImage")));
            this.btnUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnUp.FlatAppearance.BorderSize = 0;
            this.btnUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUp.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnUp.Location = new System.Drawing.Point(212, 662);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(35, 39);
            this.btnUp.TabIndex = 5;
            this.btnUp.Tag = "Up";
            this.btnUp.UseVisualStyleBackColor = false;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDown.BackColor = System.Drawing.Color.Transparent;
            this.btnDown.BackgroundImage = global::ScriptingApp.Properties.Resources.DownArrow;
            this.btnDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnDown.FlatAppearance.BorderSize = 0;
            this.btnDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDown.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnDown.Location = new System.Drawing.Point(270, 662);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(35, 39);
            this.btnDown.TabIndex = 4;
            this.btnDown.Tag = "Down";
            this.btnDown.UseVisualStyleBackColor = false;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDelete.BackColor = System.Drawing.Color.Transparent;
            this.btnDelete.BackgroundImage = global::ScriptingApp.Properties.Resources.remove;
            this.btnDelete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnDelete.Location = new System.Drawing.Point(319, 662);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(35, 39);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.Tag = "Remove";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lblDataInputCount
            // 
            this.lblDataInputCount.AutoSize = true;
            this.lblDataInputCount.Location = new System.Drawing.Point(401, 17);
            this.lblDataInputCount.Name = "lblDataInputCount";
            this.lblDataInputCount.Size = new System.Drawing.Size(107, 16);
            this.lblDataInputCount.TabIndex = 0;
            this.lblDataInputCount.Text = "Data input count:";
            // 
            // txtdatainputcount
            // 
            this.txtdatainputcount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtdatainputcount.Location = new System.Drawing.Point(522, 15);
            this.txtdatainputcount.Name = "txtdatainputcount";
            this.txtdatainputcount.Size = new System.Drawing.Size(279, 22);
            this.txtdatainputcount.TabIndex = 8;
            // 
            // txtdataoutputcount
            // 
            this.txtdataoutputcount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtdataoutputcount.Location = new System.Drawing.Point(522, 43);
            this.txtdataoutputcount.Name = "txtdataoutputcount";
            this.txtdataoutputcount.Size = new System.Drawing.Size(279, 22);
            this.txtdataoutputcount.TabIndex = 10;
            // 
            // lblDataoutputcount
            // 
            this.lblDataoutputcount.AutoSize = true;
            this.lblDataoutputcount.Location = new System.Drawing.Point(401, 45);
            this.lblDataoutputcount.Name = "lblDataoutputcount";
            this.lblDataoutputcount.Size = new System.Drawing.Size(115, 16);
            this.lblDataoutputcount.TabIndex = 9;
            this.lblDataoutputcount.Text = "Data output count:";
            // 
            // txtSheetoutputcount
            // 
            this.txtSheetoutputcount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSheetoutputcount.Enabled = false;
            this.txtSheetoutputcount.Location = new System.Drawing.Point(522, 99);
            this.txtSheetoutputcount.Name = "txtSheetoutputcount";
            this.txtSheetoutputcount.Size = new System.Drawing.Size(279, 22);
            this.txtSheetoutputcount.TabIndex = 14;
            // 
            // lblSheetoutputcount
            // 
            this.lblSheetoutputcount.AutoSize = true;
            this.lblSheetoutputcount.Location = new System.Drawing.Point(401, 101);
            this.lblSheetoutputcount.Name = "lblSheetoutputcount";
            this.lblSheetoutputcount.Size = new System.Drawing.Size(122, 16);
            this.lblSheetoutputcount.TabIndex = 13;
            this.lblSheetoutputcount.Text = "Sheet output count:";
            // 
            // txtSheetinputcount
            // 
            this.txtSheetinputcount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSheetinputcount.Enabled = false;
            this.txtSheetinputcount.Location = new System.Drawing.Point(522, 71);
            this.txtSheetinputcount.Name = "txtSheetinputcount";
            this.txtSheetinputcount.Size = new System.Drawing.Size(279, 22);
            this.txtSheetinputcount.TabIndex = 12;
            // 
            // lblSheetinputcount
            // 
            this.lblSheetinputcount.AutoSize = true;
            this.lblSheetinputcount.Location = new System.Drawing.Point(401, 73);
            this.lblSheetinputcount.Name = "lblSheetinputcount";
            this.lblSheetinputcount.Size = new System.Drawing.Size(114, 16);
            this.lblSheetinputcount.TabIndex = 11;
            this.lblSheetinputcount.Text = "Sheet input count:";
            // 
            // chkindexsheet
            // 
            this.chkindexsheet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkindexsheet.AutoSize = true;
            this.chkindexsheet.Location = new System.Drawing.Point(803, 17);
            this.chkindexsheet.Name = "chkindexsheet";
            this.chkindexsheet.Size = new System.Drawing.Size(199, 20);
            this.chkindexsheet.TabIndex = 15;
            this.chkindexsheet.Text = "Index sheet names from zero ";
            this.chkindexsheet.UseVisualStyleBackColor = true;
            // 
            // chkpreprocess
            // 
            this.chkpreprocess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkpreprocess.AutoSize = true;
            this.chkpreprocess.Location = new System.Drawing.Point(803, 45);
            this.chkpreprocess.Name = "chkpreprocess";
            this.chkpreprocess.Size = new System.Drawing.Size(200, 20);
            this.chkpreprocess.TabIndex = 16;
            this.chkpreprocess.Text = "Preprocess Inputs on demand";
            this.chkpreprocess.UseVisualStyleBackColor = true;
            // 
            // tblLayoutPane1
            // 
            this.tblLayoutPane1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tblLayoutPane1.ColumnCount = 1;
            this.tblLayoutPane1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblLayoutPane1.Controls.Add(this.grOutput, 0, 3);
            this.tblLayoutPane1.Controls.Add(this.lblDataOutput, 0, 2);
            this.tblLayoutPane1.Controls.Add(this.grInput, 0, 1);
            this.tblLayoutPane1.Controls.Add(this.lblDataInput, 0, 0);
            this.tblLayoutPane1.Location = new System.Drawing.Point(3, 3);
            this.tblLayoutPane1.Name = "tblLayoutPane1";
            this.tblLayoutPane1.RowCount = 4;
            this.tblLayoutPane1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.704641F));
            this.tblLayoutPane1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90.29536F));
            this.tblLayoutPane1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tblLayoutPane1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 443F));
            this.tblLayoutPane1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblLayoutPane1.Size = new System.Drawing.Size(392, 654);
            this.tblLayoutPane1.TabIndex = 2;
            // 
            // txtSample
            // 
            this.txtSample.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSample.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSample.Location = new System.Drawing.Point(6, 32);
            this.txtSample.Name = "txtSample";
            this.txtSample.ReadOnly = true;
            this.txtSample.Size = new System.Drawing.Size(460, 460);
            this.txtSample.TabIndex = 17;
            this.txtSample.Text = ";";
            // 
            // lblScript
            // 
            this.lblScript.AutoSize = true;
            this.lblScript.Location = new System.Drawing.Point(401, 127);
            this.lblScript.Name = "lblScript";
            this.lblScript.Size = new System.Drawing.Size(46, 16);
            this.lblScript.TabIndex = 18;
            this.lblScript.Text = "Script:";
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOk.Location = new System.Drawing.Point(522, 704);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(101, 23);
            this.btnOk.TabIndex = 19;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.Location = new System.Drawing.Point(629, 704);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(101, 23);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnFindError
            // 
            this.btnFindError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFindError.Location = new System.Drawing.Point(731, 662);
            this.btnFindError.Name = "btnFindError";
            this.btnFindError.Size = new System.Drawing.Size(101, 23);
            this.btnFindError.TabIndex = 20;
            this.btnFindError.Text = "Find Error";
            this.btnFindError.UseVisualStyleBackColor = true;
            this.btnFindError.Click += new System.EventHandler(this.btnFindError_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabSample);
            this.tabControl1.Controls.Add(this.tabScript);
            this.tabControl1.Location = new System.Drawing.Point(522, 127);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(480, 527);
            this.tabControl1.TabIndex = 21;
            // 
            // tabSample
            // 
            this.tabSample.Controls.Add(this.btnCopy);
            this.tabSample.Controls.Add(this.txtSample);
            this.tabSample.Location = new System.Drawing.Point(4, 25);
            this.tabSample.Name = "tabSample";
            this.tabSample.Padding = new System.Windows.Forms.Padding(3);
            this.tabSample.Size = new System.Drawing.Size(472, 498);
            this.tabSample.TabIndex = 0;
            this.tabSample.Text = "Sample";
            this.tabSample.UseVisualStyleBackColor = true;
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopy.Location = new System.Drawing.Point(365, 3);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(101, 23);
            this.btnCopy.TabIndex = 21;
            this.btnCopy.Text = "Copy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // tabScript
            // 
            this.tabScript.Controls.Add(this.btnPaste);
            this.tabScript.Controls.Add(this.txtScript);
            this.tabScript.Location = new System.Drawing.Point(4, 25);
            this.tabScript.Name = "tabScript";
            this.tabScript.Padding = new System.Windows.Forms.Padding(3);
            this.tabScript.Size = new System.Drawing.Size(472, 498);
            this.tabScript.TabIndex = 1;
            this.tabScript.Text = "Script";
            this.tabScript.UseVisualStyleBackColor = true;
            // 
            // txtScript
            // 
            this.txtScript.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtScript.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtScript.EnableAutoDragDrop = true;
            this.txtScript.Location = new System.Drawing.Point(6, 32);
            this.txtScript.Name = "txtScript";
            this.txtScript.Size = new System.Drawing.Size(460, 462);
            this.txtScript.TabIndex = 18;
            this.txtScript.Text = "";
            this.txtScript.WordWrap = false;
            // 
            // txtCompileStatus
            // 
            this.txtCompileStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCompileStatus.Location = new System.Drawing.Point(838, 660);
            this.txtCompileStatus.Name = "txtCompileStatus";
            this.txtCompileStatus.Size = new System.Drawing.Size(160, 70);
            this.txtCompileStatus.TabIndex = 22;
            this.txtCompileStatus.Text = "";
            // 
            // lblCompileStatus
            // 
            this.lblCompileStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCompileStatus.AutoSize = true;
            this.lblCompileStatus.Location = new System.Drawing.Point(731, 688);
            this.lblCompileStatus.Name = "lblCompileStatus";
            this.lblCompileStatus.Size = new System.Drawing.Size(101, 16);
            this.lblCompileStatus.TabIndex = 23;
            this.lblCompileStatus.Text = "Compile Status:";
            // 
            // btnPaste
            // 
            this.btnPaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPaste.Location = new System.Drawing.Point(365, 3);
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(101, 23);
            this.btnPaste.TabIndex = 22;
            this.btnPaste.Text = "Paste";
            this.btnPaste.UseVisualStyleBackColor = true;
            this.btnPaste.Click += new System.EventHandler(this.btnPaste_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1005, 735);
            this.Controls.Add(this.lblCompileStatus);
            this.Controls.Add(this.txtCompileStatus);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnFindError);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lblScript);
            this.Controls.Add(this.chkpreprocess);
            this.Controls.Add(this.chkindexsheet);
            this.Controls.Add(this.txtSheetoutputcount);
            this.Controls.Add(this.lblSheetoutputcount);
            this.Controls.Add(this.txtSheetinputcount);
            this.Controls.Add(this.lblSheetinputcount);
            this.Controls.Add(this.txtdataoutputcount);
            this.Controls.Add(this.lblDataoutputcount);
            this.Controls.Add(this.txtdatainputcount);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.lblDataInputCount);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.tblLayoutPane1);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Scripting App";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grOutput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtdatainputcount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtdataoutputcount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSheetoutputcount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSheetinputcount)).EndInit();
            this.tblLayoutPane1.ResumeLayout(false);
            this.tblLayoutPane1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabSample.ResumeLayout(false);
            this.tabScript.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDataInput;
        private AdvancedDataGridView.TreeGridView grInput;
        private System.Windows.Forms.Label lblDataOutput;
        private AdvancedDataGridView.TreeGridColumn Structure;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private AdvancedDataGridView.TreeGridView grOutput;
        private AdvancedDataGridView.TreeGridColumn treeGridColumn1;
        private System.Windows.Forms.DataGridViewComboBoxColumn dataGridViewComboBoxColumn1;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblDataInputCount;
        private System.Windows.Forms.NumericUpDown txtdatainputcount;
        private System.Windows.Forms.NumericUpDown txtdataoutputcount;
        private System.Windows.Forms.Label lblDataoutputcount;
        private System.Windows.Forms.NumericUpDown txtSheetoutputcount;
        private System.Windows.Forms.Label lblSheetoutputcount;
        private System.Windows.Forms.NumericUpDown txtSheetinputcount;
        private System.Windows.Forms.Label lblSheetinputcount;
        private System.Windows.Forms.CheckBox chkindexsheet;
        private System.Windows.Forms.CheckBox chkpreprocess;
        private System.Windows.Forms.TableLayoutPanel tblLayoutPane1;
        private System.Windows.Forms.RichTextBox txtSample;
        private System.Windows.Forms.Label lblScript;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnFindError;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabSample;
        private System.Windows.Forms.TabPage tabScript;
        private System.Windows.Forms.RichTextBox txtScript;
        private System.Windows.Forms.RichTextBox txtCompileStatus;
        private System.Windows.Forms.Label lblCompileStatus;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnPaste;
    }
}

