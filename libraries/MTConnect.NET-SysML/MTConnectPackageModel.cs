using System.Collections.Generic;

namespace MTConnect.SysML
{
    /// <summary>
    /// A parsed MTConnect package: the classes and enumerations it
    /// contributes to the generated output.
    /// </summary>
    public class MTConnectPackageModel
    {
        /// <summary>
        /// The classes declared in this package.
        /// </summary>
        public List<MTConnectClassModel> Classes { get; set; } = new();

        /// <summary>
        /// The enumerations declared in this package.
        /// </summary>
        public List<MTConnectEnumModel> Enums { get; set; } = new();
    }
}
