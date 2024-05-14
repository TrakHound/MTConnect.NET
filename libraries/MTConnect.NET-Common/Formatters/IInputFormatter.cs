// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Input;
using System.Collections.Generic;

namespace MTConnect.Formatters
{
    public interface IInputFormatter
    {
        string Id { get; }

        string ContentType { get; }


        FormatWriteResult Format(IDeviceInput device, IEnumerable<KeyValuePair<string, string>> options = null);

        FormatWriteResult Format(IEnumerable<IObservationInput> observations, IEnumerable<KeyValuePair<string, string>> options = null);

        FormatWriteResult Format(IEnumerable<IAssetInput> assets, IEnumerable<KeyValuePair<string, string>> options = null);


        FormatReadResult<IDevice> CreateDevice(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null);

        FormatReadResult<IEnumerable<IObservationInput>> CreateObservations(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null);

        FormatReadResult<IEnumerable<IAsset>> CreateAssets(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null);
    }
}