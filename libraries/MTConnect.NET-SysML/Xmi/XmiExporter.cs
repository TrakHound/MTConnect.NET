using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// <c>&lt;xmi:exporter /&gt;</c> element
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.EXPORTER, Namespace = XmiHelper.XmiNamespace)]
    public class XmiExporter
    {
        /// <summary>
        /// <c>value</c> of the element
        /// </summary>
        [XmlText]
        public string? Value { get; set; }
    }
}