using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;

namespace MTConnect.SysML.Models.Assets
{
    /// <summary>
    /// Parsed model for the MTConnect <c>Assets.PhysicalAsset</c> element,
    /// emitted as a concrete root type.
    /// </summary>
    public class MTConnectPhysicalAssetModel : MTConnectClassModel
    {
        /// <summary>
        /// Creates an empty model for manual population.
        /// </summary>
        public MTConnectPhysicalAssetModel() { }

        /// <summary>
        /// Parses the <c>PhysicalAsset</c> class as a concrete type deriving
        /// from <c>Asset</c> (abstract flag cleared).
        /// </summary>
        public MTConnectPhysicalAssetModel(XmiDocument xmiDocument, UmlClass umlClass) : base(xmiDocument, "Assets.PhysicalAsset", umlClass)
        {
            // Concrete, so Pallet and Fixture can derive from it; the Asset parent is kept so both are assets
            IsAbstract = false;
        }
    }
}
