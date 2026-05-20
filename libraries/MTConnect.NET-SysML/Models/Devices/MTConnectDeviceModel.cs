using MTConnect.SysML.Xmi;
using MTConnect.SysML.Xmi.UML;
using System.Linq;

namespace MTConnect.SysML.Models.Devices
{
    /// <summary>
    /// Parsed model for the MTConnect <c>Devices.Device</c> element, with the
    /// MTConnectVersion casing fixed and the streaming/organizer navigations
    /// stripped so the generated class carries only the static device shape.
    /// </summary>
    public class MTConnectDeviceModel : MTConnectClassModel
    {
        /// <summary>
        /// Parses the <c>Device</c> class as a concrete type, renames the
        /// version property to <c>MTConnectVersion</c> of type
        /// <see cref="System.Version"/>, and removes the organizer and
        /// streaming navigations (Adapter, Axis, Controller, System, etc.).
        /// </summary>
        public MTConnectDeviceModel(XmiDocument xmiDocument, UmlClass umlClass) : base(xmiDocument, "Devices.Device", umlClass)
        {
            IsAbstract = false;

            // Override MTConnectVersion (to fix case)
            var mtconnectVersionProperty = Properties?.FirstOrDefault(o => o.Name?.ToLower() == "mtconnectversion");
            if (mtconnectVersionProperty != null)
            {
                mtconnectVersionProperty.Name = "MTConnectVersion";
                mtconnectVersionProperty.DataType = "System.Version";
            }

            Properties?.RemoveAll(o => o.Name == "Adapter");
            Properties?.RemoveAll(o => o.Name == "Auxiliary");
            Properties?.RemoveAll(o => o.Name == "Axis");
            Properties?.RemoveAll(o => o.Name == "ComponentStream");
            Properties?.RemoveAll(o => o.Name == "CompositionStream");
            Properties?.RemoveAll(o => o.Name == "Controller");
            Properties?.RemoveAll(o => o.Name == "DataItemStream");
            Properties?.RemoveAll(o => o.Name == "Interface");
            Properties?.RemoveAll(o => o.Name == "Resource");
            Properties?.RemoveAll(o => o.Name == "Part");
            Properties?.RemoveAll(o => o.Name == "Process");
            Properties?.RemoveAll(o => o.Name == "Structure");
            Properties?.RemoveAll(o => o.Name == "System");
        }
    }
}
