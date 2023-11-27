using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi.UML
{
    /// <summary>
    /// <inheritdoc cref="MTConnect.SysML.Xmi.DefaultValue"/> where <c>xmi:type='uml:LiteralString'</c>
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.DEFAULT_VALUE, Namespace = "")]
    public class UmlLiteralString : DefaultValue
    {
        /// <inheritdoc cref="MTConnect.SysML.Xmi.XmiElement.Type"/>
        public override string Type => XmiHelper.UmlStructure.LiteralString;

        /// <summary>
        /// <c>value</c> attribute
        /// </summary>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.value, Namespace = "")]
        public string? Value { get; set; }
    }
}