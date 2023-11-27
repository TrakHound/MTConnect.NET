using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// <c>&lt;subsettedProperty /&gt;</c> element
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.SUBSETTED_PROPERTY, Namespace = "")]
    public class SubsettedProperty
    {
        /// <summary>
        /// <c>xmi:idref</c> attribute
        /// </summary>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.idRef, Namespace = XmiHelper.XmiNamespace)]
        public virtual string? IdRef { get; set; }
    }
}