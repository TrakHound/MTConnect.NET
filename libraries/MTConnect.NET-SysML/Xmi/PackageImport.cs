using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// <c>&lt;packageImport /&gt;</c> element
    /// </summary>
    [Serializable, XmlType(AnonymousType = true, Namespace = ""), XmlRoot(ElementName = XmiHelper.XmiStructure.PACKAGE_IMPORT, Namespace = "")]
    public class PackageImport : XmiElement
    {
        // TODO: Add importedPackage
    }
}