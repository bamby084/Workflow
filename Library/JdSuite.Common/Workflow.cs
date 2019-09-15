using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace JdSuite.Common
{
    public class Workflow
    {
        NLog.ILogger logger = NLog.LogManager.GetLogger("Workflow");
        private XDocument Document;
        private string Directory;
        public string Title;
        private bool isValid = false;
        private bool isDirty = true;
        public int Command { get; set; }

        public Workflow()
        {
            logger.Info("Creating Workflow object");
        }

        public Workflow(XDocument doc, string directory, string title)
        {
            logger.Info("Creating Workflow object directory:{0}, title:{1}", directory, title);

            isValid = true;
            Document = doc;
            Directory = directory;
            Title = title;
        }

        public void SetAsNew()
        {
            isValid = true;
            isDirty = true;
            Document = new XDocument(new XElement("Root"));
            Directory = null;
            Title = "Workflow 1";
        }

        public bool TryLoad(string path)
        {

            if (!File.Exists(path))
            {
                return false;
            }

            if (Path.GetExtension(path).ToLower() != ".flo")
            {
                return false;
            }

            try
            {
                logger.Info("Loading workflow file:{0}", path);
                Document = XDocument.Load(path);
            }
            catch (Exception)
            {
                return false;
            }

            Title = Path.GetFileNameWithoutExtension(path);
            Directory = Path.GetDirectoryName(path);
            isValid = true;
            isDirty = false;
            return true;
        }

        public string GetFullPath()
        {
            if (Directory == null)
            {
                return "";
            }
            return Path.Combine(Directory, String.Format("{0}.flo", Title));
        }

        public bool HasDirectory()
        {
            return Directory != null;
        }

        public void SetDirectory(string directory)
        {
            if (!isValid)
            {
                return;
            }
            Directory = directory;
            isDirty = true;
        }

        public bool TrySave()
        {
            try
            {
                Save();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public void Save()
        {
            Document.Save(Path.Combine(Directory, Title + ".flo"));
            isDirty = false;
        }


        /// <summary>
        /// Gets the state of the requested application.
        /// </summary>
        /// <param name="appName">Name of the application.</param>
        /// <param name="guid">The unique identifier for the requested instance. 
        /// If only one occurance of state is expected and a Guid is not needed, 
        /// Guid.Empty may be passed as a substitute for null.</param>
        public XElement GetAppState(string appName, Guid guid)
        {
            var guidStr = guid.ToString();
            return Document?.Root.Elements(appName)?.Where((XElement e) =>
            {
                return e.Attribute("Guid").Value == guidStr;
            }).FirstOrDefault();
        }

        public bool HasUnsavedChanges()
        {
            return isDirty;
        }

        /// <summary>
        /// Sets the state of the application.
        /// </summary>
        /// <param name="appName">Name of the application.</param>
        /// <param name="state">The state.</param>
        /// <param name="guid">The unique identifier for this instance. 
        /// If only one occurance of state is expected and a Guid is not needed, 
        /// Guid.Empty may be passed as a substitute for null.</param>
        public void SetAppState(string appName, XElement state, Guid guid)
        {
            if (!isValid)
            {
                return;
            }

            logger.Trace($"Setting State appName:{appName}, guid:{guid}");

            var element = Document.Root.Element(appName);

            if (element == null)
            {
                element = new XElement(appName, new XAttribute("Guid", guid.ToString()));
                element.Add(state);
                Document.Root.Add(element);
            }
            else
            {
                var cachedGuid = Guid.Parse(element.Attribute("Guid").Value);
                if (cachedGuid != guid || ElementEquals((XElement)element.FirstNode, state))
                {
                    return;
                }

                element.RemoveNodes();
                element.Add(state);
            }

            isDirty = true;
        }

        public static bool ElementEquals(XElement a, XElement b)
        {
            if (a == null || b == null)
            {
                return false;
            }

            var matchCount = 0;
            foreach (var attr in a.Attributes())
            {
                foreach (var other in b.Attributes())
                {
                    if (attr.Value.Equals(other.Value) && attr.Name.Equals(other.Name))
                    {
                        matchCount++;
                        break;
                    }
                }
            }
            var attrMatch = matchCount == a.Attributes().Count();
            var descMatch = a.Descendants().Count() == b.Descendants().Count();
            var childrenMatch = true;
            if (a.HasElements && attrMatch && descMatch)
            {
                if (a.HasElements != b.HasElements)
                {
                    return false;
                }

                var childMatchCount = 0;
                foreach (var child in a.Elements())
                {
                    foreach (var other in b.Elements(child.Name))
                    {
                        if (ElementEquals(child, other))
                        {
                            childMatchCount++;
                            break;
                        }
                    }
                }

                childrenMatch = childMatchCount == a.Elements().Count();
            }

            return attrMatch && descMatch && childrenMatch;
        }
    }
}
