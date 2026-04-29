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
        // The `general` attribute (xmi:id of the parent class) is deserialized
        // on <see cref="MTConnect.SysML.Xmi.UML.UmlGeneralization.General"/>,
        // which is the concrete subclass used for `xmi:type='uml:Generalization'`.
    }
}