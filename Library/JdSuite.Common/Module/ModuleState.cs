using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace JdSuite.Common.Module
{

    /// <summary>
    /// ModuleState class is used to transfer module state from one module to other
    /// </summary>
    [Serializable]
    public class ModuleState
    {
        public ModuleState() { }


        [XmlAttribute]
        public bool InputIsSchema { get; set; } = false;


        [XmlAttribute]
        public string TextEncoding { get; set; } = String.Empty;



        /// <summary>
        /// Usually shows input path of data file
        /// </summary>
        [XmlAttribute]
        public string InputPath { get; set; } = String.Empty;

       

        /// <summary>
        /// Usually shows XML schema in Field form
        /// </summary>
        public Field Schema { get; set; }
    }

}
