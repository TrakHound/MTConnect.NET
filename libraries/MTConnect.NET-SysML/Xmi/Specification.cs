using MTConnect.SysML.Xmi.UML;
using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// <c>&lt;specification /&gt;</c> element
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.SPECIFICATION, Namespace = "")]
    [XmlInclude(typeof(UmlOpaqueExpression))]
    public class Specification : XmiElement
    {
        /// <summary>
        /// <c>body</c> attribute
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.BODY, Namespace = "")]
        public string? Body { get; set; }

        /// <summary>
        /// <c>language</c> attribute
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.LANGUAGE, Namespace = "")]
        public string? Language { get; set; }
    }
}