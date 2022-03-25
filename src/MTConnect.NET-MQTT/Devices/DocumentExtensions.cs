// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using MQTTnet;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace MTConnect.Devices
{
    public static class DocumentExtensions
    {
        public static IEnumerable<MqttApplicationMessage> ToMqttMessage(this DevicesResponseDocument document)
        {
            if (document != null && !document.Devices.IsNullOrEmpty())
            {
                var messages = new List<MqttApplicationMessage>();

                foreach (var device in document.Devices)
                {                  
                    var json = JsonSerializer.Serialize(device);
                    var bytes = Encoding.ASCII.GetBytes(json);

                    messages.Add(new MqttApplicationMessage
                    {
                        Topic = device.Name,
                        Payload = bytes,
                        Retain = true
                    });
                }

                return messages;
            }

            return null;
        }
    }
}
