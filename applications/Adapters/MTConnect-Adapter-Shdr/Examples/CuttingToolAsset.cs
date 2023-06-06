using MTConnect.Assets.CuttingTools;
using MTConnect.Assets.CuttingTools.Measurements;

namespace MTConnect.Applications
{
    internal static partial class Examples
    {
        public static CuttingToolAsset CuttingTool()
        {
            var tool = new CuttingToolAsset();
            tool.SerialNumber = "12345678946";
            tool.AssetId = "5.12";
            tool.ToolId = "12";
            tool.CuttingToolLifeCycle = new CuttingToolLifeCycle
            {
                Location = new Location 
                { 
                    Type = LocationType.SPINDLE
                },
                ProgramToolNumber = "12",
                ProgramToolGroup = "5"
            };
            tool.CuttingToolLifeCycle.Measurements.Add(new FunctionalLengthMeasurement(7.6543));
            tool.CuttingToolLifeCycle.Measurements.Add(new CuttingDiameterMaxMeasurement(0.375));
            tool.CuttingToolLifeCycle.CuttingItems.Add(new CuttingItem
            {
                ItemId = "12.1",
                Indices = "1",
                Locus = CuttingItemLocas.FLUTE.ToString()
            });
            tool.CuttingToolLifeCycle.CutterStatus.Add(CutterStatus.AVAILABLE);
            tool.CuttingToolLifeCycle.CutterStatus.Add(CutterStatus.NEW);
            tool.CuttingToolLifeCycle.CutterStatus.Add(CutterStatus.MEASURED);
            tool.CuttingToolLifeCycle.ToolLife = new ToolLife();
            tool.Timestamp = UnixDateTime.Now;

            return tool;
        }
    }
}
