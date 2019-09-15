using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace JdSuite.Common.Module
{
    /// <summary>
	/// The basis for all external modules. For implementation of a module,
	/// implement <see cref="BaseModule"/> instead.
	/// </summary>
	public interface IModule : IXmlSerializable
    {
        /// <summary>
        /// Disconnects this module from all connected nodes. Does nothing
        /// if there are no active connections.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Execute a representation of the data for this module.
        /// The return object should be in a human-readable file
        /// format (string), or at least a file format readable by a program capable of reading
        /// the required file extension. A module may return null if it wishes
        /// to block and provide it's own human-readable interface.
        /// </summary>
        /// <param name="workflow">The current *.flo file.</param>
        /// <returns>The representation of this module's data.
        /// If it is not readily= available, this should return null.</returns>
        object Execute(Workflow workflow);

        /// <summary>
        /// Retrieves the name of the current module for display purposes
        /// </summary>
        /// <returns>The name of the current module</returns>
        string DisplayName { get; set; }

        /// <summary>
        /// If this is a module that outputs an end-product, this function
        /// will get the extension for that product.
        /// </summary>
        /// <returns>The extension if this is a module without output nodes.
        /// If this module has output nodes, this should return null.</returns>
        string GetExtension();

        /// <summary>
        /// Retrieves a list of input nodes for this module
        /// </summary>
        /// <returns>The input nodes</returns>
        List<BaseInputNode> GetInputNodes();

        /// <summary>
        /// Gets the major version for a module.
        /// </summary>
        int GetMajorVersion();

        /// <summary>
        /// Gets the minor version for a module.
        /// </summary>
        int GetMinorVersion();

        /// <summary>
        /// Retrieves a list of output nodes for this module
        /// </summary>
        /// <returns>The output nodes</returns>
        List<BaseOutputNode> GetOutputNodes();

        /// <summary>
        /// Gets the patch version for a module.
        /// </summary>
        int GetPatchVersion();

        /// <summary>
        /// Gets the human-readable description of this module's use for
        /// display in a front-end tooltip.
        /// </summary>
        string GetToolTipDescription();

        /// <summary>
        /// Determines whether this module has active connections.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this module has active connections; otherwise, <c>false</c>.
        /// </returns>
        bool HasConnections();

        ///// <summary>
        ///// Opens the resource that this node is attached to. Usually called
        ///// by the input node when it is ready to access the resource.
        ///// The stream's lifetime must be managed by the caller.
        ///// </summary>
        ///// <returns>
        ///// A System.IO.Stream containing data to be read, or null if one
        ///// is not available
        ///// </returns>
        //Stream OpenResource();

        /// <summary>
        /// Retrieves the Bitmap icon of the current module relative to the
        /// IconDirectory provided in the App.config file for this application.
        /// </summary>
        /// <returns>The Bitmap icon of the current module</returns>
        BitmapImage RetrieveBitmapIcon();



        void SetContextMenuItems(ContextMenu ctxMenu);


        /// <summary>
        /// The unique identifier for this instance.
        /// </summary>
        Guid Guid { get; }
        Border ModulePageBorder { get; set; }
    }
}
