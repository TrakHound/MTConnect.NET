using System.Collections.Generic;

namespace MTConnect.SysML
{
    public class MTConnectPackageModel
    {
        public List<MTConnectClassModel> Classes { get; set; } = new();

        public List<MTConnectEnumModel> Enums { get; set; } = new();
    }
}
