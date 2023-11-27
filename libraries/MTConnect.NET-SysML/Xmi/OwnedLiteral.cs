using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// <c>&lt;ownedLiteral /&gt;</c> element
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.OWNED_LITERAL, Namespace = "")]
    public class OwnedLiteral : XmiElement { }
}