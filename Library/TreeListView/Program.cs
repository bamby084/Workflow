using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JdSuite.Common.TreeListView
{
    public static class Program
    {

        [STAThread]
        public static void Main()
        {
            DataItemFactory dataFactory = new DataItemFactory();
            // fac.ValidateByXSD();
            //  var lst = fac.ErrorList;

            dataFactory.LoadData("", "");

            XMLFieldValidator validator = new XMLFieldValidator();
            validator.Validate(dataFactory.RootDataNode, dataFactory.RootSchemaNode);

            validator.LogError();
            

        }
    }
}
