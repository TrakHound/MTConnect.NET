using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// <c>&lt;generalization /&gt;</c> element
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.GENERALIZATION, Namespace = "")]
    public class Generalization : XmiElement
    {
        // TODO: Add general, general#href
    }
}