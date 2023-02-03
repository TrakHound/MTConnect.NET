using MTConnect.Assets.RawMaterials;

namespace MTConnect.Applications
{
    internal static partial class Examples
    {
        public static RawMaterialAsset RawMaterialAsset()
        {
            var rawMaterial = new RawMaterialAsset();
            rawMaterial.AssetId = "789456-A2";
            rawMaterial.Timestamp = UnixDateTime.Now;
            rawMaterial.Name = "6061 Aluminum";
            rawMaterial.SerialNumber = "789456-A2";
            rawMaterial.Material = new Material
            {
                Id = "m-al-123456",
                Name = "6061 Aluminum",
                Lot = "B-45",
                Type = "Aluminum"
            };

            return rawMaterial;
        }
    }
}
