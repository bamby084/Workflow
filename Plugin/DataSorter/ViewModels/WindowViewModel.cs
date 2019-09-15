using System.Collections.ObjectModel;
using System.ComponentModel;
using JdSuite.Common;
using JdSuite.Common.Module;


namespace JdSuite.DataSorting.ViewModels
{
    public class ViewModel : NotifyPropertyChangeBase
    {
        public ViewModel()
        {
            FieldNodes = new ObservableCollection<Field>();
            SortingFields = new ObservableCollection<SortingField>();
        }

        private bool isAddFieldButtonEnabled;

        private bool isSortedFieldSelected;

        private bool isSortedFieldMovable;

        private bool isAnyFieldSorted;

        private string _dataDirectory;
        public string DataDirectory
        {
            get { return _dataDirectory; }
            set { SetPropertry(ref _dataDirectory, value); }
        }


        private string _schemaFile;
        public string SchemaFile
        {
            get { return _schemaFile; }
            set { SetPropertry(ref _schemaFile, value); }
        }

        private string _inputDataFile;
        public string InputDataFile
        {
            get { return _inputDataFile; }
            set { SetPropertry(ref _inputDataFile, value); }
        }


        private string _outputDataFile;
        public string OutputDataFile
        {
            get { return _outputDataFile; }
            set { SetPropertry(ref _outputDataFile, value); }
        }


        public ObservableCollection<Field> FieldNodes { get; set; }

        public ObservableCollection<SortingField> SortingFields { get; set; }

        public bool IsAddFieldButtonEnabled
        {
            get { return isAddFieldButtonEnabled; }
            set
            {
                SetPropertry(ref isAddFieldButtonEnabled, value);
            }
        }

        public bool IsSortedFieldSelected
        {
            get { return isSortedFieldSelected; }
            set
            {
                SetPropertry(ref isSortedFieldSelected, value);
            }
        }

        public bool IsSortedFieldMovable
        {
            get { return isSortedFieldMovable; }
            set
            {
                SetPropertry(ref isSortedFieldMovable, value);
            }
        }

        public bool IsAnyFieldSorted
        {
            get { return isAnyFieldSorted; }
            set
            {
                SetPropertry(ref isAnyFieldSorted, value);
            }
        }



        public void Unload()
        {
            this.IsAddFieldButtonEnabled = false;
            this.IsSortedFieldSelected = false;
            this.IsSortedFieldMovable = false;
            this.IsAnyFieldSorted = false;

            this.FieldNodes.Clear();
            this.SortingFields.Clear();
        }
    }
}
