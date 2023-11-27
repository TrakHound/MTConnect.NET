using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// <c>&lt;ownedRule /&gt;</c> element
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.OWNED_RULE, Namespace = "")]
    public class OwnedRule : XmiElement
    {
        /// <summary>
        /// Child <inheritdoc cref="MTConnect.SysML.Xmi.Specification"/>
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.SPECIFICATION, Namespace = "")]
        public Specification? Specification { get; set; }
    }
}