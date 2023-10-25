using System.Collections.Generic;

namespace MTConnect.SysML.Models.Devices
{
    public class MTConnectDataItemsModel
    {
        public MTConnectDataItemModel DataItem { get; set; }

        public List<MTConnectDataItemType> Types { get; set; } = new();

        public List<MTConnectClassModel> Classes { get; set; } = new();

        public List<MTConnectEnumModel> Enums { get; set; } = new();
    }
}
