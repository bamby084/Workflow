using System.Collections.ObjectModel;
using System.ComponentModel;
using DataFilter.Models.Filters;

namespace DataFilter.Models
{
    public class Field : INotifyPropertyChanged
    {
        private Filter filter;
        private bool isFiltered;
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public Filter Filter
        {
            get { return filter; }
            set
            {
                filter = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Filter"));

                IsFiltered = filter != null;
            }
        }
        public bool IsFiltered
        {
            get { return isFiltered; }
            private set
            {
                isFiltered = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsFiltered"));
            }
        }
        public bool IsSelected { get; set; }
        public bool IsExpanded { get; set; } = true;
        public ObservableCollection<Field> SubFields { get; set; }
        public Field()
        {
            SubFields = new ObservableCollection<Field>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
