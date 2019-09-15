using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualBasic.FileIO;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using Microsoft.Win32;
using dBASE.NET;
using System.Diagnostics;


namespace CSVRealese
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowClass : Window
    {
        private string FileTypeSelected = "";
        private string FileEncoding = "";
        DataTable TempTable = new DataTable();
        private int paging_PageIndex = 1;
        private int paging_NoOfRecPerPage = 200;
        private int OldPageNumber = 0;
        private enum PagingMode { Next = 2, Previous = 3, First = 1, Last = 4 };

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Message, int wParam, int lParam);

        public const int mouseDown = 0xa1;
        public const int Caption = 0x2;

        private String file_path = "";
        private String file_extension = "";
        //List<MetaDataClass> csv_metadata = new List<MetaDataClass>();
        List<MetaDataClass> Reader_metadata = new List<MetaDataClass>();

        List<GirdColumnClass> Reader_metadataGrd = new List<GirdColumnClass>();

        //public MetaDataClass metaData;
        //public DataTable dataTable;
        public List<MetaDataClass> metaDataArray = new List<MetaDataClass>();

        public List<GirdColumnClass> metaDataArrayGrid = new List<GirdColumnClass>();


        public String txtFileName = null;
        public Encoding encode;
        public MetaDataClass metaData = new MetaDataClass();
        public GirdColumnClass metaDataGrd= new GirdColumnClass();
        public List<MetaDataClass> metaList = new List<MetaDataClass>();
        public DataTable ParseTable = new DataTable();
        public DataTable dataTable = new DataTable();
        public int colNumber = 1;
        public string strHeader = null;
        public string[] strHeaders;
        public List<string[]> strRows = new List<string[]>();
        public List<string> strLines = new List<string>();
        public int[] colLengthList;
        public int[] colStartPosList;
        public string[] rowData;
        public bool bChangeFlag = false;  // change column number flag ' no other 
        public int rowLength = 0;


        public Dbf dbf = new Dbf();

        public MainWindowClass()
        {
            InitializeComponent();
        }

        private void BtnFileBrower_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            switch (CmbFileType.SelectedIndex)
            {
                case 0:   // CSV
                    openFileDlg.Filter = "CSV files (*.csv)|*.csv";
                    break;
                case 1:   // DBF
                    openFileDlg.Filter = "DBF files (*.dbf)|*.dbf";
                    break;
                case 2:   // Text
                    openFileDlg.Filter = "Text files (*.txt)|*.txt";
                    break;
                case 3:   // ALL
                    openFileDlg.Filter = "All files (*.*)|*.*";
                    break;

            }
            
            //openFileDlg.Filter = "CSV files (*.csv)|*.csv|DBF files (*.dbf)|*.dbf|All files (*.*)|*.*";

            // Launch OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = openFileDlg.ShowDialog();

            // Get the selected file name and display in a TextBox.
            // Load content of file in a TextBlock
            if (result == true)
            {
                file_path = txtInputFile.Text = openFileDlg.FileName;
                file_extension = System.IO.Path.GetExtension(file_path);
                //GetCVSData(openFileDlg.FileName);
                
            }
        }

        /* csv to DataTable */
        private DataTable DataTabletFromCSVFile(string csv_file_path, Encoding encode = null)
        {
            ParseTable.Clear();
            ParseTable.Columns.Clear();
            //DataTable csvData = new DataTable();
            try
            {
                string[] FileColumns = null;
                string Delimiter = "";
                using (TextFieldParser csvReaderCount = new TextFieldParser(csv_file_path, encode))
                {
                    Delimiter = txtDelimiter.Text;
                    if (string.IsNullOrEmpty(Delimiter))
                    {
                        MessageBox.Show("Invalid Delimiter");

                        return ParseTable;
                    }
                    csvReaderCount.SetDelimiters(new string[] { Delimiter });
                    csvReaderCount.HasFieldsEnclosedInQuotes = true;
                    FileColumns = csvReaderCount.ReadFields();
                }
                using (TextFieldParser csvReader = new TextFieldParser(csv_file_path, encode))
                {
                    csvReader.SetDelimiters(new string[] { Delimiter });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    if (HeadercheckBox.IsChecked == true)
                    {
                        string[] colFields = csvReader.ReadFields();
                    //List<string> metaDataList = new List<string>();
                  
                        foreach (string column in colFields)
                        {


                            DataColumn dataColumn = new DataColumn(column);
                            dataColumn.AllowDBNull = true;
                            ParseTable.Columns.Add(dataColumn);

                            /* Make csv file metaData to ArrayList */
                            metaData = new MetaDataClass();
                            metaData.Name = column;
                            metaData.OpenFileType = FileTypeSelected;
                            metaData.RootArrayName = txtRootArrayName.Text;
                            metaData.InputFile = txtInputFile.Text;
                            if (file_extension != ".dbf")
                            {
                                metaData.FileEncoding = FileEncoding;
                            }
                                metaDataArray.Add(metaData);
                        }
                    }
                    else
                    {
                        if (dataGridColumns != null)
                        {

                            metaDataArray.Clear();
                            foreach (System.Data.DataRowView dr in dataGridColumns.ItemsSource)
                            {
                                string IsNotNull = dr[0].ToString();
                                if (!string.IsNullOrEmpty(IsNotNull))
                                {
                                    DataColumn dataColumn = new DataColumn(dr[0].ToString());
                                    dataColumn.AllowDBNull = true;
                                    ParseTable.Columns.Add(dataColumn);

                                    metaData = new MetaDataClass();
                                    metaData.Name = dr[0].ToString();
                                    metaData.OpenFileType = FileTypeSelected;
                                    metaData.RootArrayName = txtRootArrayName.Text;
                                    metaData.InputFile = txtInputFile.Text;
                                    if (file_extension != ".dbf")
                                    {
                                        metaData.FileEncoding = FileEncoding;
                                    }
                                    metaDataArray.Add(metaData);
                                }
                            }

                            
                            if(FileColumns.Length != ParseTable.Columns.Count)
                            {
                                MessageBox.Show("Columns length not matched with csv file..!");
                                return ParseTable;
                            }
                        }
                    }

                    Reader_metadata = metaDataArray;
                    int RowCount = 0;
                    while (!csvReader.EndOfData)
                    {
                       if(RowCount == paging_NoOfRecPerPage)
                        {
                            break;
                        }
                        string[] fieldData = csvReader.ReadFields();
                       // string[] fieldDataStr1 = csvReader.ReadFields();
                       // string fieldDataStr = string.Join(",",fieldDataStr1);
                       //// string[] fieldData = fieldDataStr.Split(',');
                       // string[] fieldData = Regex.Split(fieldDataStr, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                        //Making empty value as null
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }
                        ParseTable.Rows.Add(fieldData);
                        RowCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                if(ex.Message == "Input array is longer than the number of columns in this table.")
                {
                    MessageBox.Show("Invalid Delimiter");
                    return ParseTable;
                }
                //  MessageBox.Show(ex + "\n DataTabletFromCSVFile Exception Error!");
                MessageBox.Show(ex.Message);
            }
            return ParseTable;
        }


        /* dbf to DataTable */
        public DataTable DataTabletFromDbfFile( string dbf_file_path, Encoding encode = null)
        {
           
            //DataTable dbfData = new DataTable();
            ParseTable.Clear();
            ParseTable.Columns.Clear();
            try
            {
                dbf.Read(dbf_file_path);
                
                for (int i = 0; i < dbf.Fields.Count; i++)
                {
                    DataColumn dataColumn = new DataColumn(dbf.Fields[i].Name);
                    dataColumn.AllowDBNull = true;
                    ParseTable.Columns.Add(dbf.Fields[i].Name);

                    metaData = new MetaDataClass();
                    metaData.Name = dbf.Fields[i].Name;
                    metaData.OpenFileType = FileTypeSelected;
                    metaData.RootArrayName = txtRootArrayName.Text;
                    metaData.InputFile = txtInputFile.Text;
                    if (file_extension != ".dbf")
                    {
                        metaData.FileEncoding = FileEncoding;
                    }
                    metaDataArray.Add(metaData);
                }

                Reader_metadata = metaDataArray;
                int RowCount = 0;
                for (int i = 0; i < dbf.Records.Count; i++)
                {
                    if (RowCount == paging_NoOfRecPerPage)
                    {
                        break;
                    }
                    string[] cellArray = new string[dbf.Records[i].Data.Count];
                    for (int j = 0; j < dbf.Records[i].Data.Count; j++)
                    {
                        string cell = null;
                        if (dbf.Records[i].Data[j] == null)
                        {
                            cell = "null";
                        }
                        else
                        {
                            cell = dbf.Records[i].Data[j].ToString();
                        }

                        cellArray[j] = cell;
                    }
                    ParseTable.Rows.Add(cellArray);
                    RowCount++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex + "\n Exception Error !");
            }

            return ParseTable;
        }

        /* Text file to DataTable */
        private DataTable DataTabeletFromTxtFile(string txt_file_path, Encoding encode = null)
        {

            DataTable TxtData  = new DataTable();
            //ParserWindow parserWin = new ParserWindow();
            //parserWin.txtFileName = txt_file_path;
            //parserWin.encode = encode;
            //var result = parserWin.ShowDialog();

            //if(result == true)
            //{               
                TxtData = TempTable;

                //dataGrid.ItemsSource = TxtData.DefaultView;
                metaDataArray.Clear();

                for(int col = 0; col < strHeaders.Count(); col ++)
                {
                    metaData = new MetaDataClass();
                    metaData.Name = strHeaders[col];
                    metaData.StartPos = colStartPosList[col];
                    metaData.Length = colLengthList[col];
                    metaData.OpenFileType = FileTypeSelected;
                    metaData.RootArrayName = txtRootArrayName.Text;
                    metaData.InputFile = txtInputFile.Text;
                    if (file_extension != ".dbf")
                    {
                    metaData.FileEncoding = FileEncoding;
                    }
                metaDataArray.Add(metaData);
                }

                Reader_metadata = metaDataArray;
            //}

            return TxtData;
        }

        /* Get Preview Button Click Event */
        private void CVSViewBtn_Click(object sender, RoutedEventArgs e)
        {
            int k = cbxEncoding.SelectedIndex;
            encode = null;
            
            switch (k) {
                case 0:
                    encode = Encoding.UTF8;
                    break;
                case 1:
                    encode = Encoding.Unicode;
                    break;
                case 2:
                    encode = Encoding.UTF32;
                    break;
                case 3:
                    encode = Encoding.ASCII;
                    break;
                default:
                    encode = null;
                    break;
            }

            if ( file_extension == ".dbf" )
            {
                dataTable = DataTabletFromDbfFile(file_path, encode);
                ListProducts(0);
                // dataGrid.ItemsSource = dataTable.DefaultView;
            }
            else if ( file_extension == ".csv" )
            {
                dataTable = DataTabletFromCSVFile(file_path, encode);
                ListProducts(0); 
                //dataGrid.ItemsSource = dataTable.DefaultView;
            } else if ( file_extension == ".txt" )
            {
                //dataTable.Clear();
                PreviewTextFile(file_path, encode);  // Preview File Contents
                TabItem_ContextMenuOpening(null, null);
                DataTabeletFromTxtFile(file_path, encode);                
                //dataGrid.ItemsSource = dataTable.DefaultView;
            } else
            {
                MessageBox.Show("Please Choose the Correct file like '.csv' or '.dbf'");
            }
        }
        public void addCombo()
        {
            //ADD COLUMNS
            DataGridComboBoxColumn ss = new DataGridComboBoxColumn();
            ss.Header = "DataType";
            // ss.Name
            List<string> list = new List<string>();
            list.Add("Item 1");
            list.Add("Item 2");
            // ss.res = new Binding(list);
            ss.ItemsSource = list;
            ss.SelectedItemBinding = new Binding("test");
            ss.Width = 50;
            //ss.EditingElementStyle = style;

            //dataGridColumns.Columns.Add(ss);
            dataGridColumns.Columns.Insert(1, ss);

        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataGridHidden.Visibility = Visibility.Hidden;
            btnFirst.Visibility = Visibility.Hidden;
            btnPrev.Visibility = Visibility.Hidden;
            lblPageNumber.Visibility = Visibility.Hidden;
            btnNext.Visibility = Visibility.Hidden;
            btnLast.Visibility = Visibility.Hidden;
            lblPagingInfo.Visibility = Visibility.Hidden;

            // DataTable mdt = new DataTable();
            // mdt.Columns.Add("Name");
            //// mdt.Columns.Add("DataType");
            // //mdt.Columns.Add("strFormat");
            // mdt.Columns.Add("StartPos");
            // mdt.Columns.Add("Length");
            //// mdt.Columns.Add("Precision");
            // mdt.Columns.Add("TrimSpaces");
            // dataGridColumns.ItemsSource = mdt.DefaultView;
            //addCombo();
            metaDataArrayGrid.Clear();

            metaDataGrd = new GirdColumnClass();
            metaDataArrayGrid.Add(metaDataGrd);
            Reader_metadataGrd = metaDataArrayGrid;

            Reader_metadataGrd = metaDataArrayGrid;

            DataTable mdt = ConvertArrayListToDataTable(new ArrayList(Reader_metadataGrd));
            dataGridColumns.ItemsSource = mdt.DefaultView;
            foreach (var column in dataGridColumns.Columns)
            {
                
                column.MinWidth = column.ActualWidth;
                column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                //column.HeaderStyle = Color.FromRgb()
            }
            
        }

        //public class AddItems
        //{
        //    public string strColumnName { get; set; }
        //    public string eDataType { get; set; }
        //    public string strFormat { get; set; }
        //    public string iStartPos { get; set; }
        //    public string iLength { get; set; }
        //    public string Precision { get; set; }
        //    public string eTrimType { get; set; }

        //}

        public void PreviewTextFile(String fileName, Encoding encode)
        {
            //DataTable TempTable = new DataTable();
            ParseTable.Clear();
            ParseTable.Columns.Clear();
            using (StreamReader txtReader = new StreamReader(fileName, encode))
            {
                string sLine = "";
                //TempTable.reset();
                //ArrayList arrText = new ArrayList();
                //sLine = txtReader.ReadLine();
                sLine = GetHeader(fileName, encode);
                string[] lines = File.ReadAllLines(fileName);
                int cou = 0;
                while (txtReader.ReadLine() != sLine)
                {
                    if (lines.Length <= cou)
                    {
                        break;
                    }
                    txtReader.ReadLine();
                    cou++;
                }
                strHeader = sLine;
                sLine = sLine.Replace(".", "");

                //sLine = sLine.Replace(" ", "");
                //TempTable.Columns.Add("dfas df");
                ParseTable.Columns.Add(sLine.Trim());
                int RowCount = 0;
                while (sLine != null)
                {
                    if (RowCount == paging_NoOfRecPerPage)
                    {
                        break;
                    }
                    sLine = txtReader.ReadLine();
                    ParseTable.Rows.Add(sLine);
                    //TempTable.Rows.Add(sLine);
                    //arrText.Add(sLine);
                    RowCount++;
                }

                txtReader.Close();
                // dataGrid.ItemsSource = TempTable.DefaultView;
                ListProducts(0);
            }
        }

        public string GetHeader(String fileName, Encoding encode)
        {
            string HeaderLine = "";
            string[] lines = File.ReadAllLines(fileName);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("------"))
                {
                    
                   return HeaderLine = lines[i - 1];
                }
            }
            return HeaderLine;
        }

        /* Get Data Field Button Click Event */
        private void GetMetaDataFieldBtn_Click(object sender, RoutedEventArgs e)
        {
            if ( file_extension == ".csv")
            {
                DataTable mdt = ConvertArrayListToDataTable(new ArrayList(Reader_metadata));
                metaDataGrid.ItemsSource = mdt.DefaultView;
            } else if ( file_extension == ".dbf")
            {
                DataTable mdt = ConvertArrayListToDataTable(new ArrayList(Reader_metadata));
                metaDataGrid.ItemsSource = mdt.DefaultView;
            } else if ( file_extension == ".txt" )
            {
                DataTable mdt = ConvertArrayListToDataTable(new ArrayList(Reader_metadata));
                metaDataGrid.ItemsSource = mdt.DefaultView;                
            }
            else
            {
                MessageBox.Show("Please Choose the a file like '*.csv',  '*.dbf' or PlannText file '*.txt' ");

            }
        }


        private void LoadDBF(string Filepath,Encoding enc)
        {
            

           // DataTable TempTable = new DataTable();
           // DataTable LoadedFile = new DataTable();
           // using (StreamReader txtReader = new StreamReader(file_path, encode))
            //{
                string sLine = "";
                ParseTable.Clear();
                ParseTable.Columns.Clear();
                bool HasRows = false;
                if (dataGridColumns != null)
                {

                    metaDataArray.Clear();
                    foreach (System.Data.DataRowView dr in dataGridColumns.ItemsSource)
                    {
                        string IsNotNull = dr[0].ToString();
                        if (!string.IsNullOrEmpty(IsNotNull))
                        {
                            ParseTable.Columns.Add(dr[0].ToString());
                            HasRows = true;
                            // add in array to parse
                            string start = dr[2].ToString();
                            string length = dr[3].ToString();
                            if (!string.IsNullOrEmpty(IsNotNull) && !string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(length))
                            {
                                int startIndex = Convert.ToInt32(dr[2]);
                                int Length = Convert.ToInt32(dr[3]);
                                metaData = new MetaDataClass();
                                metaData.Name = dr[0].ToString();
                                metaData.StartPos = startIndex;
                                metaData.Length = Length;
                                metaData.OpenFileType = FileTypeSelected;
                                metaData.RootArrayName = txtRootArrayName.Text;
                                metaData.InputFile = txtInputFile.Text;
                                if (file_extension != ".dbf")
                                {
                                metaData.FileEncoding = FileEncoding;
                                }
                            metaDataArray.Add(metaData);
                            }
                            //MessageBox.Show(dr[0].ToString());
                    }
                    }
                }

                if (HasRows)
                {
                dataTable = DataTabletFromDbfFileLength(Filepath, enc);
                int RowCount = 0;
                for (int line = 0; line<dataTable.Rows.Count; line++)//while (sLine != null)
                    {
                    if(RowCount == paging_NoOfRecPerPage)
                    {
                        break;
                    }
                        sLine = dataTable.Rows[line][0].ToString();//txtReader.ReadLine();
                        DataRow rowData = ParseTable.NewRow();
                        
                        if (sLine != null)
                        {
                            bool addRow = false;
                            foreach (System.Data.DataRowView dr in dataGridColumns.ItemsSource)
                            {
                                string col = dr[0].ToString();
                                string start = dr[2].ToString();
                                string length = dr[3].ToString();
                                if (!string.IsNullOrEmpty(col) && !string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(length))
                                {
                                    if (IsDigitsOnly(dr[2].ToString()) == true && IsDigitsOnly(dr[3].ToString()) == true)
                                    {
                                        int startIndex = Convert.ToInt32(dr[2]);
                                        int Length = Convert.ToInt32(dr[3]);
                                        int LineLength = sLine.Length;
                                        int Total = LineLength - startIndex;
                                    if (LineLength < Length)
                                    {
                                        Length = LineLength;
                                    }
                                   if (Total < Length)
                                    {
                                        if (Total > 0)
                                        {
                                            Length = Total;
                                        }
                                        else
                                        {
                                            Length = LineLength;
                                        }
                                    }
                                    if (startIndex >= LineLength)
                                    {
                                        startIndex = 0;
                                    }
                                    //  if (startIndex < Length)
                                    //  {
                                    string sub = sLine.Substring(startIndex, Length);

                                            rowData[dr[0].ToString()] = sub;
                                            addRow = true;
                                     //   }

                                    }
                                }
                            }
                            if (addRow)
                            {
                            ParseTable.Rows.Add(rowData);
                           
                            }
                        }
                    RowCount++;
                }

                    //txtReader.Close();
                    //dataGrid.ItemsSource = TempTable.DefaultView;
                    ListProducts(0);

                }
                else
                {
                    CVSViewBtn_Click(null, null);
                }
            //}


            //dataGrid.ItemsSource = dataTable.DefaultView;
        }
        public DataTable DataTabletFromDbfFileLength(string dbf_file_path, Encoding encode = null)
        {

            DataTable dbfData = new DataTable();
            try
            {
                dbf.Read(dbf_file_path);
                dbfData.Columns.Add("");
                //for (int i = 0; i < dbf.Fields.Count; i++)
                //{
                //    DataColumn dataColumn = new DataColumn(dbf.Fields[i].Name);
                //    dataColumn.AllowDBNull = true;
                //   // dbfData.Columns.Add(dbf.Fields[i].Name);

                //    metaData = new MetaDataClass();
                //    metaData.strColumnName = dbf.Fields[i].Name;
                //    metaDataArray.Add(metaData);
                //}

                Reader_metadata = metaDataArray;

                for (int i = 0; i < dbf.Records.Count; i++)
                {
                    //string[] cellArray = new string[dbf.Records[i].Data.Count];
                    string str = "";
                    for (int j = 0; j < dbf.Records[i].Data.Count; j++)
                    {
                        string cell = null;
                        if (dbf.Records[i].Data[j] == null)
                        {
                            cell = "null";
                        }
                        else
                        {
                            cell = dbf.Records[i].Data[j].ToString();
                        }
                        str = string.Concat(str, cell);
                        // cellArray[j] = cell;
                    }
                    dbfData.Rows.Add(str);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex + "\n Exception Error !");
            }

            return dbfData;
        }

        private void ApplyEdit()
        {
            using (StreamReader txtReader = new StreamReader(file_path, encode))
            {
                string sLine = "";
                ParseTable.Clear();
                ParseTable.Columns.Clear();
                bool HasRows = false;
                if (dataGridColumns != null)
                {
                    metaDataArray.Clear();
                    foreach (System.Data.DataRowView dr in dataGridColumns.ItemsSource)
                    {
                        string IsNotNull = dr[0].ToString();
                        if (!string.IsNullOrEmpty(IsNotNull))
                        {
                            ParseTable.Columns.Add(dr[0].ToString());
                            HasRows = true;
                            // add in array to parse
                            string DType = dr[1].ToString();
                            string start = dr[2].ToString();
                            string length = dr[3].ToString();
                            if (!string.IsNullOrEmpty(IsNotNull) && !string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(length))
                            {
                                if (IsDigitsOnly(dr[2].ToString()) == true && IsDigitsOnly(dr[3].ToString()) == true)
                                {

                                    int startIndex = Convert.ToInt32(dr[2]);
                                    int Length = Convert.ToInt32(dr[3]);
                                    metaData = new MetaDataClass();
                                    metaData.Name = dr[0].ToString();
                                    metaData.StartPos = startIndex;
                                    metaData.Length = Length;
                                    metaData.OpenFileType = FileTypeSelected;
                                    metaData.RootArrayName = txtRootArrayName.Text;
                                    metaData.InputFile = txtInputFile.Text;
                                    if (!string.IsNullOrEmpty(DType))
                                    {
                                        if (DType == "1")
                                        {
                                            DType = "String";
                                        }
                                        else if (DType == "2")
                                        {
                                            DType = "Date";
                                        }
                                        else if (DType == "3")
                                        {
                                            DType = "Boolean";
                                        }
                                        else if (DType == "4")
                                        {
                                            DType = "Integer";
                                        }
                                        else if (DType == "5")
                                        {
                                            DType = "LongInteger";
                                        }
                                        metaData.DataTypeStr = DType;
                                    }
                                    if (file_extension != ".dbf")
                                    {
                                        metaData.FileEncoding = FileEncoding;
                                    }
                                    metaDataArray.Add(metaData);
                                }
                            }
                            //MessageBox.Show(dr[0].ToString());
                        }
                    }
                }

                if (HasRows)
                {
                    int RowsCount = 0;

                    foreach (System.Data.DataRowView drLines in dataGrid.ItemsSource)
                    {

                        if (RowsCount == paging_NoOfRecPerPage)
                        {
                            break;
                        }
                        sLine = drLines[0].ToString();//txtReader.ReadLine();
                        sLine = sLine.Replace(" | ", "");
                        DataRow rowData = ParseTable.NewRow();
                        if (sLine != null)
                        {
                            bool addRow = false;
                            foreach (System.Data.DataRowView dr in dataGridColumns.ItemsSource)
                            {
                                string col = dr[0].ToString();
                                string start = dr[2].ToString();
                                string length = dr[3].ToString();
                                if (!string.IsNullOrEmpty(col) && !string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(length))
                                {
                                    if (IsDigitsOnly(dr[2].ToString()) == true && IsDigitsOnly(dr[3].ToString()) == true)
                                    {
                                        int startIndex = Convert.ToInt32(dr[2]);
                                        int Length = Convert.ToInt32(dr[3]);
                                        int LineLength = sLine.Length;
                                        int Total = LineLength - startIndex;
                                        if (LineLength < Length)
                                        {
                                            Length = LineLength;
                                        }
                                        if (Total < Length)
                                        {
                                            if (Total > 0)
                                            {
                                                Length = Total;
                                            }
                                            else
                                            {
                                                Length = LineLength;
                                            }
                                        }
                                        if (startIndex >= LineLength)
                                        {
                                            startIndex = 0;
                                        }
                                        //  if (startIndex < Length)
                                        //  {
                                        string sub = sLine.Substring(startIndex, Length);

                                        rowData[dr[0].ToString()] = sub;
                                        addRow = true;
                                        // }

                                    }
                                }
                            }
                            if (addRow)
                            {
                                ParseTable.Rows.Add(rowData);
                                RowsCount++;
                            }
                        }
                    }

                    txtReader.Close();
                    dataGridHidden.ItemsSource = ParseTable.DefaultView;
                    //ListProducts(0);
                }
               
            }
        }

        private void MetaDataSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(cbxEncoding.Text);

            if (File.Exists(file_path))
            {
                int k = cbxEncoding.SelectedIndex;
                encode = null;

                switch (k)
                {
                    case 0:
                        encode = Encoding.UTF8;
                        break;
                    case 1:
                        encode = Encoding.Unicode;
                        break;
                    case 2:
                        encode = Encoding.UTF32;
                        break;
                    case 3:
                        encode = Encoding.ASCII;
                        break;
                    default:
                        encode = null;
                        break;
                }
                if (file_extension == ".dbf")
                {
                    //LoadDBF(file_path, encode);
                    CVSViewBtn_Click(sender, e);
                    return;
                }
                if (file_extension == ".csv")
                {
                    CVSViewBtn_Click(sender, e);
                    return;
                }
               // CVSViewBtn_Click(sender, e);
               // return;
                //DataTable LoadedFile = new DataTable();
                using (StreamReader txtReader = new StreamReader(file_path, encode))
                {
                    string sLine = "";
                    ParseTable.Clear();
                    ParseTable.Columns.Clear();
                    bool HasRows = false;
                    if (dataGridColumns != null)
                    {
                        metaDataArray.Clear();
                        foreach (System.Data.DataRowView dr in dataGridColumns.ItemsSource)
                        {
                            string IsNotNull = dr[0].ToString();
                            if (!string.IsNullOrEmpty(IsNotNull))
                            {
                                ParseTable.Columns.Add(dr[0].ToString());
                                HasRows = true;
                                // add in array to parse
                                string DType = dr[1].ToString();
                                string start = dr[2].ToString();
                                string length = dr[3].ToString();
                                if (!string.IsNullOrEmpty(IsNotNull) && !string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(length))
                                {
                                    if (IsDigitsOnly(dr[2].ToString()) == true && IsDigitsOnly(dr[3].ToString()) == true)
                                    {

                                        int startIndex = Convert.ToInt32(dr[2]);
                                        int Length = Convert.ToInt32(dr[3]);
                                        metaData = new MetaDataClass();
                                        metaData.Name = dr[0].ToString();
                                        metaData.StartPos = startIndex;
                                        metaData.Length = Length;
                                        metaData.OpenFileType = FileTypeSelected;
                                        metaData.RootArrayName = txtRootArrayName.Text;
                                        metaData.InputFile = txtInputFile.Text;
                                        //if (IsDigitsOnly(DType))
                                        //{
                                        //    metaData.DataType = Enum.Parse(DType);
                                        //}
                                        if (!string.IsNullOrEmpty(DType))
                                        {
                                            if (DType == "1")
                                            {
                                                DType = "String";
                                            }
                                            else if (DType == "2")
                                            {
                                                DType = "Date";
                                            }
                                            else if (DType == "3")
                                            {
                                                DType = "Boolean";
                                            }
                                            else if (DType == "4")
                                            {
                                                DType = "Integer";
                                            }
                                            else if (DType == "5")
                                            {
                                                DType = "LongInteger";
                                            }
                                            metaData.DataTypeStr = DType;
                                        }
                                        if (file_extension != ".dbf")
                                        {
                                            metaData.FileEncoding = FileEncoding;
                                        }
                                        metaDataArray.Add(metaData);
                                    }
                                }
                                //MessageBox.Show(dr[0].ToString());
                            }
                        }
                    }

                    if (HasRows)
                    {
                        int RowsCount = 0;

                        while (sLine != null)
                        {

                            if (RowsCount == paging_NoOfRecPerPage)
                            {
                                break;
                            }
                            sLine = txtReader.ReadLine();
                            DataRow rowData = ParseTable.NewRow();
                            if (sLine != null)
                            {
                                bool addRow = false;
                                foreach (System.Data.DataRowView dr in dataGridColumns.ItemsSource)
                                {
                                    string col = dr[0].ToString();
                                    string start = dr[2].ToString();
                                    string length = dr[3].ToString();
                                    if (!string.IsNullOrEmpty(col) && !string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(length))
                                    {
                                        if (IsDigitsOnly(dr[2].ToString()) == true && IsDigitsOnly(dr[3].ToString()) == true)
                                        {
                                            int startIndex = Convert.ToInt32(dr[2]);
                                            int Length = Convert.ToInt32(dr[3]);
                                            int LineLength = sLine.Length;
                                            int Total = LineLength - startIndex;
                                            if(LineLength < Length)
                                            {
                                                Length = LineLength;
                                            }
                                            if (Total < Length)
                                            {
                                                if (Total > 0)
                                                {
                                                    Length = Total;
                                                }
                                                else
                                                {
                                                    Length = LineLength;
                                                }
                                            }
                                            if(startIndex >= LineLength)
                                            {
                                                startIndex = 0;
                                            }
                                          //  if (startIndex < Length)
                                          //  {
                                                string sub = sLine.Substring(startIndex, Length);

                                                rowData[dr[0].ToString()] = sub;
                                                addRow = true;
                                           // }

                                        }
                                    }
                                }
                                if (addRow)
                                {
                                    ParseTable.Rows.Add(rowData);
                                    RowsCount++;
                                }
                            }
                        }

                        dataGrid.GridLinesVisibility = DataGridGridLinesVisibility.All;
                        //dataGrid.GridLinesVisibility = DataGridGridLinesVisibility.Horizontal;
                        // dataGrid.VerticalGridLinesBrush = 
                        txtReader.Close();
                        //dataGridHidden.ItemsSource = ParseTable.DefaultView;
                        ListProducts(0);
                    }
                    else
                    {
                        CVSViewBtn_Click(sender,e);
                    }
                }
            }
            else
            {
                MessageBox.Show("Invalid File Path");
            }
        }


        bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        /* Get Parse to XML Schema Button Click Event */
        private void ParseToXMLBtn_Click(object sender, RoutedEventArgs e)
        {
            if (dataTable == null)
            {
                MessageBox.Show("There is no data to save.");
                return;
            }
            
            // Save Metadata as XML file
            XmlSerializer xs = new XmlSerializer(metaDataArray.GetType());

            using (FileStream fs = new FileStream("ReaderMeta.xml", FileMode.Create))
                xs.Serialize(fs, metaDataArray);
            
            // Save file data as XML file 
            dataTable.TableName = "dataList";
            dataTable.WriteXml(@"ReaderData.xml");
            MessageBox.Show("Success export into Data & MetaData!");
        }

        /* ArrayList to DataTable functions */
        public static DataTable ConvertArrayListToDataTable(ArrayList arrayList)
        {
            DataTable dt = new DataTable();

            if (arrayList.Count != 0)
            {
                dt = ConvertObjectToDataTableSchema(arrayList[0]);
               // FillData(arrayList, dt);
            }

            return dt;
        }

        public static DataTable ConvertObjectToDataTableSchema(Object o)
        {
            DataTable dt = new DataTable();
            PropertyInfo[] properties = o.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                DataColumn dc = new DataColumn(property.Name);
                dc.DataType = property.PropertyType; dt.Columns.Add(dc);
            }
            return dt;
        }

        private static void FillData(ArrayList arrayList, DataTable dt)
        {
            foreach (Object o in arrayList)
            {
                DataRow dr = dt.NewRow();
                PropertyInfo[] properties = o.GetType().GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    dr[property.Name] = property.GetValue(o, null);
                }
                dt.Rows.Add(dr);
            }
        }

        private static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }

        private void PrevBtn_Click(object sender, RoutedEventArgs e)
        {
           
                bChangeFlag = true;
                if (colNumber == 1)
                {
                    //LenTxt.Text = colLengthList[colNumber-1].ToString();
                    //LenSlider.Value = colLengthList[colNumber-1];
                }
                else if (colNumber < 1)
                {
                    return;
                }
                else
                {
                    colNumber = colNumber - 1;
                    //LenTxt.Text = (colLengthList[colNumber-1] - colLengthList[colNumber - 2]).ToString();
                    //LenSlider.Value = (colLengthList[colNumber-1] - colLengthList[colNumber - 2]);
                }
                ColTxt.Text = colNumber.ToString();
                colStartPosTxt.Text = colLengthList[colNumber - 1].ToString();

            // ParserGrid.ItemsSource = TempTable.DefaultView;
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            bChangeFlag = true;
            LenTxt.Text = "0";
            LenSlider.Value = 0;

            if (colNumber == strHeaders.Count())
            {
                //LenTxt.Text = (colLengthList[colNumber-1] - colLengthList[colNumber - 2]).ToString();
                //LenSlider.Value = (colLengthList[colNumber-1] - colLengthList[colNumber - 2]);
            }
            else if (colNumber < strHeaders.Count())
            {
                colNumber = colNumber + 1;
                
                //LenTxt.Text = colLengthList[colNumber-1].ToString();
                //LenSlider.Value = colLengthList[colNumber-1];
            }

            //ParserGrid.ItemsSource = TempTable.DefaultView;
            ColTxt.Text = colNumber.ToString();
            colStartPosTxt.Text = colLengthList[colNumber - 2].ToString();

        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            DataTable TxtData = new DataTable();
            TxtData = TempTable;

            //dataGrid.ItemsSource = TxtData.DefaultView;
            metaDataArray.Clear();

            for (int col = 0; col < strHeaders.Count(); col++)
            {
                metaData = new MetaDataClass();
                metaData.Name = strHeaders[col];
                metaData.Length = colLengthList[col];
                metaData.StartPos = colStartPosList[col];
                metaData.OpenFileType = FileTypeSelected;
                metaData.RootArrayName = txtRootArrayName.Text;
                metaData.InputFile = txtInputFile.Text;
                if (file_extension != ".dbf")
                {
                    metaData.FileEncoding = FileEncoding;
                }
                metaDataArray.Add(metaData);
            }

            Reader_metadata = metaDataArray;
        }

        private void LenSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (bChangeFlag == true)  // when change column number don't call this function
            {
                bChangeFlag = false;
                return;
            }

            LenTxt.Text = LenSlider.Value.ToString();
            //TempTable = new DataTable();

            int colPos = System.Convert.ToInt32(ColTxt.Text) - 1;

            rowData = new string[strHeaders.Count()];
            TempTable.Clear();
            for (int kk = 0; kk < strHeaders.Count(); kk++)
            {
                if (TempTable.Columns.Contains(strHeaders[kk]) == false)
                    TempTable.Columns.Add(strHeaders[kk]);
            }

            colLengthList[colPos] = System.Convert.ToInt32(LenSlider.Value); // + System.Convert.ToInt32(colStartPosTxt.Text);
            colStartPosList[colPos] = System.Convert.ToInt32(colStartPosTxt.Text);

            if (colPos > 0)
            {
                //colLengthList[colPos] = System.Convert.ToInt32(LenSlider.Value) + colLengthList[colPos - 1];

                colLengthList[colPos] = System.Convert.ToInt32(LenSlider.Value); // + System.Convert.ToInt32(colStartPosTxt.Text);
                colStartPosList[colPos] = System.Convert.ToInt32(colStartPosTxt.Text); 

                if (colLengthList[colPos] + colStartPosList[colPos] > rowLength && rowLength > 0)
                {
                    LenSlider.Value = rowLength - colLengthList[colPos - 1];
                    return;
                }
            }
            else
            {
                colLengthList[colPos] = System.Convert.ToInt32(LenSlider.Value); // + System.Convert.ToInt32(colStartPosTxt.Text);
                colStartPosList[colPos] = System.Convert.ToInt32(colStartPosTxt.Text);

                if (colLengthList[colPos] + colStartPosList[colPos] > rowLength && rowLength > 0)
                {
                    LenSlider.Value = rowLength - colLengthList[colPos];

                    return;
                }

                //colLengthList[colPos] = System.Convert.ToInt32(LenSlider.Value);
                //colLengthList[colPos] = System.Convert.ToInt32(colStartPosTxt.Text);// + System.Convert.ToInt32(LenSlider.Value);

            }


            for (int row = 0; row < strLines.Count(); row++)
            {
                if (strLines[row] == null)
                    return;

                rowLength = strLines[row].Length;

                for (int subCol = 0; subCol < (colPos + 1); subCol++)
                {

                    //if (subRow > 0)
                    //{
                    //    if (colLengthList[subRow] == 0)
                    //    {
                    //        //rowData[subRow] = strLines[row].Substring(colLengthList[subRow - 1], colLengthList[subRow - 1]);
                    //        rowData[subRow] = strLines[row].Substring(System.Convert.ToInt32(colStartPosTxt.Text), System.Convert.ToInt32(LenTxt.Text));
                    //    }
                    //    else
                    //    {
                    //        //rowData[subRow] = strLines[row].Substring(colLengthList[subRow - 1], colLengthList[subRow] - colLengthList[subRow - 1]);
                    //        rowData[subRow] = strLines[row].Substring(System.Convert.ToInt32(colStartPosTxt.Text), System.Convert.ToInt32(LenTxt.Text));
                    //    }

                    //}
                    //else if (subRow <= 0)
                    //{
                        rowData[subCol] = strLines[row].Substring(colStartPosList[subCol], colLengthList[subCol]);
                   //}

                }

                TempTable.Rows.Add(rowData);

                ParserGrid.ItemsSource = TempTable.DefaultView;
            }
        }

        private void TabItem_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            LenSlider.Minimum = 0;
            LenSlider.Maximum = 100;
            LenSlider.IsSnapToTickEnabled = true;
            LenSlider.TickFrequency = 1;
            dataTable = new DataTable();

            //strHeaders = new string[100];

            using (StreamReader txtReader = new StreamReader(file_path, encode))
            {
                string sLine = "";
                
                //ArrayList arrText = new ArrayList();
                sLine = txtReader.ReadLine();
                if (file_extension == ".txt")
                {
                    sLine = GetHeader(file_path, encode);
                    string[] lines = File.ReadAllLines(file_path);
                    int cou = 0;
                    while (txtReader.ReadLine() != sLine)
                    {
                        if (lines.Length <= cou)
                        {
                            break;
                        }
                        txtReader.ReadLine();
                        cou++;
                    }
                }

                sLine = sLine.Replace(".", "");
                strHeader = sLine;

                strHeaders = strHeader.Trim().Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                string[] rowData = new string[strHeaders.Count()];
                colLengthList = new int[strHeaders.Count()];
                colStartPosList = new int[strHeaders.Count()];
                //sLine = sLine.Replace(" ", "");

                dataTable.Columns.Add(sLine);

                while (sLine != null)
                {
                    sLine = txtReader.ReadLine();
                    dataTable.Rows.Add(sLine);
                    strLines.Add(sLine);
                    strRows.Add(rowData);
                    //arrText.Add(sLine);
                }

                txtReader.Close();
                ParserGrid.ItemsSource = dataTable.DefaultView;
            }
        }

        private void ColStartPosTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (colStartPosList == null)
                return;

            colStartPosList[System.Convert.ToInt32(ColTxt.Text)] = System.Convert.ToInt32(colStartPosTxt.Text);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo Info = new ProcessStartInfo();
            Info.Arguments = "/C choice /C Y /N /D Y /T 1 & START \"\" \"" + Assembly.GetExecutingAssembly().Location + "\"";
            Info.WindowStyle = ProcessWindowStyle.Hidden;
            Info.CreateNoWindow = true;
            Info.FileName = "cmd.exe";
            Process.Start(Info);
            Application.Current.Shutdown();
        }


        private void ListProducts(int category)
        {

            try
            {

                paging_PageIndex = 1;
                //dt_Products.Rows.Clear();
                //dt_Products.Columns.Add("Name");
                //dt_Products.Columns.Add("Address");

                //DataRow rowData = dt_Products.NewRow();
                //rowData["Name"] = "Qtech";
                //rowData["Address"] = "Hyderabd";
                //dt_Products.Rows.Add(rowData);

                //DataRow rowData2 = dt_Products.NewRow();
                //rowData2["Name"] = "Qtech2";
                //rowData2["Address"] = "Hyderabd2";
                //dt_Products.Rows.Add(rowData2);

                //DataRow rowData3 = dt_Products.NewRow();
                //rowData3["Name"] = "Qtech3";
                //rowData3["Address"] = "Hyderabd3";
                //dt_Products.Rows.Add(rowData3);
                //TotalRecords = ParseTable.Rows.Count;
                if (ParseTable.Rows.Count > 0)
                {
                    DataTable tmpTable = new DataTable();
                    tmpTable = ParseTable.Clone();

                    if (ParseTable.Rows.Count >= paging_NoOfRecPerPage)
                    {
                        for (int i = 0; i < paging_NoOfRecPerPage; i++)
                        {
                            tmpTable.ImportRow(ParseTable.Rows[i]);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < ParseTable.Rows.Count; i++)
                        {
                            tmpTable.ImportRow(ParseTable.Rows[i]);
                        }
                    }

                    dataGrid.DataContext = tmpTable.DefaultView;
                    tmpTable.Dispose();

                    DisplayPagingInfo();
                }
                else
                {
                    MessageBox.Show("No Records Exists for the selected category");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void ChangePageInfo()
        {
            
                if (!string.IsNullOrEmpty(textBoxFrom.Text))
                {
                if (IsDigitsOnly(textBoxFrom.Text))
                {
                    if (OldPageNumber != Convert.ToInt32(textBoxFrom.Text))
                    {
                        paging_NoOfRecPerPage = Convert.ToInt32(textBoxFrom.Text);
                        if (File.Exists(txtInputFile.Text))
                        {
                            MetaDataSaveBtn_Click(null, null);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Only Digits Allowe..!!");
                }
                OldPageNumber = paging_NoOfRecPerPage;
              }
        }

        private void DisplayPagingInfo()
        {
            string pagingInfo = "Displaying " + (((paging_PageIndex - 1) * paging_NoOfRecPerPage) + 1) + " to " + paging_PageIndex * paging_NoOfRecPerPage;

            if (ParseTable.Rows.Count < (paging_PageIndex * paging_NoOfRecPerPage))
            {
                pagingInfo = "Displaying " + (((paging_PageIndex - 1) * paging_NoOfRecPerPage) + 1) + " to " + ParseTable.Rows.Count;
            }
            lblPagingInfo.Content = pagingInfo;// +" out of "+ TotalRecords;

            lblPageNumber.Content = paging_PageIndex;
        }

        private void btnFirst_Click(object sender, RoutedEventArgs e)
        {
            CustomPaging((int)PagingMode.First);
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            CustomPaging((int)PagingMode.Previous);
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            CustomPaging((int)PagingMode.Next);
        }

        private void btnLast_Click(object sender, RoutedEventArgs e)
        {
            CustomPaging((int)PagingMode.Last);
        }


        private void CustomPaging(int mode)
        {
            int totalRecords = ParseTable.Rows.Count;
            int pageSize = paging_NoOfRecPerPage;
            int currentPageIndex = paging_PageIndex;

            if (ParseTable.Rows.Count <= paging_NoOfRecPerPage)
            {
                return;
            }

            switch (mode)
            {
                case (int)PagingMode.Next:
                    if (ParseTable.Rows.Count > (paging_PageIndex * paging_NoOfRecPerPage))
                    {
                        DataTable tmpTable = new DataTable();
                        tmpTable = ParseTable.Clone();

                        if (ParseTable.Rows.Count >= ((paging_PageIndex * paging_NoOfRecPerPage) + paging_NoOfRecPerPage))
                        {
                            for (int i = paging_PageIndex * paging_NoOfRecPerPage; i < ((paging_PageIndex * paging_NoOfRecPerPage) + paging_NoOfRecPerPage); i++)
                            {
                                tmpTable.ImportRow(ParseTable.Rows[i]);
                            }
                        }
                        else
                        {
                            for (int i = paging_PageIndex * paging_NoOfRecPerPage; i < ParseTable.Rows.Count; i++)
                            {
                                tmpTable.ImportRow(ParseTable.Rows[i]);
                            }
                        }

                        paging_PageIndex += 1;

                        dataGrid.DataContext = tmpTable.DefaultView;
                        tmpTable.Dispose();
                    }
                    break;
                case (int)PagingMode.Previous:
                    if (paging_PageIndex > 1)
                    {
                        DataTable tmpTable = new DataTable();
                        tmpTable = ParseTable.Clone();

                        paging_PageIndex -= 1;

                        for (int i = ((paging_PageIndex * paging_NoOfRecPerPage) - paging_NoOfRecPerPage); i < (paging_PageIndex * paging_NoOfRecPerPage); i++)
                        {
                            tmpTable.ImportRow(ParseTable.Rows[i]);
                        }

                        dataGrid.DataContext = tmpTable.DefaultView;
                        tmpTable.Dispose();
                    }
                    break;
                case (int)PagingMode.First:
                    paging_PageIndex = 2;
                    CustomPaging((int)PagingMode.Previous);
                    break;
                case (int)PagingMode.Last:
                    paging_PageIndex = (ParseTable.Rows.Count / paging_NoOfRecPerPage);
                    CustomPaging((int)PagingMode.Next);
                    break;
            }

            DisplayPagingInfo();
        }

        private void ParseXml_Click(object sender, RoutedEventArgs e)
        {
            
            //ApplyEdit();
            if (dataGrid.Items.Count < 1 || dataTable == null)
            {
                MessageBox.Show("There is no data to save.");
                return;
            }

            dataTable.Rows.Clear();
            dataTable.Columns.Clear();
            dataTable = ((DataView)dataGrid.ItemsSource).ToTable();  //dataGrid.table;


            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            string FolderPathMeta = "";
            string FolderPathData = "";
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                 FolderPathMeta = fbd.SelectedPath + "\\ReaderMeta.xml";
                 FolderPathData = fbd.SelectedPath + "\\ReaderData.xml";
            }
            if(!Directory.Exists(fbd.SelectedPath))
            {
                MessageBox.Show("Invalid file path..!");
                return;
            }
            List<MetaDataClassUpdated> metaDataArrayUpdated = new List<MetaDataClassUpdated>();
            if (FileTypeSelected == "DBF")
            {
                metaDataArrayUpdated = metaDataArray.Select(x => new MetaDataClassUpdated { Name = x.Name, StartPos = x.StartPos, Length = x.Length, TrimSpaces = x.TrimSpaces, OpenFileType = x.OpenFileType, RootArrayName = x.RootArrayName, InputFile = x.InputFile, FileEncoding = "None", DataTypeStr = x.DataTypeStr }).ToList();
            }
            else
            {
                metaDataArrayUpdated = metaDataArray.Select(x => new MetaDataClassUpdated { Name = x.Name, StartPos = x.StartPos, Length = x.Length, TrimSpaces = x.TrimSpaces, OpenFileType = x.OpenFileType, RootArrayName = x.RootArrayName, InputFile = x.InputFile, FileEncoding = x.FileEncoding, DataTypeStr = x.DataTypeStr }).ToList();
            }

            // Save Metadata as XML file
            XmlSerializer xs = new XmlSerializer(metaDataArrayUpdated.GetType());

            using (FileStream fs = new FileStream(FolderPathMeta, FileMode.Create))
                xs.Serialize(fs, metaDataArrayUpdated);

            // Save file data as XML file 
            dataTable.TableName = "dataList";
            dataTable.WriteXml(FolderPathData);
            MessageBox.Show("Success export into Data & MetaData!");
        }

        private void CmbFileType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (CmbFileType.SelectedItem != null)
            {
                ComboBoxItem typeItem = (ComboBoxItem)CmbFileType.SelectedItem;
                if (typeItem != null)
                {
                    if (typeItem.Content != null)
                    {
                        FileTypeSelected = typeItem.Content.ToString();
                        if (FileTypeSelected == "CSV")
                        {
                            dataGridColumns.IsReadOnly = false;
                            DelimiterLbl.Visibility = Visibility.Visible;
                            txtDelimiter.Visibility = Visibility.Visible;
                            EncLbl.Visibility = Visibility.Visible;
                            txtEnclousure.Visibility = Visibility.Visible;
                            if (HeadercheckBox != null)
                            {
                                HeadercheckBox.Visibility = Visibility.Visible;
                            }
                        }
                        else if (FileTypeSelected == "Text")
                        {
                            dataGridColumns.IsReadOnly = false;
                            DelimiterLbl.Visibility = Visibility.Hidden;
                            txtDelimiter.Visibility = Visibility.Hidden;
                            EncLbl.Visibility = Visibility.Hidden;
                            txtEnclousure.Visibility = Visibility.Hidden;
                            if (HeadercheckBox != null)
                            {
                                HeadercheckBox.Visibility = Visibility.Hidden;
                            }
                        }
                        else
                        {
                            dataGridColumns.IsReadOnly = true;
                            DelimiterLbl.Visibility = Visibility.Hidden;
                            txtDelimiter.Visibility = Visibility.Hidden;
                            EncLbl.Visibility = Visibility.Hidden;
                            txtEnclousure.Visibility = Visibility.Hidden;
                            if (HeadercheckBox != null)
                            {
                                HeadercheckBox.Visibility = Visibility.Hidden;
                            }
                        }

                       
                    }
                }
            }
        }

        private void HeadercheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (HeadercheckBox.IsChecked == true)
            {
                dataGridColumns.IsReadOnly = true;
            }
            else
            {
                dataGridColumns.IsReadOnly = false;
            }
        }

        private void textBoxFrom_LostFocus(object sender, RoutedEventArgs e)
        {
            ChangePageInfo();
        }

        private void cbxEncoding_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxEncoding.SelectedItem != null)
            {
                ComboBoxItem typeItem = (ComboBoxItem)cbxEncoding.SelectedItem;
                if (typeItem != null)
                {
                    if (typeItem.Content != null)
                    {
                        FileEncoding = typeItem.Content.ToString();
                        
                    }
                }
            }
        }


        
    }


}
