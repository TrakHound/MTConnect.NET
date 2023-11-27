using MTConnect.SysML.Xmi.Profile;
using MTConnect.SysML.Xmi.UML;
using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// <c>xmi:XMI</c> element
    /// </summary>
    [Serializable, XmlRoot(ElementName = "XMI", Namespace = XmiHelper.XmiNamespace)]
    public class XmiDocument
    {
        /// <summary>
        /// Child <inheritdoc cref="MTConnect.SysML.Xmi.XmiDocumentation"/>
        /// </summary>
        public XmiDocumentation? Documentation { get; set; }

        /// <summary>
        /// Child <inheritdoc cref="MTConnect.SysML.Xmi.UML.UmlModel"/>
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.MODEL, Namespace = XmiHelper.UmlNamespace)]
        public UmlModel? Model { get; set; }

        /// <summary>
        /// Collection of <inheritdoc cref="MTConnect.SysML.Xmi.Profile.Normative"/>
        /// </summary>
        [XmlElement(ElementName = XmiHelper.ProfileStructure.NORMATIVE, Namespace = XmiHelper.ProfileNamespace)]
        public Normative[]? NormativeIntroductions { get; set; }

        /// <summary>
        /// Collection of <inheritdoc cref="MTConnect.SysML.Xmi.Profile.Deprecated"/>
        /// </summary>
        [XmlElement(ElementName = XmiHelper.ProfileStructure.DEPRECATED, Namespace = XmiHelper.ProfileNamespace)]
        public Deprecated[]? Deprecations { get; set; }
    }
}