using MTConnect.SysML.Models.Devices;

namespace MTConnect.SysML.Json_cppagent
{
    /// <summary>
    /// Scriban-template bag of <see cref="MTConnectDataItemType"/>
    /// entries used by the JSON-cppagent DataItems template.
    /// </summary>
    public class DataItemsModel
    {
        /// <summary>The data-item types to render.</summary>
        public List<MTConnectDataItemType> Types { get; set; } = new();
    }
}
