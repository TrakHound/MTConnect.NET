// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using System.Xml.Serialization;

namespace MTConnect.MTConnectDevices.Components.Controller
{
    [XmlRoot("Controller")]
    public class ControllerComponent : IComponent
    {
        /// <summary>
        /// A container for lower level Component XML Elements associated with this parent Component.
        /// These lower level elements in this container are defined as Subcomponent elements.
        /// </summary>
        [XmlArray("Components")]
        [XmlArrayItem("Path", typeof(SubComponents.PathComponent))]
        public List<IComponent> Components;
    }
}
