using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using JdSuite.Common.Module;

namespace JdSuite.Common.TreeListView
{
    /// <summary>
    /// Instances of this class are used to hold data from xml data file for tree list control nodes. Each node is binded with one DataItem object.
    /// </summary>
    public class DataItem : ViewModelBase
    {
        private string _name;
        private string _type;
        private string _value;
        private int _level;
        private int _reverseLevel;

        public int LineNo;
        public int Position;
        private ObservableCollection<DataItem> _props;
        private ObservableCollection<DataItem> _children;

        public DataItem()
        {

        }


        public DataItem(string name,string type, string svalue)
        {
            _name = name;
            _type = type;
            _value = svalue;
        }

       
        public string Name
        {
            get { return _name; }
            set { SetPropertry(ref _name, value); }
        }

       
        public string Type
        {
            get { return _type; }
            set { SetPropertry(ref _type, value); }
        }

       
        public string Value
        {
            get { return _value; }
            set { SetPropertry(ref _value, value); }
        }

        
        public int Level
        {
            get { return _level; }
            set { SetPropertry(ref _level, value); }
        }

       
        public int ReverseLevel
        {
            get { return _reverseLevel; }
            set { SetPropertry(ref _reverseLevel, value); }
        }

        /// <summary>
        /// Number of child nodes
        /// </summary>
        public int ChildCount
        {
            get { return _children?.Count ?? 0; }
        }

        /// <summary>
        /// Number of properties
        /// </summary>
        public int PropCount
        {
            get { return _props?.Count ?? 0; }
        }

        
        public ObservableCollection<DataItem> Props
        {
            get { return _props; }
            set
            {
                SetPropertry(ref _props, value);
            }
        }


     
        public ObservableCollection<DataItem> Children
        {
            get { return _children; }
            set
            {

                SetPropertry(ref _children, value);
            }
        }

        public void AddProp(DataItem prop)
        {
            if (_props == null)
            {
                _props = new ObservableCollection<DataItem>();
            }
            prop.Level = this.Level + 1;
            Props.Add(prop);
        }

        /// <summary>
        /// Sets child level=Parent level +1
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public DataItem AddChild(DataItem child)
        {
            if (_children == null)
            {
                _children = new ObservableCollection<DataItem>();
            }
            child.Level = this.Level + 1;
            Children.Add(child);
           
            return child;
        }

       


    }
}
