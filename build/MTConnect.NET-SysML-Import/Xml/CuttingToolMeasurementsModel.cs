using MTConnect.SysML.Models.Assets;

namespace MTConnect.SysML.Xml
{
    /// <summary>
    /// Scriban-template bag of <see cref="MTConnectMeasurementModel"/>
    /// entries used by the XML CuttingToolMeasurements template.
    /// </summary>
    public class CuttingToolMeasurementsModel
    {
        /// <summary>The measurement models to render.</summary>
        public List<MTConnectMeasurementModel> Types { get; set; } = new();
    }
}
