using System;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi.UML
{
    /// <summary>
    /// <inheritdoc cref="MTConnect.SysML.Xmi.OwnedAttribute"/> where <c>xmi:type='uml:Property'</c>
    /// </summary>
    [Serializable, XmlRoot(ElementName = XmiHelper.XmiStructure.OWNED_ATTRIBUTE, Namespace = "")]
    public class UmlProperty : OwnedAttribute
    {
        /// <inheritdoc cref="MTConnect.SysML.Xmi.XmiElement.Type"/>
        public override string Type => XmiHelper.UmlStructure.Property;

        /// <summary>
        /// <c>association</c> attribute
        /// </summary>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.association, Namespace = "")]
        public string? Association { get; set; }

        // TODO: Lookup the uml:Association[@name] to determine the expected Property Name
        // TODO: Figure out how to determine if the associated type is an array. Possibly just a reference to the lowerValue/upperValue elements

        /// <summary>
        /// <c>aggregation</c> attribute
        /// </summary>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.aggregation, Namespace = "")]
        public string? Aggregation { get; set; }

        /// <summary>
        /// <c>type</c> attribute
        /// </summary>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.type, Namespace = "")]
        public string? PropertyType { get; set; }

        /// <summary>
        /// <c>isStatic</c> attribute
        /// </summary>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.isStatic, Namespace = "")]
        public bool IsStatic { get; set; }

        /// <summary>
        /// <c>isReadOnly</c> attribute
        /// </summary>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.isReadOnly, Namespace = "")]
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Child <inheritdoc cref="MTConnect.SysML.Xmi.LowerValue"/>
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.LOWER_VALUE, Namespace = "")]
        public LowerValue? LowerValue { get; set; }

        /// <summary>
        /// Child <inheritdoc cref="MTConnect.SysML.Xmi.DefaultValue"/>
        /// </summary>
        [XmlAnyElement(XmiHelper.XmiStructure.DEFAULT_VALUE, Namespace = "")]
        public XmlElement? DefaultValueElement { get; set; }
        private DefaultValue? _defaultValue;
        public DefaultValue? DefaultValue
        {
            get
            {
                if (_defaultValue != null)
                    return _defaultValue;
                if (DefaultValueElement == null)
                    return null;

                XmlRootAttribute xRoot = new XmlRootAttribute
                {
                    ElementName = XmiHelper.XmiStructure.DEFAULT_VALUE,
                    IsNullable = true,
                    Namespace = ""
                };

                //XmlSerializer serial = new XmlSerializer(typeof(T), xRoot);
                using var xReader = new XmlNodeReader(DefaultValueElement);

                XmlSerializer? serial = null;
                string umlType = DefaultValueElement.GetAttribute(XmiHelper.XmiStructure.type, XmiHelper.XmiNamespace);
                switch (umlType)
                {
                    case XmiHelper.UmlStructure.InstanceValue:
                        serial = new XmlSerializer(typeof(UmlInstanceValue), xRoot);
                        break;
                    case XmiHelper.UmlStructure.LiteralString:
                        serial = new XmlSerializer(typeof(UmlLiteralString), xRoot);
                        break;
                    default:
                        break;
                }

                if (serial != null)
                {
                    object? deserializedObject = serial.Deserialize(xReader);

                    if (deserializedObject != null)
                    {
                        switch (umlType)
                        {
                            case XmiHelper.UmlStructure.InstanceValue:
                                _defaultValue = deserializedObject as UmlInstanceValue;
                                break;
                            case XmiHelper.UmlStructure.LiteralString:
                                _defaultValue = deserializedObject as UmlLiteralString;
                                break;
                            default:
                                break;
                        }
                    }
                }

                return _defaultValue;
            }
        }

        /// <summary>
        /// Child <inheritdoc cref="MTConnect.SysML.Xmi.XmiExtension"/>
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.EXTENSION, Namespace = XmiHelper.XmiNamespace)]
        public XmiExtension? Extension { get; set; }

        /// <summary>
        /// <c>visibility</c> attribute
        /// </summary>
        [XmlAttribute(AttributeName = XmiHelper.XmiStructure.visibility, Namespace = "")]
        public string Visibility { get; set; } = "public";

        /// <summary>
        /// Collection of <inheritdoc cref="MTConnect.SysML.Xmi.UML.UmlComment"/>
        /// </summary>
        [XmlElement(ElementName = XmiHelper.XmiStructure.OWNED_COMMENT, Namespace = "")]
        public UmlComment[]? Comments { get; set; }
    }
}