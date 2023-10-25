using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// <c>&lt;xmi:exporterVersion /&gt;</c> element
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.EXPORTER_VERSION, Namespace = XmiHelper.XmiNamespace)]
    public class XmiExporterVersion
    {
        /// <summary>
        /// <c>value</c> of the element
        /// </summary>
        [XmlText]
        public string? Value { get; set; }
    }
}