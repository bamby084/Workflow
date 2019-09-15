using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataOutput.ViewModel
{
    public class OutputViewModel : ViewModelBase
    {
        private string _struct = "";
        public string Struct
        {
            get { return _struct; }
            set => SetPropertry(ref _struct, value);
        }

        private string _type = "";
        public string Type
        {
            get { return _type; }
            set => SetPropertry(ref _type, value);
        }

        private string _value = "";
        public string Value
        {
            get { return _value; }
            set => SetPropertry(ref _value, value);
        }
    }
}
