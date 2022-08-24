// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

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
