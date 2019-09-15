using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace JdSuite.Common.Module
{
    /// <summary>
    /// The basis for all I/O Nodes. For implementation of a node,
    /// implement <see cref="BaseNode"/> instead.
    /// </summary>
    public interface INode : IXmlSerializable
    {
        Guid Identity { get; set; }

        /// <summary>
        /// Determines whether this instance can connect to the specified node.
        /// </summary>
        /// <param name="node">The node to connect to.</param>
        /// <returns>
        ///   <c>true</c> if this instance can connect to the specified node; otherwise, <c>false</c>.
        /// </returns>
        bool CanConnectTo(INode node);

        /// <summary>
        /// Reports if this node connected to another.
        /// </summary>
        /// <returns>
        /// True if the socket is connected.
        /// </returns>
        bool IsConnected();

        /// <summary>
        /// If connected, disconnect from both ends. If this node is not
        /// connected, this command will do nothing.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Retrieves the type of Connection (Input or Output).
        /// </summary>
        /// <returns>The node connection type</returns>
        INodeConnectionType GetConnectionType();

        /// <summary>
        /// Gets the end-point connection if avialable
        /// </summary>
        /// <returns>The connector if avialble, if not, return null</returns>
        INode GetConnector();

        /// <summary>
        /// Gets the display name of the node.
        /// </summary>
        /// <returns>The display name.</returns>
        string GetDisplayName();

        BaseModule GetModule();

        /// <summary>
        /// Retrieves the type of node (Hook or Line).
        /// </summary>
        /// <returns>The node type</returns>
        INodeType GetNodeType();

        /// <summary>
        /// Retrieves a list of extensions supported by this node.
        /// It can be either a file extension or an internal extension.
        /// For example: pdf, xls, csv, dat, Table, GroupByDate
        /// </summary>
        /// <returns>The support extensions</returns>
        List<string> GetSupportedExtensions();
    }
}
