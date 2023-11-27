using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// <c>&lt;xmi:Documentation /&gt;</c> element
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.DOCUMENTATION, Namespace = XmiHelper.XmiNamespace)]
    public class XmiDocumentation
    {
        /// <summary>
        /// Child <inheritdoc cref="XmiExporter"/>
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.EXPORTER, Namespace = XmiHelper.XmiNamespace)]
        public XmiExporter? Exporter { get; set; }

        /// <summary>
        /// Child <inheritdoc cref="XmiExporterVersion"/>
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.EXPORTER_VERSION, Namespace = XmiHelper.XmiNamespace)]
        public XmiExporterVersion? ExporterVersion { get; set; }
    }
}