// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    [XmlRoot("Agent")]
    public class XmlAgent : XmlDevice 
    {
        public override IDevice ToDevice()
        {
            var device = new Agent();
            return ToDevice(device);
        }
    }
}