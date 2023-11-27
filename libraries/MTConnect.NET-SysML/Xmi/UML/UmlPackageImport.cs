using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi.UML
{
    /// <summary>
    /// <inheritdoc cref="MTConnect.SysML.Xmi.PackageImport"/> where <c>xmi:type='uml:PackageImport'</c>
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.PACKAGE_IMPORT, Namespace = "")]
    public class UmlPackageImport : PackageImport
    {
        /// <inheritdoc cref="MTConnect.SysML.Xmi.XmiElement.Type"/>
        public override string Type => XmiHelper.UmlStructure.PackageImport;

        /// <summary>
        /// <c>importedPackage</c> attribute
        /// </summary>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.importedPackage, Namespace = "")]
        public string? ImportedPackage { get; set; }
    }
}