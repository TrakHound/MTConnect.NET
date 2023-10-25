using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;

namespace MTConnect.SysML.Models.Assets
{
    public class MTConnectAssetModel : MTConnectClassModel
    {
        public MTConnectAssetModel() { }

        public MTConnectAssetModel(XmiDocument xmiDocument, UmlClass umlClass) : base(xmiDocument, "Assets.Asset", umlClass)
        {
            ParentName = null;
            IsAbstract = false;
        }
    }
}
