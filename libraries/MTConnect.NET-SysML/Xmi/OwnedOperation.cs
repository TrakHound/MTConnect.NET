using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// <c>&lt;ownedOperation /&gt;</c> element
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.OWNED_OPERATION, Namespace = "")]
    public class OwnedOperation : XmiElement
    {
        /// <summary>
        /// <c>visibility</c> attribute
        /// </summary>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.visibility, Namespace = "")]
        public string? Visibility { get; set; }

        /// <summary>
        /// <c>isQuery</c> attribute
        /// </summary>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.isQuery, Namespace = "")]
        public bool isQuery { get; set; }

        /// <summary>
        /// Collection of <inheritdoc cref="MTConnect.SysML.Xmi.UML.UmlComment"/>
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.OWNED_COMMENT)]
        public OwnedComment[]? Comment { get; set; }

        /// <summary>
        /// Collection of <inheritdoc cref="MTConnect.SysML.Xmi.OwnedParameter"/>
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.OWNED_PARAMETER)]
        public OwnedParameter[]? Parameters { get; set; }
    }
}