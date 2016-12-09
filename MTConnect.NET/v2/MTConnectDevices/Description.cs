// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Xml;

namespace MTConnect.MTConnectDevices
{
    /// <summary>
    /// An element that can contain any description content.
    /// </summary>
    public class Description
    {
        public Description(XmlNode node)
        {
            Tools.XML.AssignProperties(this, node);
            CDATA = node.InnerText;
        }

        /// <summary>
        /// The name of the manufacturer of the Component
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// The model description of the Component
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// The component's serial number
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// The station where the Component is located when a component is part of a manufacturing unit or cell with multiple stations that share the same physical controller.
        /// </summary>
        public string Station { get; set; }

        /// <summary>
        /// Any additional descriptive information the implementer chooses to include regarding the Component.
        /// </summary>
        public string CDATA { get; set; }
    }
}
