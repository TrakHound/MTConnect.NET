using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// <c>&lt;modelExtension /&gt;</c> element
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.MODEL_EXTENSION, Namespace = "")]
    public class ModelExtension
    {
        /// <summary>
        /// Child <inheritdoc cref="MTConnect.SysML.Xmi.UpperValue"/>
        /// </summary>
        [XmlElement(XmiHelper.XmiStructure.UPPER_VALUE, Namespace = "")]
        public UpperValue? UpperValue { get; set; }

        /// <summary>
        /// Child <inheritdoc cref="MTConnect.SysML.Xmi.LowerValue"/>
        /// </summary>
        [XmlElement(XmiHelper.XmiStructure.LOWER_VALUE, Namespace = "")]
        public LowerValue? LowerValue { get; set; }
    }
}