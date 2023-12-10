// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Observations;
using System.Collections.Generic;

namespace MTConnect.Formatters
{
    public interface IEntityFormatter
    {
        string Id { get; }

        string ContentType { get; }


        FormatWriteResult Format(IDevice device, IEnumerable<KeyValuePair<string, string>> options = null);

        FormatWriteResult Format(IObservation observation, IEnumerable<KeyValuePair<string, string>> options = null);

        FormatWriteResult Format(IEnumerable<IObservation> observations, IEnumerable<KeyValuePair<string, string>> options = null);

        FormatWriteResult Format(IAsset asset, IEnumerable<KeyValuePair<string, string>> options = null);


        FormatReadResult<IDevice> CreateDevice(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null);

        FormatReadResult<IComponent> CreateComponent(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null);

        FormatReadResult<IComposition> CreateComposition(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null);

        FormatReadResult<IDataItem> CreateDataItem(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null);

        FormatReadResult<IAsset> CreateAsset(string assetType, byte[] content, IEnumerable<KeyValuePair<string, string>> options = null);
    }
}