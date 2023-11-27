using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// <c>&lt;type /&gt;</c> element
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.TYPE, Namespace = "")]
    public class Type : XmiElement
    {
        /// <summary>
        /// <c>href</c> attribute
        /// </summary>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.href, Namespace = "")]
        public string? Href { get; set; }
    }
}