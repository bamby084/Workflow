using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace JdSuite.DataSorting
{
    public class XMLSorter
    {
        NLog.ILogger logger = NLog.LogManager.GetLogger(nameof(XMLSorter));

        public string DataFile { get; set; }
        public string OutputFileName { get; set; }
        public List<SortingField> SortingFields { get; private set; } = new List<SortingField>();

        private XElement rootNode;

        /// <summary>
        /// 
        /// </summary>
        public void LoadData()
        {
            logger.Info("Loading xml data file {0}", DataFile);
            rootNode = XElement.Load(DataFile);

            /*
            //Test Case 1
           SortingFields.Add(new SortingField()
           {
               XPath = "/Invoices/Invoice/ZIP",
               RemoveDuplicate = true,
               SortingType = SortingType.Ascending,
               ComparisonMode = ComparisonMode.CaseSensitive
           });



             //Test Case 2
           SortingFields.Add(new SortingField()
           {
               XPath = "/Invoices/Invoice/items/item/QTY",
               RemoveDuplicate = true,
               SortingType = SortingType.Ascending,
               ComparisonMode = ComparisonMode.Integer
           });

           SortingFields.Add(new SortingField()
           {
               XPath = "/Invoices/Invoice/items/item/ItemNumber",
               RemoveDuplicate = true,
               SortingType = SortingType.Ascending,
               ComparisonMode = ComparisonMode.Integer
           });
          

            //Test Case 3
            SortingFields.Add(new SortingField()
            {
                XPath = "/item/QTY",
                RemoveDuplicate = true,
                SortingType = SortingType.Ascending,
                ComparisonMode = ComparisonMode.Integer
            });

            SortingFields.Add(new SortingField()
            {
                XPath = "/item/ItemNumber",
                RemoveDuplicate = true,
                SortingType = SortingType.Ascending,
                ComparisonMode = ComparisonMode.Float
            });
             */

        }


        public void Sort()
        {
            logger.Info("Entered");
            try
            {
                if (SortingFields.Count == 0)
                {
                    logger.Warn("No sorting field is found to sort data, please set a sorting field first");
                    throw new InvalidOperationException("No sorting field is found to sort data, please set a sorting field first");
                }

                SortingField rootSF = SortingFields[0];//xpath is null form grid

                var endElement = rootNode.Descendants(rootSF.GetLeafName()).First();
                var parentElement = endElement.Parent;
                var grandParentElement = parentElement.Parent;


                if (grandParentElement == null)
                {
                    TwoLevelSort(rootSF);
                }
                else
                {
                    MultiLevelSort(rootSF, grandParentElement, parentElement);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "XML Sorting Error, DataFile {0}", DataFile);
                throw ex;
            }
            logger.Info("Leaving");
        }

        public void Save()
        {
            rootNode.Save(OutputFileName);
        }

        /// <summary>
        /// It is used to sort root/child/grand_child or a more deeper hieararchy
        /// </summary>
        /// <param name="rootSF"></param>
        /// <param name="grandParentElement"></param>
        /// <param name="parentElement"></param>
        private void MultiLevelSort(SortingField rootSF, XElement grandParentElement, XElement parentElement)
        {
            logger.Info("Sorting using xpath {0}", rootSF.XPath);

            if (rootNode == grandParentElement)
            {
                logger.Info("RootNode is GrandParent element case, RootSF XPath {0}", rootSF.XPath);

                // IEnumerable<XElement> grandParentNodes = null;
                IEnumerable<XElement> parentNodes = grandParentElement.Descendants(parentElement.Name);
                IOrderedEnumerable<XElement> orderNodeTree = Process(parentNodes, rootSF);


                for (int i = 1; i < SortingFields.Count; i++)
                {
                    var sf = SortingFields[i];
                    //logger.Info("Sorting using SF {0}", sf.XPath);
                    orderNodeTree = Process(orderNodeTree, sf);
                }


                foreach (var item in orderNodeTree)
                {
                    item.Remove();
                    grandParentElement.Add(item);
                }

            }
            else
            {
                logger.Info("RootNode <> GrandParent element case, RootSF XPath {0}", rootSF.XPath);

                var grandParentNodes = rootNode.Descendants(grandParentElement.Name);

                foreach (var gparent in grandParentNodes)
                {

                    IOrderedEnumerable<XElement> orderNodeTree = null;

                    orderNodeTree = Process(gparent.Descendants(parentElement.Name), rootSF);


                    for (int i = 1; i < SortingFields.Count; i++)
                    {
                        var sf = SortingFields[i];
                       // logger.Info("Sorting using SF {0}", sf.XPath);
                        orderNodeTree = Process(orderNodeTree, sf);
                    }


                    foreach (var item in orderNodeTree)
                    {
                        item.Remove();
                        gparent.Add(item);
                    }
                }
            }
            logger.Info("Multi-level sorting completed");

        }

        /// <summary>
        /// It is used to sort root/child only case
        /// </summary>
        /// <param name="rootSF"></param>
        private void TwoLevelSort(SortingField rootSF)
        {

            logger.Info("Sorting using xpath {0}", rootSF.XPath);

            IOrderedEnumerable<XElement> orderNodeTree = null;

            orderNodeTree = rootSF.Process(rootNode);

            var lastElement = ApplyOrder(rootNode, orderNodeTree);

            for (int i = 1; i < SortingFields.Count; i++)
            {
                var sf = SortingFields[i];
                //logger.Info("Sorting using SF {0}", sf.XPath);

                orderNodeTree = sf.Process(rootNode);
                lastElement = ApplyOrderAfter(lastElement, orderNodeTree);
            }

            logger.Info("2-level sorting operation completed");
        }

        /// <summary>
        /// Adds ordered XElements starting as first child of ParentNode and returns last XElement added to tree
        /// </summary>
        /// <param name="ParentNode"></param>
        /// <param name="orderNodeTree"></param>
        /// <returns></returns>
        private XElement ApplyOrder(XElement ParentNode, IOrderedEnumerable<XElement> orderNodeTree)
        {
            bool first = true;
            XElement last = null;
            foreach (var item in orderNodeTree)
            {
                item.Remove();
                if (first)
                {
                    ParentNode.AddFirst(item);
                    last = item;
                    first = false;
                }
                else
                {
                    last.AddAfterSelf(item);
                    last = item;
                }
            }

            return last;
        }

        /// <summary>
        /// Adds ordered XElements after PreviousSibling and returns last XElement added to tree
        /// </summary>
        /// <param name="PreviousSibling"></param>
        /// <param name="orderNodeTree"></param>
        /// <returns></returns>
        private XElement ApplyOrderAfter(XElement PreviousSibling, IOrderedEnumerable<XElement> orderNodeTree)
        {

            XElement last = PreviousSibling;
            foreach (var item in orderNodeTree)
            {
                item.Remove();
                last.AddAfterSelf(item);
                last = item;
            }

            return last;
        }

        /// <summary>
        /// Called by multi level grand_parent/root_node for first level ordering
        /// </summary>
        /// <param name="NodeTree"></param>
        /// <param name="sf"></param>
        /// <returns></returns>
        public IOrderedEnumerable<XElement> Process(IEnumerable<XElement> NodeTree, SortingField sf)
        {

            IOrderedEnumerable<XElement> orderNodeTree;

            string elementName = sf.GetLeafName();

            if (sf.SortingType == SortingType.Ascending)
            {
                orderNodeTree = NodeTree.OrderBy(m => m.Element(elementName).Value, sf.GetComparer());
            }
            else
            {
                orderNodeTree = NodeTree.OrderByDescending(m => m.Element(elementName).Value, sf.GetComparer());
            }

            return orderNodeTree;
        }

        /// <summary>
        /// Called by multi level child nodes for second level ordering
        /// </summary>
        /// <param name="orderNodeTree"></param>
        /// <param name="sf"></param>
        /// <returns></returns>
        public IOrderedEnumerable<XElement> Process(IOrderedEnumerable<XElement> orderNodeTree, SortingField sf)
        {

            string elementName = sf.GetLeafName();


            if (sf.SortingType == SortingType.Ascending)
            {
                orderNodeTree = orderNodeTree.ThenBy(m => m.Element(elementName).Value, sf.GetComparer());
            }
            else
            {
                orderNodeTree = orderNodeTree.ThenByDescending(m => m.Element(elementName).Value, sf.GetComparer());
            }

            return orderNodeTree;
        }





    }
}

