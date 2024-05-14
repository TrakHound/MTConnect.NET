using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi.UML
{
    /// <summary>
    /// <inheritdoc cref="MTConnect.SysML.Xmi.PackagedElement"/> where <c>xmi:type='uml:Association'</c>
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.PACKAGED_ELEMENT, Namespace = "")]
    public class UmlAssociation : PackagedElement
    {
        /// <summary>
        /// Collection of <inheritdoc cref="MemberEnd"/>
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.MEMBER_END, Namespace = "")]
        public MemberEnd[]? MemberEnds { get; set; }

        // TODO: Add <ownedEnd xmi:type='uml:Property' />
    }
}