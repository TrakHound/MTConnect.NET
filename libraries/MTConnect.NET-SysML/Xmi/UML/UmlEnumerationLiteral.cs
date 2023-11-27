using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi.UML
{
    /// <summary>
    /// <inheritdoc cref="MTConnect.SysML.Xmi.OwnedLiteral" /> where <c>xmi:type='uml:EnumerationLiteral'</c>
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.OWNED_LITERAL, Namespace = "")]
    public class UmlEnumerationLiteral : OwnedLiteral
    {
        /// <inheritdoc cref="MTConnect.SysML.Xmi.XmiElement.Type"/>
        public override string Type => XmiHelper.UmlStructure.EnumerationLiteral;

        /// <summary>
        /// Collection of <inheritdoc cref="MTConnect.SysML.Xmi.UML.UmlComment"/>
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.OWNED_COMMENT, Namespace = "")]
        public UmlComment[]? Comments { get; set; }
    }
}