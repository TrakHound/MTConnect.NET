using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;

namespace MTConnect.SysML.Models.Devices
{
    /// <summary>
    /// Parsed model for the MTConnect <c>Devices.Component</c> element, with
    /// the streaming navigations stripped so the generated base class only
    /// carries the static device-model shape.
    /// </summary>
    public class MTConnectComponentModel : MTConnectClassModel
    {
        /// <summary>
        /// Parses the <c>Component</c> class as a concrete, root-level type
        /// and removes the ComponentStream, CompositionStream, and
        /// DataItemStream navigations that belong to the streaming model.
        /// </summary>
        public MTConnectComponentModel(XmiDocument xmiDocument, UmlClass umlClass) : base(xmiDocument, "Devices.Component", umlClass)
        {
            IsAbstract = false;
            ParentName = null;

            Properties?.RemoveAll(o => o.Name == "ComponentStream");
            Properties?.RemoveAll(o => o.Name == "CompositionStream");
            Properties?.RemoveAll(o => o.Name == "DataItemStream");
        }
    }
}
