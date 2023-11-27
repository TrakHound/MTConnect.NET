using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// <c>&lt;upperValue /&gt;</c> element
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.UPPER_VALUE, Namespace = "")]
    public class UpperValue : XmiElement
    {
        /// <summary>
        /// <c>value</c> attribute
        /// </summary>
        [XmlAttribute(XmiHelper.XmiStructure.value, Namespace = "")]
        public string? Value { get; set; }
    }
}