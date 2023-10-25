using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// <c>&lt;metamodelReference /&gt;</c> element
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.METAMODEL_REFERENCE, Namespace = "")]
    public class MetamodelReference : XmiElement
    {
        /// <summary>
        /// <c>xmi:idref</c> attribute
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.idRef, Namespace = XmiHelper.XmiNamespace)]
        public string? IdRef { get; set; }
    }
}