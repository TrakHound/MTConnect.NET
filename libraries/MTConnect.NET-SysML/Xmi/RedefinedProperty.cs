using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// <c>&lt;redefinedProperty /&gt;</c> element
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.REDEFINED_PROPERTY, Namespace = "")]
    public class RedefinedProperty
    {
        /// <summary>
        /// <c>xmi:idref</c> attribute
        /// </summary>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.idRef, Namespace = XmiHelper.XmiNamespace)]
        public virtual string? IdRef { get; set; }
    }
}