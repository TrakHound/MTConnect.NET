using MTConnect.SysML.Xmi.UML;
using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// <c>&lt;ownedComment /&gt;</c> element
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.OWNED_COMMENT, Namespace = "")]
    [XmlInclude(typeof(UmlComment))]
    public class OwnedComment : XmiElement {
        /// <summary>
        /// <c>body</c> attribute
        /// </summary>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.BODY, Namespace = "")]
        public string? Body { get; set; }

        /// <summary>
        /// Child <inheritdoc cref="MTConnect.SysML.Xmi.OwnedComment"/>
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.OWNED_COMMENT, Namespace = "")]
        public OwnedComment? SubComment { get; set; }

        /// <summary>
        /// Child <inheritdoc cref="MTConnect.SysML.Xmi.AnnotatedElement"/>
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.ANNOTATED_ELEMENT, Namespace = "")]
        public AnnotatedElement? AnnotatedElement { get; set; }
    }
}