using System.Collections.Generic;

namespace MTConnect.SysML.Models.Devices
{
    /// <summary>
    /// The parsed Component model: the <c>Component</c> base class plus its
    /// concrete component subtypes.
    /// </summary>
    public class MTConnectComponentsModel
    {
        /// <summary>
        /// The parsed <c>Component</c> base class.
        /// </summary>
        public MTConnectComponentModel Component { get; set; }

        /// <summary>
        /// The concrete Component subtypes derived from the
        /// <c>ComponentTypeEnum</c>.
        /// </summary>
        public List<MTConnectComponentType> Types { get; set; } = new();
    }
}
