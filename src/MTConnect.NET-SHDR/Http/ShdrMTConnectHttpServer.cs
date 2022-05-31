// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Adapters.Shdr;
using MTConnect.Agents;
using MTConnect.Devices.DataItems;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTConnect.Http
{
    /// <summary>
    /// An Http Web Server for processing MTConnect REST Api Requests with a processor PUT requests for SHDR data sent in the URL query string
    /// </summary>
    public class ShdrMTConnectHttpServer : MTConnectHttpServer
    {
        private readonly IMTConnectAgent _mtconnectAgent;


        public ShdrMTConnectHttpServer(IMTConnectAgent mtconnectAgent, IEnumerable<string> prefixes = null) : base(mtconnectAgent, prefixes) 
        {
            _mtconnectAgent = mtconnectAgent;
        }


        protected override async Task<bool> OnObservationInput(string deviceKey, string dataItemKey, string input)
        {
            // Get the Devices Document from the Agent
            var devicesDocument = await _mtconnectAgent.GetDevicesAsync(deviceKey);
            if (devicesDocument != null && !devicesDocument.Devices.IsNullOrEmpty())
            {
                // Get the first Device (should only be one Device)
                var device = devicesDocument.Devices.FirstOrDefault();
                if (device != null)
                {
                    // Get the DataItem based on the Key
                    var dataItem = device.GetDataItemByKey(dataItemKey);
                    if (dataItem != null)
                    {
                        // Construct an SHDR Line using the DataItemId and the Input string from Http
                        var shdrLine = $"{dataItem.Id}|{input}";

                        if (dataItem.Category == DataItemCategory.CONDITION)
                        {
                            var condition = ShdrFaultState.FromString(shdrLine);
                            if (condition != null) await _mtconnectAgent.AddObservationAsync(device.Uuid, condition);
                        }
                        else if (dataItem.Type == Devices.DataItems.Events.MessageDataItem.TypeId)
                        {
                            var message = ShdrMessage.FromString(shdrLine);
                            if (message != null) await _mtconnectAgent.AddObservationAsync(device.Uuid, message);
                        }
                        else if (dataItem.Representation == DataItemRepresentation.TABLE)
                        {
                            var table = ShdrTable.FromString(shdrLine);
                            if (table != null) await _mtconnectAgent.AddObservationAsync(device.Uuid, table);
                        }
                        else if (dataItem.Representation == DataItemRepresentation.DATA_SET)
                        {
                            var dataSet = ShdrDataSet.FromString(shdrLine);
                            if (dataSet != null) await _mtconnectAgent.AddObservationAsync(device.Uuid, dataSet);
                        }
                        else if (dataItem.Representation == DataItemRepresentation.TIME_SERIES)
                        {
                            var timeSeries = ShdrTimeSeries.FromString(shdrLine);
                            if (timeSeries != null) await _mtconnectAgent.AddObservationAsync(device.Uuid, timeSeries);
                        }
                        else
                        {
                            var dataItems = ShdrDataItem.FromString(shdrLine);
                            if (!dataItems.IsNullOrEmpty()) await _mtconnectAgent.AddObservationsAsync(device.Uuid, dataItems);
                        }
                    }

                    return true;
                }
            }

            return false;
        }
    }
}
