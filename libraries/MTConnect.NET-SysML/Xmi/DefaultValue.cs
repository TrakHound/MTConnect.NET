using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// <c>&lt;defaultValue /&gt;</c> element
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.DEFAULT_VALUE, Namespace = "")]
    public class DefaultValue : XmiElement
    {

    }
}