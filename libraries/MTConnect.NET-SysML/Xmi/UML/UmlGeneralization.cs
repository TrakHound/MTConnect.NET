using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi.UML
{
    /// <summary>
    /// <inheritdoc cref="MTConnect.SysML.Xmi.Generalization"/> where <c>xmi:type='uml:Generalization'</c>
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.GENERALIZATION, Namespace = "")]
    public class UmlGeneralization : Generalization
    {
        /// <inheritdoc cref="MTConnect.SysML.Xmi.XmiElement.Type"/>
        public override string Type => XmiHelper.UmlStructure.Generalization;

        /// <summary>
        /// <c>general</c> attribute
        /// </summary>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.GENERAL, Namespace = "")]
        public string? General { get; set; }
    }
}