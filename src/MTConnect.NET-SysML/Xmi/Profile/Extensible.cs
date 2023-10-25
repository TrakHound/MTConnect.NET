using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi.Profile
{
    /// <summary>
    /// <c>&lt;Profile:extensible /&gt;</c> element
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.ProfileStructure.EXTENSIBLE, Namespace = XmiHelper.ProfileNamespace)]
    public class Extensible : ProfileElement
    {
        /// <summary>
        /// <c>base_Enumeration</c> attribute.
        /// </summary>
        /// <remarks>Foreign key to the <see cref="XmiElement.Id"/> of the object this applies to.</remarks>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.baseEnumeration, Namespace = "")]
        public string? BaseEnumeration { get; set; }
    }
}