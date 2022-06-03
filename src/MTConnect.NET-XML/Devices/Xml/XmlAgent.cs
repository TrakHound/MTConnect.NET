// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MTConnect.Devices.References;
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
            DataItemCollection = new XmlDataItemCollection();
            Compositions = new List<XmlComposition>();
        }

        public XmlAgent(IDevice device)
        {
            DataItemCollection = new XmlDataItemCollection();
            Compositions = new List<XmlComposition>();

            if (device != null)
            {
                Id = device.Id;
                Name = device.Name;
                NativeName = device.NativeName;
                Uuid = device.Uuid;
                SampleRate = device.SampleRate;
                SampleInterval = device.SampleInterval;
                Iso841Class = device.Iso841Class;
                CoordinateSystemIdRef = device.CoordinateSystemIdRef;
                if (device.MTConnectVersion != null) MTConnectVersion = device.MTConnectVersion.ToString();

                if (device.Description != null) Description = new XmlDescription(device.Description);
                if (device.Configuration != null) Configuration = new XmlConfiguration(device.Configuration);

                // References
                if (!device.References.IsNullOrEmpty())
                {
                    var references = new List<XmlReference>();
                    foreach (var reference in device.References)
                    {
                        if (reference.GetType() == typeof(ComponentReference))
                        {
                            references.Add(new XmlComponentReference((ComponentReference)reference));
                        }

                        if (reference.GetType() == typeof(DataItemReference))
                        {
                            references.Add(new XmlDataItemReference((DataItemReference)reference));
                        }
                    }
                    References = references;
                }

                // DataItems
                if (!device.DataItems.IsNullOrEmpty())
                {
                    foreach (var dataItem in device.DataItems)
                    {
                        DataItemCollection.DataItems.Add(dataItem);
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
                    var componentCollection = new XmlComponentCollection();
                    foreach (var component in device.Components)
                    {
                        componentCollection.Components.Add(new XmlComponent(component));
                    }
                    ComponentCollection = componentCollection;
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
