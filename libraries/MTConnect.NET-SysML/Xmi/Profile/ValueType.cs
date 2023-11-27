using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi.Profile
{
    /// <summary>
    /// <c>&lt;Profile:valueType /&gt;</c> element
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.ProfileStructure.VALUE_TYPE, Namespace = XmiHelper.ProfileNamespace)]
    public class ValueType : ProfileElement
    {
        /// <summary>
        /// <c>base_Class</c> attribute.
        /// </summary>
        /// <remarks>Foreign key to the <see cref="XmiElement.Id"/> of the object this applies to.</remarks>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.baseClass, Namespace = "")]
        public string? BaseClass { get; set; }
    }
}