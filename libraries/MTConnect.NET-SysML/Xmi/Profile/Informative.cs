using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi.Profile
{
    /// <summary>
    /// <c>&lt;Profile:informative /&gt;</c> element
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.ProfileStructure.INFORMATIVE, Namespace = XmiHelper.ProfileNamespace)]
    public class Informative : ProfileElement
    {
        /// <summary>
        /// <c>base_Element</c> attribute.
        /// </summary>
        /// <remarks>Foreign key to the <see cref="XmiElement.Id"/> of the object this applies to.</remarks>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.baseElement, Namespace = "")]
        public string? BaseElement { get; set; }
    }
}