using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi.UML
{
    /// <summary>
    /// <inheritdoc cref="MTConnect.SysML.Xmi.PackagedElement" /> where <c>xmi:type='uml:Enumeration'</c>
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.PACKAGED_ELEMENT, Namespace = "")]
    public class UmlEnumeration : PackagedElement
    {
        /// <inheritdoc cref="MTConnect.SysML.Xmi.XmiElement.Type"/>
        public override string Type => XmiHelper.UmlStructure.Enumeration;

        /// <summary>
        /// Collection of <inheritdoc cref="UmlEnumerationLiteral"/>
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.OWNED_LITERAL, Namespace = "")]
        public UmlEnumerationLiteral[]? Items { get; set; }

        /// <summary>
        /// Collection of <inheritdoc cref="UmlGeneralization"/>
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.GENERALIZATION, Namespace = "")]
        public UmlGeneralization[]? Generalization { get; set; }
    }
}