using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// <c>&lt;lowerValue /&gt;</c> element
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.LOWER_VALUE, Namespace = "")]
    public class LowerValue : XmiElement
    {
        /// <summary>
        /// <c>value</c> attribute
        /// </summary>
        [XmlAttribute(XmiHelper.XmiStructure.value, Namespace = "")]
        public string? Value { get; set; }
    }
}