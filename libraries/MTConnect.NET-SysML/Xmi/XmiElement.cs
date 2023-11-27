using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// Abstract model for common <c>xmi</c> elements
    /// </summary>
    public abstract class XmiElement : IXmiElement
    {
        /// <summary>
        /// <c>xmi:id</c> attribute
        /// </summary>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.id, Namespace = XmiHelper.XmiNamespace)]
        public virtual string? Id { get; set; }

        /// <summary>
        /// <c>name</c> attribute
        /// </summary>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.name, Namespace = "")]
        public virtual string? Name { get; set; }

        /// <summary>
        /// <c>xmi:type</c> attribute
        /// </summary>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.type, Namespace = XmiHelper.XmiNamespace)]
        public virtual string? Type { get; set; }
    }
}