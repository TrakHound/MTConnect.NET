using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi.UML
{
    /// <summary>
    /// <inheritdoc cref="MTConnect.SysML.Xmi.DefaultValue"/> where <c>xmi:type='uml:InstanceValue'</c>
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.DEFAULT_VALUE, Namespace = "")]
    public class UmlInstanceValue : DefaultValue
    {
        /// <inheritdoc cref="MTConnect.SysML.Xmi.XmiElement.Type"/>
        public override string Type => XmiHelper.UmlStructure.InstanceValue;

        /// <summary>
        /// <c>instance</c> attribute
        /// </summary>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.instance, Namespace = "")]
        public string? Instance { get; set; }
    }
}