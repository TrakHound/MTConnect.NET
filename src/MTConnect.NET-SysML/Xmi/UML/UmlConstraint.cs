using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi.UML
{
    /// <summary>
    /// <inheritdoc cref="MTConnect.SysML.Xmi.OwnedRule"/> where <c>xmi:type='uml:Constraint'</c>
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.OWNED_RULE, Namespace = "")]
    public class UmlConstraint : OwnedRule
    {
        /// <summary>
        /// Child <inheritdoc cref="MTConnect.SysML.Xmi.ConstrainedElement"/>
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.CONSTRAINED_ELEMENT, Namespace = "")]
        public ConstrainedElement? ConstrainedElement { get; set; }
    }
}