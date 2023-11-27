using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi.UML
{
    /// <summary>
    /// <inheritdoc cref="MTConnect.SysML.Xmi.PackagedElement" /> where <c>xmi:type='uml:DataType'</c>
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.PACKAGED_ELEMENT, Namespace = "")]
    public class UmlDataType : PackagedElement
    {
        /// <inheritdoc cref="MTConnect.SysML.Xmi.XmiElement.Type"/>
        public override string Type => XmiHelper.UmlStructure.DataType;

        /// <summary>
        /// <c>isAbstract</c> attribute
        /// </summary>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.isAbstract, Namespace = "")]
        public bool IsAbstract { get; set; }

        /// <summary>
        /// <c>visibility</c> attribute
        /// </summary>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.visibility, Namespace = "")]
        public string Visibility { get; set; } = "public";

        /// <summary>
        /// Collection of <inheritdoc cref="UmlComment"/>
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.OWNED_COMMENT, Namespace = "")]
        public UmlComment[]? Comments { get; set; }

        /// <summary>
        /// Collection of <inheritdoc cref="UmlGeneralization"/>
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.GENERALIZATION, Namespace = "")]
        public UmlGeneralization[]? Generalization { get; set; }
    }
}