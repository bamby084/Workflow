using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace JdSuite.Common.Module
{

    /// <summary>
    /// This class should be used by all modules which need to load/save and process xml schema nodes
    /// 
    /// </summary>
    public class Field
    {
        [XmlAttribute]
        public String Type { get; set; }
        [XmlAttribute]
        public String Name { get; set; }
        [XmlAttribute]
        public String DataType { get; set; }
        [XmlAttribute]
        public String Use { get; set; }
        [XmlAttribute]
        public String Change { get; set; }
        [XmlAttribute]
        public String Alias { get; set; }

        [XmlElement("Field")]
        public List<Field> ChildNodes { get; set; } = new List<Field>();


        public IEnumerable<Field> GetProps()
        {
           return ChildNodes.Where(x => x.ChildNodes == null || x.ChildNodes.Count == 0);
        }

        public IEnumerable<Field> GetContainerChild()
        {
            return ChildNodes.Where(x => x.ChildNodes == null || x.ChildNodes.Count > 0);
        }

        public void Save(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(this.GetType());
            using (StreamWriter writer = new StreamWriter(filename))
            {
                serializer.Serialize(writer, this);
            }
        }

      

        public static void FindChain(Field Parent, ref Field Child, ref bool Found, ref List<Field> Chain)
        {
            if (Found == true)
            {
                if (Parent.ChildNodes.Contains(Chain.Last()))
                    Chain.Add(Parent);
                return;
            }

            if (Parent.ChildNodes == null || Parent.ChildNodes.Count == 0)
                return;

            if (Parent.ChildNodes.Contains(Child))
            {
                Chain.Add(Child);
                Chain.Add(Parent);
                Found = true;
                return;
            }

            for (int i = 0; i < Parent.ChildNodes.Count; i++)
            {
                FindChain(Parent.ChildNodes[i], ref Child, ref Found, ref Chain);
                if (Found)
                {
                    if (Parent.ChildNodes.Contains(Chain.Last()))
                        Chain.Add(Parent);
                    break;
                }
                    
            }


        }

        /// <summary>
        /// Loads xml schema fields into Field data structure
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Field Parse(String filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Field));
            using (Stream reader = new FileStream(filename, FileMode.Open))
            {
                return (Field)serializer.Deserialize(reader);
            }
        }
    }
}
