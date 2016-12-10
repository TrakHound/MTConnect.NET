using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MTConnect.MTConnectDevices.Components.Axes
{
    [XmlRoot("Axes")]
    public class AxesComponent : IComponent
    {
        /// <summary>
        /// A container for lower level Component XML Elements associated with this parent Component.
        /// These lower level elements in this container are defined as Subcomponent elements.
        /// </summary>
        [XmlArray("Components")]
        [XmlArrayItem("Linear", typeof(SubComponents.LinearComponent))]
        [XmlArrayItem("Rotary", typeof(SubComponents.RotaryComponent))]
        public List<IComponent> Components;
    }
}
