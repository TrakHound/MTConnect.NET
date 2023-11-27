using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// <c>&lt;xmi:Extension /&gt;</c> element
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.EXTENSION, Namespace = XmiHelper.XmiNamespace)]
    public class XmiExtension : XmiElement
    {
        /// <summary>
        /// <c>extender</c> attribute
        /// </summary>
        [XmlAttribute(XmiHelper.XmiStructure.extender, Namespace = "")]
        public string? Extender { get; set; }
    }
}