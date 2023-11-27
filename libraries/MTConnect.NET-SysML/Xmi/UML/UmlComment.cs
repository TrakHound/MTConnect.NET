using System;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi.UML
{
    /// <summary>
    /// <inheritdoc cref="MTConnect.SysML.Xmi.OwnedComment"/> where <c>xmi:type='uml:Comment'</c>
    /// </summary>
    [Serializable, XmlType(Namespace = XmiHelper.UmlNamespace), XmlRoot(ElementName = XmiHelper.XmiStructure.OWNED_COMMENT, Namespace = "")]
    public class UmlComment : OwnedComment
    {
        /// <inheritdoc cref="MTConnect.SysML.Xmi.XmiElement.Type"/>
        public override string Type => XmiHelper.UmlStructure.Comment;

    }
}