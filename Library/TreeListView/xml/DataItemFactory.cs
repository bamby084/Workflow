using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using JdSuite.Common.Module;
using System.Linq.Expressions;
using System.Xml.Schema;
using System.Xml;

namespace JdSuite.Common.TreeListView
{
    /// <summary>
    /// This class is used to load data from xml data file into DataItem tree structure
    /// </summary>
    public class DataItemFactory
    {
        private NLog.ILogger logger = NLog.LogManager.GetLogger(nameof(DataItemFactory));
        public List<string> ErrorList { get; private set; } = new List<string>();

        public Field RootSchemaNode { get; private set; }
        public XElement RootXmlNode { get; private set; }
        public DataItem RootDataNode { get; private set; }

        public int Maxlevel { get; private set; }

        public void LoadData(string DataFile, string FieldFile)
        {
           // DataFile = @"E:\Canvas2\Dropbox\BlockApp\Data\Customer_data_XML_1.xml"; //for testing only
           // FieldFile = @"E:\Canvas2\Dropbox\BlockApp\Data\PassedSchema.xml";

            using (XmlReader xmlReader = XmlReader.Create(DataFile))
            {
                
                LoadOptions options = LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo | LoadOptions.SetBaseUri;
                XDocument doc = XDocument.Load(xmlReader, options);

                RootXmlNode = doc.Root;
                RootSchemaNode = Field.Parse(FieldFile);

                RootDataNode = new DataItem();
                RootDataNode.Level = 1;
                int maxLevel = 0;

                AddNode(RootDataNode, RootXmlNode, RootSchemaNode, ref maxLevel);
                this.Maxlevel = maxLevel;
            }
        }

        /// <summary>
        /// Loads data from DataFile (xml data file)
        /// </summary>
        /// <param name="DataFile">xml data file</param>
        /// <param name="rootSchemaField">root schema node</param>
        public void LoadData(string DataFile, Field rootSchemaField)
        {            

            using (XmlReader xmlReader = XmlReader.Create(DataFile))
            {
               
                LoadOptions options = LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo | LoadOptions.SetBaseUri;
                XDocument doc = XDocument.Load(xmlReader, options);

                RootXmlNode = doc.Root;
                RootSchemaNode = rootSchemaField;

                RootDataNode = new DataItem();
                RootDataNode.Level = 1;
                int maxLevel = 0;

                AddNode(RootDataNode, RootXmlNode, RootSchemaNode, ref maxLevel);
                this.Maxlevel = maxLevel;
            }
        }

        public void AddNode(DataItem dataItem, XElement xElement, Field schema, ref int maxLevel)
        {
            try
            {
                if (xElement == null)
                {
                    logger.Error("XElement is null");
                    return;
                }



                var nodeName = xElement.Name.LocalName;

                if (schema == null)
                {
                    logger.Error($"Schema node for xml data node {nodeName} is null");
                    return;
                }

                var nextSchemaNode = schema;
                if (nextSchemaNode.Name != nodeName)
                {
                    nextSchemaNode = schema.ChildNodes.FirstOrDefault(x => x.Name == nodeName);
                }

                if (nextSchemaNode == null)
                {
                    logger.Error($"Schema node for xml data node {nodeName} is null");
                    return;
                }


                var targetType = nextSchemaNode.DataType;
                targetType = targetType.Replace("xs:", "");

                string nodeValue = xElement.Value;
                if (xElement.HasElements)
                    nodeValue = "Array";

                //Validate xml element value
                if (nodeValue != "Array")
                    ValidateNodeDataType(nodeName, targetType, nodeValue);


                IXmlLineInfo lineInfo = xElement as IXmlLineInfo;
                if (lineInfo != null)
                {
                    dataItem.LineNo = lineInfo.LineNumber;
                    dataItem.Position = lineInfo.LinePosition;
                }
                dataItem.Name = nodeName;
                dataItem.Type = targetType;
                dataItem.Value = "Array";// element.Value;


                foreach (var childXelement in xElement.Elements())
                {
                    if (childXelement.HasElements)
                    {
                        var childNode = dataItem.AddChild(new DataItem());

                        AddNode(childNode, childXelement, nextSchemaNode, ref maxLevel);

                        lineInfo = childXelement as IXmlLineInfo;
                        if (lineInfo != null)
                        {
                            childNode.LineNo = lineInfo.LineNumber;
                            childNode.Position = lineInfo.LinePosition;
                        }

                        if (maxLevel < childNode.Level)
                        {
                            maxLevel = childNode.Level;
                        }
                    }
                    else
                    {
                        AddProp(dataItem, childXelement, nextSchemaNode);
                    }
                }
                if (dataItem.ReverseLevel < maxLevel)
                {
                    dataItem.ReverseLevel = maxLevel;
                }

                // Debug.WriteLine("ReverseLevel = " + dataItem.ReverseLevel);

            }
            catch (Exception ex)
            {
                logger.Error(ex, $"XElement_processing_error {xElement.Name.LocalName} Err:{ex.Message}");
            }
        }

        public void AddProp(DataItem dataItem, XElement element, Field schema)
        {
            if (element == null)
            {
                logger.Error($"XElement is null");
                return;
            }

            var nodeName = element.Name.LocalName;

            var schemaNode = schema;
            if (schemaNode.Name != nodeName)
            {
                schemaNode = schema.ChildNodes.FirstOrDefault(x => x.Name == nodeName);
            }


            if (schemaNode == null)
            {
                logger.Error($"Schema node for XElement {nodeName} is null");
                return;
            }

            var targetType = schemaNode.DataType;
            targetType = targetType.Replace("xs:", "");


            string nodeValue = element.Value;

            if (element.HasElements)
                nodeValue = "Array";

            if (nodeValue != "Array")
                ValidateNodeDataType(nodeName, targetType, nodeValue);

            var propItem = new DataItem(nodeName, targetType, nodeValue);
            var lineInfo = element as IXmlLineInfo;
            if (lineInfo != null)
            {
                propItem.LineNo = lineInfo.LineNumber;
                propItem.Position = lineInfo.LinePosition;
            }
            dataItem.AddProp(propItem);
        }


        public bool ValidateNodeDataType(string nodeName, string targetType, string nodeValue)
        {


            //Validate xml element value
            bool isValid = true;

            if (targetType == "Int16")
            {
                if (!Int16.TryParse(nodeValue, out var intVal))
                {
                    isValid = false;
                    logger.Error($"Node {nodeName} Value {nodeValue} is invalid Required {targetType} ");
                }
            }
            else if (targetType == "Int32")
            {
                if (!int.TryParse(nodeValue, out var intVal))
                {
                    isValid = false;
                    logger.Error($"Node {nodeName} Value {nodeValue} is invalid Required {targetType} ");
                }
            }

            else if (targetType == "Int64")
            {
                if (!Int64.TryParse(nodeValue, out var intVal))
                {
                    isValid = false;
                    logger.Error($"Node {nodeName} Value {nodeValue} is invalid Required {targetType} ");
                }
            }
            else if (targetType == "Boolean" || targetType == "bool")
            {
                if (!bool.TryParse(nodeValue, out var intVal))
                {
                    isValid = false;
                    logger.Error($"Node {nodeName} Value {nodeValue} is invalid Required {targetType} ");
                }
            }
            else if (targetType == "Double")
            {
                if (!double.TryParse(nodeValue, out var intVal))
                {
                    logger.Error($"Node {nodeName} Value {nodeValue} is invalid Required {targetType} ");
                }
            }
            else if (targetType == "Single")
            {
                if (!Single.TryParse(nodeValue, out var intVal))
                {
                    isValid = false;
                    logger.Error($"Node {nodeName} Value {nodeValue} is invalid Required {targetType} ");
                }
            }
            else if (targetType == "Date/Time")
            {
                if (!DateTime.TryParse(nodeValue, out var intVal))
                {
                    isValid = false;
                    logger.Error($"Node {nodeName} Value {nodeValue} is invalid Required {targetType} ");
                }
            }

            return isValid;
        }
    }
}
