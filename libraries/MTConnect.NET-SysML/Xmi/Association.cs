using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// <c>&lt;assocation /&gt;</c> element
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.ASSOCIATION, Namespace = "")]
    public class Association
    {
        /// <summary>
        /// <c>href</c> attribute
        /// </summary>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.href, Namespace = "")]
        public string? Href { get; set; }
    }
}