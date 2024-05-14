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
            XmiDocument? result = null;

            XmlRootAttribute xRoot = new XmlRootAttribute();
            xRoot.ElementName = xDoc.DocumentElement.LocalName;
            xRoot.IsNullable = true;
            xRoot.Namespace = XmiHelper.XmiNamespace;
            XmlSerializer serial = new XmlSerializer(typeof(Xmi.XmiDocument), xRoot);
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
            xDoc.Load(filename);

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
            xDoc.LoadXml(xml);

            return new XmiDeserializer(xDoc);
        }
    }
}
