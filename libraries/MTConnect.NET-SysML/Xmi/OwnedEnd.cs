using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// <c>&lt;ownedEnd /&gt;</c> element
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.OWNED_END, Namespace = "")]
    public class OwnedEnd : XmiElement
    {
        /// <summary>
        /// <c>visibility</c> attribute
        /// </summary>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.visibility, Namespace = "")]
        public string? Visibility { get; set; }

        /// <summary>
        /// <c>aggregation</c> attribute
        /// </summary>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.aggregation, Namespace = "")]
        public string? Aggregation { get; set; }

        /// <summary>
        /// <c>type</c> attribute
        /// </summary>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.type, Namespace = "")]
        public string? TypeId { get; set; }

        /// <summary>
        /// <c>association</c> attribute
        /// </summary>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.association, Namespace = "")]
        public string? Association { get; set; }

        // TODO: Add lowerValue
        // TODO: Add xmi:Extension;
        // TODO: Handle variants of attributes as elements. For example, if type is not an attribute it could be an element.
    }
}