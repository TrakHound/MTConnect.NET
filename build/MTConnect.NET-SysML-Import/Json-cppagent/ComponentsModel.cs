using MTConnect.SysML.Models.Devices;

namespace MTConnect.SysML.Json_cppagent
{
    /// <summary>
    /// Scriban-template bag of <see cref="MTConnectComponentType"/>
    /// entries used by the JSON-cppagent Components template.
    /// </summary>
    public class ComponentsModel
    {
        /// <summary>The component types to render.</summary>
        public List<MTConnectComponentType> Types { get; set; } = new();
    }
}
