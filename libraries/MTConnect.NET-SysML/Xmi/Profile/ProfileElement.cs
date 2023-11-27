using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi.Profile
{
    /// <summary>
    /// <c>&lt;Profile:x /&gt;</c> element
    /// </summary>
    public abstract class ProfileElement
    {
        /// <summary>
        /// Unique ID within the XMI.
        /// </summary>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.id, Namespace = XmiHelper.XmiNamespace)]
        public virtual string? Id { get; set; }
    }
}