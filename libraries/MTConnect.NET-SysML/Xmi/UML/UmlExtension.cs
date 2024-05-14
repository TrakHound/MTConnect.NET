using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi.UML
{
    /// <summary>
    /// <inheritdoc cref="MTConnect.SysML.Xmi.PackagedElement"/> where <c>xmi:type='uml:Extension'</c>
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.PACKAGED_ELEMENT, Namespace = "")]
    public class UmlExtension : PackagedElement
    {
        /// <inheritdoc cref="MTConnect.SysML.Xmi.XmiElement.Type"/>
        public override string Type => XmiHelper.UmlStructure.Extension;

        /// <summary>
        /// Collection of <inheritdoc cref="MTConnect.SysML.Xmi.MemberEnd"/>
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.MEMBER_END, Namespace = "")]
        public MemberEnd[]? MemberEnds { get; set; }

        /// <summary>
        /// Child <inheritdoc cref="MTConnect.SysML.Xmi.UML.UmlExtensionEnd"/>
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.OWNED_END, Namespace = "")]
        public UmlExtensionEnd? End { get; set; }

    }
}