using System.Collections.Generic;

namespace MTConnect.SysML.Models.Devices
{
    /// <summary>
    /// The parsed Composition model: the <c>Composition</c> base class plus
    /// its concrete composition subtypes.
    /// </summary>
    public class MTConnectCompositionsModel
    {
        /// <summary>
        /// The parsed <c>Composition</c> base class.
        /// </summary>
        public MTConnectCompositionModel Composition { get; set; }

        /// <summary>
        /// The concrete Composition subtypes derived from the
        /// <c>CompositionTypeEnum</c>.
        /// </summary>
        public List<MTConnectCompositionType> Types { get; set; } = new();
    }
}
