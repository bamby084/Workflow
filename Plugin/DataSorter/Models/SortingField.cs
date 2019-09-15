
using JdSuite.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace JdSuite.DataSorting
{

    [Serializable]
    public class SortingField : NotifyPropertyChangeBase
    {
        int _id;
        private string _xpath;
        private string _name;
        SortingType _sortingType;
        private ComparisonMode _compareMode;
        private bool _removeDuplicates;

        [XmlAttribute]
        public int Id
        {
            get { return _id; }
            set
            {
                SetPropertry(ref _id, value);
            }
        }

        [XmlAttribute]
        public string XPath
        {
            get { return _xpath; }
            set { SetPropertry(ref _xpath, value); }
        }

        [XmlAttribute]
        public string Name
        {
            get { return _name; }
            set { SetPropertry(ref _name, value); }
        }


        [XmlAttribute]
        public SortingType SortingType
        {
            get { return _sortingType; }
            set { SetPropertry(ref _sortingType, value); }
        }

        [XmlAttribute]
        public ComparisonMode ComparisonMode
        {
            get { return _compareMode; }
            set { SetPropertry(ref _compareMode, value); }
        }

        [XmlAttribute]
        public bool RemoveDuplicate
        {
            get { return _removeDuplicates; }
            set { SetPropertry(ref _removeDuplicates, value); }
        }
              

        public string GetLeafName()
        {
            var parts = XPath.Split('/');
            return parts[parts.Length - 1];
        }

        public string GetParentOf(string childName)
        {
            var parts = XPath.Split('/');
            var index = Array.IndexOf<string>(parts, childName);
            if (index <= 0)
            {
                return string.Empty;
            }
            return parts[index - 1];
        }


        public BaseComparer<string> GetComparer()
        {
            if (ComparisonMode == ComparisonMode.Integer)
            {
                return new IntegerComparer();
            }
            else if (ComparisonMode == ComparisonMode.Float)
            {
                return new FloatComparer();
            }
            else if (ComparisonMode == ComparisonMode.Decimal)
            {
                return new DecimalComparer();
            }
            else if (ComparisonMode == ComparisonMode.Binary)
            {
                return new BinaryComparer();
            }

            return new StringComparer(this);
        }

        /// <summary>
        /// Creates sorting expression for single level sorting
        /// </summary>
        /// <param name="ParentNode"></param>
        /// <returns></returns>
        public IOrderedEnumerable<XElement> Process(XElement ParentNode)
        {
            if (ParentNode == null)
                throw new ArgumentNullException(nameof(ParentNode));

            IOrderedEnumerable<XElement> orderNodeTree = null;
            if (SortingType == SortingType.Ascending)
            {
                orderNodeTree = ParentNode.Descendants(GetLeafName()).OrderBy(x => x.Value, GetComparer());
            }
            else
            {
                orderNodeTree = ParentNode.Descendants(GetLeafName()).OrderByDescending(x => x.Value, GetComparer());
            }
            return orderNodeTree;
        }

    }
}
