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
        /// Parses the <c>PhysicalAsset</c> class as a concrete, root-level
        /// type (parent and abstract flag cleared).
        /// </summary>
        public MTConnectPhysicalAssetModel(XmiDocument xmiDocument, UmlClass umlClass) : base(xmiDocument, "Assets.PhysicalAsset", umlClass)
        {
            ParentName = null;
            IsAbstract = false;
        }
    }
}
