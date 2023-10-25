using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// <c>&lt;annotatedElement /&gt;</c> element
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.ANNOTATED_ELEMENT, Namespace = "")]
    public class AnnotatedElement
    {
        /// <summary>
        /// <c>xmi:idref</c> attribute
        /// </summary>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.idRef, Namespace = XmiHelper.XmiNamespace)]
        public string? IdRef { get; set; }
    }
}