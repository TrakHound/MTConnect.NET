using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;

namespace MTConnect.SysML.Models.Assets
{
    /// <summary>
    /// Parsed model for the MTConnect <c>Assets.Asset</c> element, emitted as
    /// a concrete root type.
    /// </summary>
    public class MTConnectAssetModel : MTConnectClassModel
    {
        /// <summary>
        /// Creates an empty model for manual population.
        /// </summary>
        public MTConnectAssetModel() { }

        /// <summary>
        /// Parses the <c>Asset</c> class as a concrete, root-level type
        /// (parent and abstract flag cleared).
        /// </summary>
        public MTConnectAssetModel(XmiDocument xmiDocument, UmlClass umlClass) : base(xmiDocument, "Assets.Asset", umlClass)
        {
            ParentName = null;
            IsAbstract = false;
        }
    }
}
