using MTConnect.SysML.Models.Assets;

namespace MTConnect.SysML.Json_cppagent
{
    /// <summary>
    /// Scriban-template bag of <see cref="MTConnectMeasurementModel"/>
    /// entries used by the JSON-cppagent CuttingToolMeasurements
    /// template.
    /// </summary>
    public class CuttingToolMeasurementsModel
    {
        /// <summary>The measurement models to render.</summary>
        public List<MTConnectMeasurementModel> Types { get; set; } = new();
    }
}
