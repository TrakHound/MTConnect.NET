using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi.UML
{
    /// <summary>
    /// <inheritdoc cref="MTConnect.SysML.Xmi.PackagedElement"/> where <c>xmi:type='uml:PrimitiveType'</c>
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.PACKAGED_ELEMENT, Namespace = "")]
    public class UmlPrimitiveType : PackagedElement
    {
        /// <inheritdoc cref="MTConnect.SysML.Xmi.XmiElement.Type"/>
        public override string Type => XmiHelper.UmlStructure.PrimitiveType;
    }
}