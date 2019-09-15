using System.Xml;
using DataFilter.Models;
using DataFilter.Services.Interfaces;

namespace DataFilter.Services
{
    public class SchemeReader : ISchemeReader
    {
        public Field ReadSchema(string path)
        {
            var rootField = new Field();

            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            SchemaChildren(doc.ChildNodes, rootField);

            return rootField;
        }

        private void SchemaChildren(XmlNodeList parent, Field field)
        {
            int id = 1;

            foreach (XmlNode child in parent)
            {
                if (child.Name != "xs:schema" && child.Name != "xml")
                {
                    string dataType = child.Attributes[1].Value.Replace("xs:", "") == "datetime" ? "Date/Time" : child.Attributes[1].Value.Replace("xs:", "");

                    var newField = new Field()
                    {
                        Id = id++,
                        Name = child.Attributes[0].Value,
                        Type = FirstLetterToUpper(dataType)
                    };

                    field.SubFields.Add(newField);

                    if (child.HasChildNodes)
                    {
                        SchemaChildren(child.ChildNodes, newField);
                    }
                }

                if (child.Name == "xs:schema")
                {
                    SchemaChildren(child.ChildNodes, field);
                }
            }
        }

        private string FirstLetterToUpper(string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }
    }
}
