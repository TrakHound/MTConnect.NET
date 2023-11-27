using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// <c>&lt;constrainedElement /&gt;</c> element
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.CONSTRAINED_ELEMENT, Namespace = "")]
    public class ConstrainedElement : XmiElement
    {
        /// <summary>
        /// <c>xmi:idref</c> attribute
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.idRef, Namespace = XmiHelper.XmiNamespace)]
        public string? IdRef { get; set; }
    }
}