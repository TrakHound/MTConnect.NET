using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// <c>&lt;memberEnd /&gt;</c> element
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.MEMBER_END, Namespace = "")]
    public class MemberEnd : XmiElement
    {
        /// <summary>
        /// <c>xmi:idref</c> attribute
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.idRef, Namespace = XmiHelper.XmiNamespace)]
        public string? IdRef { get; set; }
    }
}