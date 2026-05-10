using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi.UML
{
    /// <summary>
    /// <inheritdoc cref="MTConnect.SysML.Xmi.PackagedElement"/> where <c>xmi:type='uml:Class'</c>
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.PACKAGED_ELEMENT, Namespace = "")]
    public class UmlClass : PackagedElement
    {
        /// <inheritdoc cref="MTConnect.SysML.Xmi.XmiElement.Type"/>
        public override string Type => XmiHelper.UmlStructure.Class;

        /// <summary>
        /// Collection of <inheritdoc cref="MTConnect.SysML.Xmi.UML.UmlComment"/>
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.OWNED_COMMENT, Namespace = "")]
        public UmlComment[]? Comments { get; set; }

        /// <summary>
        /// Collection of <inheritdoc cref="MTConnect.SysML.Xmi.UML.UmlProperty"/>
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.OWNED_ATTRIBUTE, Namespace = "")]
        public UmlProperty[]? Properties { get; set; }

        /// <summary>
        /// <c>isAbstract</c> attribute
        /// </summary>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.isAbstract, Namespace = "")]
        public bool IsAbstract { get; set; }

        /// <summary>
        /// Collection of <inheritdoc cref="MTConnect.SysML.Xmi.UML.UmlConstraint"/>
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.OWNED_RULE)]
        public UmlConstraint[]? Constraints { get; set; }

        /// <summary>
        /// Collection of <inheritdoc cref="MTConnect.SysML.Xmi.UML.UmlGeneralization"/>.
        /// A SysML class may declare more than one <c>&lt;generalization&gt;</c>
        /// element (multi-inheritance in UML). The XMI parser must capture
        /// every entry so downstream models can decide which becomes the
        /// primary C# class base and which become marker interfaces.
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.GENERALIZATION, Namespace = "")]
        public UmlGeneralization[]? Generalizations { get; set; }
    }
}