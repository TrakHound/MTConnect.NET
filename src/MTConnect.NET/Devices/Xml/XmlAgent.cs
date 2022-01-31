// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using System.Collections.Generic;
using System.Xml.Serialization;

namespace MTConnect.Devices.Xml
{
    /// <summary>
    /// Agent is a Device representing the MTConnect Agent and all its connected data sources.
    /// </summary>
    [XmlRoot("Agent")]
    public class XmlAgent : XmlDevice 
    {
        public XmlAgent()
        {
            DataItems = new List<XmlDataItem>();
            Compositions = new List<XmlComposition>();
        }

        public XmlAgent(Device device)
        {
            DataItems = new List<XmlDataItem>();
            Compositions = new List<XmlComposition>();

            if (device != null)
            {
                Id = device.Id;
                Name = device.Name;
                NativeName = device.NativeName;
                Uuid = device.Uuid;
                Description = device.Description;
                SampleRate = device.SampleRate;
                SampleInterval = device.SampleInterval;
                Iso841Class = device.Iso841Class;
                CoordinateSystemIdRef = device.CoordinateSystemIdRef;
                if (device.MTConnectVersion != null) MTConnectVersion = device.MTConnectVersion.ToString();
                Configuration = device.Configuration;
                References = device.References;
                Description = device.Description;


                // DataItems
                if (!device.DataItems.IsNullOrEmpty())
                {
                    foreach (var dataItem in device.DataItems)
                    {
                        DataItems.Add(new XmlDataItem(dataItem));
                    }
                }

                // Compositions
                if (!device.Compositions.IsNullOrEmpty())
                {
                    foreach (var composition in device.Compositions)
                    {
                        Compositions.Add(new XmlComposition(composition));
                    }
                }

                // Components
                if (!device.Components.IsNullOrEmpty())
                {
                    ComponentCollection = new XmlComponentCollection { Components = device.Components };
                }
            }
        }

        public override Device ToDevice()
        {
            var device = base.ToDevice();
            device.Type = Agent.TypeId;

            return device;
        }
    }
}
