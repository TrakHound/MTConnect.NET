using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// <c>&lt;ownedParameter /&gt;</c> element
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.OWNED_PARAMETER, Namespace = "")]
    public class OwnedParameter : XmiElement
    {
        /// <summary>
        /// Collection of <inheritdoc cref="MTConnect.SysML.Xmi.OwnedComment"/>
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.OWNED_COMMENT, Namespace = "")]
        public OwnedComment[]? Comments { get; set; }

        /// <summary>
        /// Child <inheritdoc cref="MTConnect.SysML.Xmi.DefaultValue"/>
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.DEFAULT_VALUE, Namespace = "")]
        public DefaultValue? DefaultValue { get; set; }
    }
}