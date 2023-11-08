// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Formatters;
using MTConnect.Servers;
using MTConnect.Shdr;
using System;
using System.Linq;
using System.Text;

namespace MTConnect.Modules.Http
{
    /// <summary>
    /// An Http Web Server for processing MTConnect REST Api Requests that supports SHDR Put and Post requests
    /// </summary>
    public class MTConnectShdrHttpAgentServer : MTConnectHttpAgentServer
    {
        public MTConnectShdrHttpAgentServer(HttpModuleConfiguration configuration, IMTConnectAgentBroker mtconnectAgent) : base(configuration, mtconnectAgent) { }


        //protected override bool OnObservationInput(string deviceKey, string dataItemKey, string input)
        protected override bool OnObservationInput(MTConnectObservationInputArgs args)
        {
            // Get the Devices Document from the Agent
            var devicesDocument = _mtconnectAgent.GetDevicesResponseDocument(args.DeviceKey);
            if (devicesDocument != null && !devicesDocument.Devices.IsNullOrEmpty())
            {
                // Get the first Device (should only be one Device)
                var device = devicesDocument.Devices.FirstOrDefault();
                if (device != null)
                {
                    // Get the DataItem based on the Key
                    var dataItem = device.GetDataItemByKey(args.DataItemKey);
                    if (dataItem != null)
                    {
                        // Construct an SHDR Line using the DataItemId and the Input string from Http
                        var shdrLine = $"|{dataItem.Id}|{args.Value}";

                        if (dataItem.Category == DataItemCategory.CONDITION)
                        {
                            var condition = ShdrFaultState.FromString(shdrLine);
                            if (condition != null) _mtconnectAgent.AddObservation(device.Uuid, condition);
                        }
                        else if (dataItem.Type == Devices.DataItems.MessageDataItem.TypeId)
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
                        //if (_mtconnectAgent.InvalidObservationAdded != null)
                        //{
                        //    _mtconnectAgent.InvalidObservationAdded.Invoke(deviceKey, dataItemKey, new ValidationResult(false, $"DataItemKey \"{dataItemKey}\" not Found in Device"));
                        //}
                    }

                    return true;
                }
                else
                {
                    //if (_mtconnectAgent.InvalidObservationAdded != null)
                    //{
                    //    _mtconnectAgent.InvalidObservationAdded.Invoke(deviceKey, dataItemKey, new ValidationResult(false, $"Device \"{deviceKey}\" not Found"));
                    //}
                }
            }

            return false;
        }

        //protected override bool OnAssetInput(string assetId, string deviceKey, string assetType, byte[] requestBytes, string documentFormat = DocumentFormat.XML)
        protected override bool OnAssetInput(MTConnectAssetInputArgs args)
        {
            if (!string.IsNullOrEmpty(args.DeviceKey) && !string.IsNullOrEmpty(args.AssetType))
            {
                //var asset = Assets.Xml.XmlAsset.FromXml(assetType, );
                var result = EntityFormatter.CreateAsset(args.DocumentFormat, args.AssetType, ReadRequestBody(args.RequestBody));
                if (result.Success)
                {
                    var asset = (Asset)result.Entity;
                    asset.AssetId = args.AssetId;
                    asset.Timestamp = asset.Timestamp > DateTime.MinValue ? asset.Timestamp : DateTime.Now;
                    return _mtconnectAgent.AddAsset(args.DeviceKey, asset);
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