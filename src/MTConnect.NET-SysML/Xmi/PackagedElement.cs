using MTConnect.SysML.Xmi.UML;
using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// <c>&lt;packagedElement /&gt;</c> element
    /// </summary>
    [Serializable, XmlType(Namespace = XmiHelper.UmlNamespace), XmlRoot(ElementName = XmiHelper.XmiStructure.PACKAGED_ELEMENT, Namespace = XmiHelper.UmlNamespace)]
    [XmlInclude(typeof(UmlPackage)),
        XmlInclude(typeof(UmlProfile)),
        XmlInclude(typeof(UmlClass)),
        XmlInclude(typeof(UmlEnumeration)),
        XmlInclude(typeof(UmlExtension)),
        XmlInclude(typeof(UmlPrimitiveType)),
        XmlInclude(typeof(UmlStereotype))]
    public class PackagedElement : XmiElement
    {

    }
}