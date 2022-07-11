// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Devices.DataItems;
using MTConnect.Shdr;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTConnect.Http
{
    /// <summary>
    /// An Http Web Server for processing MTConnect REST Api Requests with
    /// a processor PUT requests for SHDR data sent in the URL query string
    /// and a processor for POST request for XML Asset data
    /// </summary>
    public class ShdrMTConnectHttpServer : MTConnectHttpServer
    {
        private readonly IMTConnectAgent _mtconnectAgent;


        public ShdrMTConnectHttpServer(
            HttpAgentConfiguration configuration,
            IMTConnectAgent mtconnectAgent,
            IEnumerable<string> prefixes = null,
            int port = 0
            )
            : base(configuration, mtconnectAgent, prefixes, port) 
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
                    else
                    {
                        if (_mtconnectAgent.InvalidObservationAdded != null)
                        {
                            _mtconnectAgent.InvalidObservationAdded.Invoke(deviceKey, dataItemKey, new ValidationResult(false, $"DataItemKey \"{dataItemKey}\" not Found in Device"));
                        }
                    }

                    return true;
                }
                else
                {
                    if (_mtconnectAgent.InvalidObservationAdded != null)
                    {
                        _mtconnectAgent.InvalidObservationAdded.Invoke(deviceKey, dataItemKey, new ValidationResult(false, $"Device \"{deviceKey}\" not Found"));
                    }
                }
            }

            return false;
        }

        protected override async Task<bool> OnAssetInput(string assetId, string deviceKey, string assetType, byte[] requestBytes)
        {
            var requestBody = ReadRequestBody(requestBytes);
            if (!string.IsNullOrEmpty(deviceKey) && !string.IsNullOrEmpty(assetType) && !string.IsNullOrEmpty(requestBody))
            {
                var asset = Assets.XmlAsset.FromXml(assetType, requestBody);
                if (asset != null)
                {
                    asset.AssetId = assetId;
                    asset.Timestamp = asset.Timestamp > 0 ? asset.Timestamp : UnixDateTime.Now;
                    return await _mtconnectAgent.AddAssetAsync(deviceKey, asset);
                }
            }

            return false;
        }

        private string ReadRequestBody(byte[] bytes)
        {
            if (bytes != null)
            {
                try
                {
                    var utf8Bytes = Encoding.Convert(Encoding.ASCII, Encoding.UTF8, bytes);
                    return Encoding.UTF8.GetString(utf8Bytes);
                }
                catch { }
            }

            return null;
        }
    }
}
