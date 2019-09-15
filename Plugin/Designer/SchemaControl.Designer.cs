using AdvancedDataGridView;
using System.Xml.Linq;

namespace Designer
{
    partial class SchemaControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.treeGridView1 = new AdvancedDataGridView.TreeGridView();
            this.Structure = new AdvancedDataGridView.TreeGridColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.treeGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // treeGridView1
            // 
            this.treeGridView1.AllowUserToAddRows = false;
            this.treeGridView1.AllowUserToDeleteRows = false;
            this.treeGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.treeGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.treeGridView1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(245)))), ((int)(((byte)(253)))));
            this.treeGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Structure,
            this.Type,
            this.Value});
            this.treeGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.treeGridView1.ImageList = null;
            this.treeGridView1.Location = new System.Drawing.Point(0, 0);
            this.treeGridView1.Margin = new System.Windows.Forms.Padding(0);
            this.treeGridView1.Name = "treeGridView1";
            this.treeGridView1.Size = new System.Drawing.Size(289, 96);
            this.treeGridView1.TabIndex = 0;
            // 
            // Structure
            // 
            this.Structure.DefaultNodeImage = null;
            this.Structure.FillWeight = 112.1495F;
            this.Structure.HeaderText = "Structure";
            this.Structure.Name = "Structure";
            this.Structure.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Type
            // 
            this.Type.HeaderText = "Type";
            this.Type.Name = "Type";
            this.Type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Value
            // 
            this.Value.HeaderText = "Value";
            this.Value.Name = "Value";
            this.Value.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // SchemaControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeGridView1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "SchemaControl";
            this.Size = new System.Drawing.Size(289, 96);
            ((System.ComponentModel.ISupportInitialize)(this.treeGridView1)).EndInit();
            this.ResumeLayout(false);

        }

		#endregion

		private AdvancedDataGridView.TreeGridView treeGridView1;
		private TreeGridColumn Structure;
		private System.Windows.Forms.DataGridViewTextBoxColumn Type;
		private System.Windows.Forms.DataGridViewTextBoxColumn Value;
	}
}
