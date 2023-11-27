using System;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi.UML
{
    /// <summary>
    /// <inheritdoc cref="MTConnect.SysML.Xmi.PackagedElement"/> where <c>xmi:type='uml:Package'</c>
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.PACKAGED_ELEMENT, Namespace = "")]
    public class UmlPackage : PackagedElement
    {
        /// <summary>
        /// Collection of <inheritdoc cref="MTConnect.SysML.Xmi.UML.UmlComment"/>
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.OWNED_COMMENT, Namespace = "")]
        public UmlComment[]? Comments { get; set; }

        /// <summary>
        /// Represents <c>&lt;packagedElement /&gt;</c> element(s):
        /// <list type="bullet">
        /// <item><c>&lt;packagedElement xmi:type='uml:Enumeration' /&gt;</c></item>
        /// <item><c>&lt;packagedElement xmi:type='uml:DataType' /&gt;</c></item>
        /// <item><c>&lt;packagedElement xmi:type='uml:Class' /&gt;</c></item>
        /// <item><c>&lt;packagedElement xmi:type='uml:Stereotype' /&gt;</c></item>
        /// <item><c>&lt;packagedElement xmi:type='uml:Extension' /&gt;</c></item>
        /// <item><c>&lt;packagedElement xmi:type='uml:Package' /&gt;</c></item>
        /// <item><c>&lt;packagedElement xmi:type='uml:Association' /&gt;</c></item>
        /// <item><c>&lt;packagedElement xmi:type='uml:AssociationClass' /&gt;</c></item>
        /// </list>
        /// </summary>
        [XmlAnyElement(XmiHelper.XmiStructure.PACKAGED_ELEMENT, Namespace = "")]
        public XmlElement[]? PackagedElements { get; set; }

        /// <summary>
        /// Internal switch property for <see cref="Extensions"/>.
        /// </summary>
        [XmlIgnore]
        private PackagedElementCollection<UmlExtension>? _extensions;
        /// <summary>
        /// Collection of <inheritdoc cref="MTConnect.SysML.Xmi.UML.UmlExtension"/>
        /// </summary>
        [XmlIgnore]
        public PackagedElementCollection<UmlExtension> Extensions => _extensions ??= PackagedElementCollection<UmlExtension>.Deserialize(PackagedElements, XmiHelper.UmlStructure.Extension);

        /// <summary>
        /// Internal switch property for <see cref="Stereotypes"/>.
        /// </summary>
        [XmlIgnore]
        private PackagedElementCollection<UmlStereotype>? _stereotypes;
        /// <summary>
        /// Collection of <inheritdoc cref="MTConnect.SysML.Xmi.UML.UmlStereotype"/>
        /// </summary>
        [XmlIgnore]
        public PackagedElementCollection<UmlStereotype> Stereotypes => _stereotypes ??= PackagedElementCollection<UmlStereotype>.Deserialize(PackagedElements, XmiHelper.UmlStructure.Stereotype);

        /// <summary>
        /// Internal switch property for <see cref="Classes"/>.
        /// </summary>
        [XmlIgnore]
        private PackagedElementCollection<UmlClass>? _classes;
        /// <summary>
        /// Collection of <inheritdoc cref="MTConnect.SysML.Xmi.UML.UmlClass"/>
        /// </summary>
        [XmlIgnore]
        public PackagedElementCollection<UmlClass> Classes => _classes ??= PackagedElementCollection<UmlClass>.Deserialize(PackagedElements, XmiHelper.UmlStructure.Class);

        /// <summary>
        /// Internal switch property for <see cref="Enumerations"/>.
        /// </summary>
        [XmlIgnore]
        private PackagedElementCollection<UmlEnumeration>? _enumerations;
        /// <summary>
        /// Collection of <inheritdoc cref="MTConnect.SysML.Xmi.UML.UmlEnumeration"/>
        /// </summary>
        [XmlIgnore]
        public PackagedElementCollection<UmlEnumeration> Enumerations => _enumerations ??= PackagedElementCollection<UmlEnumeration>.Deserialize(PackagedElements, XmiHelper.UmlStructure.Enumeration);

        /// <summary>
        /// Internal switch property for <see cref="Packages"/>.
        /// </summary>
        [XmlIgnore]
        private PackagedElementCollection<UmlPackage>? _packages;
        /// <summary>
        /// Collection of <inheritdoc cref="MTConnect.SysML.Xmi.UML.UmlPackage"/>
        /// </summary>
        [XmlIgnore]
        public PackagedElementCollection<UmlPackage> Packages => _packages ??= PackagedElementCollection<UmlPackage>.Deserialize(PackagedElements, XmiHelper.UmlStructure.Package);

        /// <summary>
        /// Internal switch property for <see cref="DataTypes"/>.
        /// </summary>
        [XmlIgnore]
        private PackagedElementCollection<UmlDataType>? _dataTypes;
        /// <summary>
        /// Collection of <inheritdoc cref="MTConnect.SysML.Xmi.UML.UmlDataType"/>
        /// </summary>
        [XmlIgnore]
        public PackagedElementCollection<UmlDataType> DataTypes => _dataTypes ??= PackagedElementCollection<UmlDataType>.Deserialize(PackagedElements, XmiHelper.UmlStructure.DataType);

        /// <summary>
        /// Internal switch property for <see cref="Associations"/>.
        /// </summary>
        [XmlIgnore]
        private PackagedElementCollection<UmlAssociation>? _associations;
        /// <summary>
        /// Collection of <inheritdoc cref="MTConnect.SysML.Xmi.UML.UmlAssociation"/>
        /// </summary>
        [XmlIgnore]
        public PackagedElementCollection<UmlAssociation> Associations => _associations ??= PackagedElementCollection<UmlAssociation>.Deserialize(PackagedElements, XmiHelper.UmlStructure.Association);

        /// <summary>
        /// Internal switch property for <see cref="AssociationClasses"/>.
        /// </summary>
        [XmlIgnore]
        private PackagedElementCollection<UmlAssociationClass>? _associationClasses;
        /// <summary>
        /// Collection of <inheritdoc cref="MTConnect.SysML.Xmi.UML.UmlAssociationClass"/>
        /// </summary>
        [XmlIgnore]
        public PackagedElementCollection<UmlAssociationClass> AssociationClasses => _associationClasses ??= PackagedElementCollection<UmlAssociationClass>.Deserialize(PackagedElements, XmiHelper.UmlStructure.AssociationClass);
        //// TODO: Add xmi:Extension

    }
}