using System.Collections.Generic;

namespace MTConnect.SysML.Models.Devices
{
    /// <summary>
    /// The parsed DataItem model: the <c>DataItem</c> base class, its
    /// concrete type subclasses, and the supporting classes and enumerations
    /// the DataItem types depend on.
    /// </summary>
    public class MTConnectDataItemsModel
    {
        /// <summary>
        /// The parsed <c>DataItem</c> base class.
        /// </summary>
        public MTConnectDataItemModel DataItem { get; set; }

        /// <summary>
        /// The concrete DataItem type subclasses derived from the DataItem
        /// type enumerations.
        /// </summary>
        public List<MTConnectDataItemType> Types { get; set; } = new();

        /// <summary>
        /// Supporting classes referenced by the DataItem types.
        /// </summary>
        public List<MTConnectClassModel> Classes { get; set; } = new();

        /// <summary>
        /// Enumerations referenced by the DataItem types.
        /// </summary>
        public List<MTConnectEnumModel> Enums { get; set; } = new();
    }
}
