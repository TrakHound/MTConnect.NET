namespace MTConnect.SysML
{
    /// <summary>
    /// A parsed MTConnect model element that can be rendered into generated
    /// C# by the Scriban templates.
    /// </summary>
    public interface IMTConnectExportModel
    {
        /// <summary>
        /// The <c>xmi:id</c> of the source XMI element this model was parsed
        /// from.
        /// </summary>
        string UmlId { get; }

        /// <summary>
        /// The dotted MTConnect model identifier (for example
        /// <c>Devices.Component</c>) used to key and resolve the element.
        /// </summary>
        string Id { get; }
    }
}
