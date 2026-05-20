// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// XML serialization surrogate for an MTConnect <c>Agent</c> device. The
    /// agent is a specialized device that reports the state of the MTConnect
    /// Agent itself; this surrogate maps the <c>Agent</c> element of an
    /// MTConnectDevices document onto the strongly-typed <see cref="Agent"/> model.
    /// </summary>
    [XmlRoot("Agent")]
    public class XmlAgent : XmlDevice
    {
        /// <summary>
        /// Converts this surrogate to a strongly-typed <see cref="Agent"/>,
        /// applying the shared device deserialization defined by <see cref="XmlDevice"/>.
        /// </summary>
        public override IDevice ToDevice()
        {
            var device = new Agent();
            return ToDevice(device);
        }
    }
}