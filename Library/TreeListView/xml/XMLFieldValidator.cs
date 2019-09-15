using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using JdSuite.Common.Module;

namespace JdSuite.Common.TreeListView
{
    /// <summary>
    /// This class is used to validate xml data nodes, using Field schema
    /// </summary>
    public class XMLFieldValidator
    {
        public List<string> ErrorList { get; private set; } = new List<string>();

        NLog.ILogger logger = NLog.LogManager.GetLogger(nameof(XMLFieldValidator));

        DataItem RootDataNode = null;
        Field RootSchemaNode = null;

        public void Validate(DataItem rootDataNode, Field rootSchemaNode)
        {

            try
            {
                this.RootDataNode = rootDataNode;
                this.RootSchemaNode = rootSchemaNode;

                int i = 1;
                var schemaNode = RootSchemaNode.ChildNodes[0];
                foreach (var child in RootDataNode.Children)
                {
                    logger.Info($"Validating {RootDataNode.Name}/{child.Name}[{i}] Schema:{RootSchemaNode.Name}/{schemaNode.Name}");
                    ValidateInternal(child, schemaNode);
                    i++;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }


        }



        private bool ValidateInternal(DataItem dataNode, Field schemaNode)
        {
            bool isValid = true;
            Field ff_current = schemaNode;
            DataItem data_current = dataNode;

            if (ff_current.Name != data_current.Name)
            {
                ErrorList.Add($"Expected XMLNode {ff_current.Name} found {data_current.Name} Line:{dataNode.LineNo}");
                isValid = false;
            }

            //logger.Info($"Validating {data_current.Name} properties");

            foreach (var ff_prop in ff_current.GetProps())
            {
                var data_prop = data_current.Props.FirstOrDefault(x => x.Name == ff_prop.Name);
                if (data_prop == null)
                {
                    ErrorList.Add($"Expected XMLNode {ff_current.Name}/{ff_prop.Name} is missing in {data_current.Name} Line:{data_current.LineNo}");
                    isValid = false;
                }
                else
                {
                    if (!DataTypeChecker.IsValid(ff_prop, data_prop.Value))
                    {
                        ErrorList.Add($"Expected XMLNode {ff_current.Name}/{ff_prop.Name} value type mismatch Type:{ff_prop.Type}[{data_prop.Value}] Line:{data_prop.LineNo}");
                    }
                }
            }



            foreach (var ff_child in ff_current.GetContainerChild())
            {
               // logger.Info($"Validating child {ff_child.Name}");

                try
                {
                    if(data_current.ChildCount==0)
                    {
                        ErrorList.Add($"Expected XMLNode <{ff_child.Name}> is missing in <{data_current.Name}> Line:{data_current.LineNo}");
                        isValid = false;
                        data_current.AddChild(new DataItem(ff_child.Name, ff_child.Type, "Array"));
                        break;
                    }

                    lblCheckAgain:
                    var data_child = data_current.Children.FirstOrDefault(x => x.Name == ff_child.Name);
                    if (data_child == null)
                    {
                        ErrorList.Add($"Expected XMLNode <{ff_child.Name}> is missing in <{data_current.Name}> Line:{data_current.LineNo}");
                        data_current.AddChild(new DataItem(ff_child.Name, ff_child.Type, "Array"));
                        isValid = false;
                        goto lblCheckAgain;
                    }
                    else
                    {
                        if (ff_child.ChildNodes != null)
                        {
                            ValidateInternal(data_child, ff_child);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorList.Add($"Child Node <{ff_child.Name}> validation error <{data_current.Name}> Line:{data_current.LineNo} {ex.Message}");
                }
            }

            return isValid;


        }

        public void LogError()
        {
            logger.Error($"XML validation errors: {string.Join(Environment.NewLine, ErrorList)} ");
        }

        public void LogEachError()
        {
            foreach (var err in ErrorList)
            {
                logger.Error(err);
            }
        }
    }
}
