using System;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace MTConnect.SysML.Xmi
{
    /// <summary>
    /// A class that can deserialize a XMI document into an object-oriented form.
    /// </summary>
    public sealed class XmiDeserializer
    {
        private XmlDocument xDoc;
        private XmlNamespaceManager nsmgr;

        /// <summary>
        /// Constructs a new instance of the deserializer with a reference to the source document.
        /// </summary>
        /// <param name="xmlDocument">A source of XMI to deserialize</param>
        /// <param name="logger"><inheritdoc cref="ILogger" path="/summary"/></param>
        public XmiDeserializer(XmlDocument xmlDocument)
        {
            xDoc = xmlDocument;
            nsmgr = new XmlNamespaceManager(xDoc.NameTable);
            nsmgr.AddNamespace("xmi", XmiHelper.XmiNamespace);
            nsmgr.AddNamespace("uml", XmiHelper.UmlNamespace);
            nsmgr.AddNamespace("Profile", XmiHelper.ProfileNamespace);
            nsmgr.AddNamespace("StandardProfile", XmiHelper.StandardProfileNamespace);
            nsmgr.AddNamespace("Validation_Profile", XmiHelper.Validation_ProfileNamespace);
            nsmgr.AddNamespace("Dependency_Matrix_Profile", XmiHelper.Dependency_Matrix_ProfileNamespace);
            nsmgr.AddNamespace("Concept_Modeling_Profile", XmiHelper.Concept_Modeling_ProfileNamespace);
            nsmgr.AddNamespace("DSL_Customization", XmiHelper.DSL_CustomizationNamespace);
            nsmgr.AddNamespace("sysml", XmiHelper.SysMlNamespace);
            nsmgr.AddNamespace("MagicDraw_Profile", XmiHelper.MagicDraw_ProfileNamespace);
            nsmgr.AddNamespace("CCM_Internal_Implementation_Profile", XmiHelper.Ccm_Internal_Implementation_ProfileNamespace);
            nsmgr.AddNamespace("MD_Customization_for_SysML__additional_stereotypes", XmiHelper.Md_Customization_for_SysML__additional_stereotypesNamespace);
            nsmgr.AddNamespace("SimulationProfile", XmiHelper.SimulationProfileNamespace);
        }

        /// <summary>
        /// Deserializes the XML Document into the specified type.
        /// </summary>
        /// <returns>The deserialized object as a <see cref="XmiDocument"/>.</returns>
        public XmiDocument? Deserialize(CancellationToken cancellationToken)
        {
            // Honour the cancellation token at the entry point and again after
            // the (synchronous, but potentially slow) XmlSerializer construction
            // so callers can abort between cooperative checkpoints (row 18).
            cancellationToken.ThrowIfCancellationRequested();

            // Guard a malformed / empty input. `xDoc.DocumentElement` is null
            // for an XmlDocument that loaded a fragment with no root element;
            // dereferencing `.LocalName` would NRE (row 17).
            if (xDoc.DocumentElement == null)
                throw new InvalidOperationException("XMI document has no root element; nothing to deserialize.");

            XmiDocument? result = null;

            XmlRootAttribute xRoot = new XmlRootAttribute();
            xRoot.ElementName = xDoc.DocumentElement.LocalName;
            xRoot.IsNullable = true;
            xRoot.Namespace = XmiHelper.XmiNamespace;
            XmlSerializer serial = new XmlSerializer(typeof(Xmi.XmiDocument), xRoot);

            cancellationToken.ThrowIfCancellationRequested();

            // Deserialize the XmlNode
            using (XmlNodeReader xReader = new XmlNodeReader(xDoc.DocumentElement))
            {
                object? deserializedObject = serial.Deserialize(xReader);

                result = deserializedObject as XmiDocument;

            }

            return result;
        }

        /// <summary>
        /// Creates a <see cref="XmiDeserializer"/> from a reference to the filepath of a XMI document.
        /// </summary>
        /// <param name="filename">Filepath to a XMI-formatted XML document.</param>
        /// <param name="logger"><inheritdoc cref="ILogger" path="/summary"/></param>
        /// <returns><inheritdoc cref="XmiDeserializer" path="/summary"/></returns>
        public static XmiDeserializer FromFile(string filename)
        {
            var xDoc = new XmlDocument();
            // Defence-in-depth: .NET 6+ defaults `XmlResolver` to null and
            // disables DTD processing, but pinning both via XmlReaderSettings
            // survives a future framework downgrade or accidental restoration
            // of XmlUrlResolver. Refuses billion-laughs DoS and external
            // entity resolution. See OWASP "XML External Entities (XXE)" (row 51).
            xDoc.XmlResolver = null;
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
            };
            using var reader = XmlReader.Create(filename, settings);
            xDoc.Load(reader);

            return new XmiDeserializer(xDoc);
        }

        /// <summary>
        /// Creates a <see cref="XmiDeserializer"/> from raw XML.
        /// </summary>
        /// <param name="xml">Raw XML string</param>
        /// <param name="logger"><inheritdoc cref="ILogger" path="/summary"/></param>
        /// <returns><inheritdoc cref="XmiDeserializer" path="/summary"/></returns>
        public static XmiDeserializer FromXml(string xml)
        {
            var xDoc = new XmlDocument();
            // See FromFile for rationale (row 51).
            xDoc.XmlResolver = null;
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
            };
            using var stringReader = new StringReader(xml);
            using var reader = XmlReader.Create(stringReader, settings);
            xDoc.Load(reader);

            return new XmiDeserializer(xDoc);
        }
    }
}
