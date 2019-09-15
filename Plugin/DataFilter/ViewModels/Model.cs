using DataFilter.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DataFilter.ViewModels
{
    public class Model : INotifyPropertyChanged
    {
        public Model()
        {
            SubFields = new ObservableCollection<Field>();
        }

        private bool isAddFieldButtonEnabled;

        private bool isDeleteFieldButtonEnabled;

        public ObservableCollection<Field> SubFields { get; set; }

        public bool IsAddFieldButtonEnabled
        {
            get { return isAddFieldButtonEnabled; }
            set
            {
                isAddFieldButtonEnabled = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsAddFieldButtonEnabled"));
            }
        }

        public bool IsDeleteFieldButtonEnabled
        {
            get { return isDeleteFieldButtonEnabled; }
            set
            {
                isDeleteFieldButtonEnabled = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsDeleteFieldButtonEnabled"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Unload()
        {
            this.IsAddFieldButtonEnabled = false;
            this.IsDeleteFieldButtonEnabled = false;

            this.SubFields.Clear();
        }
    }
}
