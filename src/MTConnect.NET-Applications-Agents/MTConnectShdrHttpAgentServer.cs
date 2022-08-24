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

namespace MTConnect.Applications.Agents
{
    /// <summary>
    /// An Http Web Server for processing MTConnect REST Api Requests that supports SHDR Put and Post requests
    /// </summary>
    public class MTConnectShdrHttpAgentServer : MTConnectHttpAgentServer
    {
        public MTConnectShdrHttpAgentServer(
            IHttpAgentApplicationConfiguration configuration,
            IMTConnectAgent mtconnectAgent,
            IEnumerable<string> prefixes = null,
            int port = 0
            )
            : base(configuration, mtconnectAgent, prefixes, port) { }


        protected override bool OnObservationInput(string deviceKey, string dataItemKey, string input)
        {
            // Get the Devices Document from the Agent
            var devicesDocument = _mtconnectAgent.GetDevices(deviceKey);
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
                            if (condition != null) _mtconnectAgent.AddObservation(device.Uuid, condition);
                        }
                        else if (dataItem.Type == Devices.DataItems.Events.MessageDataItem.TypeId)
                        {
                            var message = ShdrMessage.FromString(shdrLine);
                            if (message != null) _mtconnectAgent.AddObservation(device.Uuid, message);
                        }
                        else if (dataItem.Representation == DataItemRepresentation.TABLE)
                        {
                            var table = ShdrTable.FromString(shdrLine);
                            if (table != null) _mtconnectAgent.AddObservation(device.Uuid, table);
                        }
                        else if (dataItem.Representation == DataItemRepresentation.DATA_SET)
                        {
                            var dataSet = ShdrDataSet.FromString(shdrLine);
                            if (dataSet != null) _mtconnectAgent.AddObservation(device.Uuid, dataSet);
                        }
                        else if (dataItem.Representation == DataItemRepresentation.TIME_SERIES)
                        {
                            var timeSeries = ShdrTimeSeries.FromString(shdrLine);
                            if (timeSeries != null) _mtconnectAgent.AddObservation(device.Uuid, timeSeries);
                        }
                        else
                        {
                            var dataItems = ShdrDataItem.FromString(shdrLine);
                            if (!dataItems.IsNullOrEmpty()) _mtconnectAgent.AddObservations(device.Uuid, dataItems);
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

        protected override bool OnAssetInput(string assetId, string deviceKey, string assetType, byte[] requestBytes)
        {
            if (!string.IsNullOrEmpty(deviceKey) && !string.IsNullOrEmpty(assetType))
            {
                var asset = Assets.Xml.XmlAsset.FromXml(assetType, ReadRequestBody(requestBytes));
                if (asset != null)
                {
                    asset.AssetId = assetId;
                    asset.Timestamp = asset.Timestamp > 0 ? asset.Timestamp : UnixDateTime.Now;
                    return _mtconnectAgent.AddAsset(deviceKey, asset);
                }
            }

            return false;
        }

        private byte[] ReadRequestBody(byte[] bytes)
        {
            if (bytes != null)
            {
                try
                {
                    return Encoding.Convert(Encoding.ASCII, Encoding.UTF8, bytes);
                }
                catch { }
            }

            return null;
        }
    }
}
