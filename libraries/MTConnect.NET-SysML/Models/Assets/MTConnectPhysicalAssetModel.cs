using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;

namespace MTConnect.SysML.Models.Assets
{
    public class MTConnectPhysicalAssetModel : MTConnectClassModel
    {
        public MTConnectPhysicalAssetModel() { }

        public MTConnectPhysicalAssetModel(XmiDocument xmiDocument, UmlClass umlClass) : base(xmiDocument, "Assets.PhysicalAsset", umlClass)
        {
            ParentName = null;
            IsAbstract = false;
        }
    }
}
