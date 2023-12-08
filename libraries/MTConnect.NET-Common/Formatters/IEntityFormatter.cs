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


        FormattedEntityWriteResult Format(IDevice device, IEnumerable<KeyValuePair<string, string>> options = null);

        //FormattedEntityWriteResult Format(IComponent component, IEnumerable<KeyValuePair<string, string>> options = null);

        //FormattedEntityWriteResult Format(IComposition composition, IEnumerable<KeyValuePair<string, string>> options = null);

        //FormattedEntityWriteResult Format(IDataItem dataItem, IEnumerable<KeyValuePair<string, string>> options = null);

        FormattedEntityWriteResult Format(IObservation observation, IEnumerable<KeyValuePair<string, string>> options = null);

        FormattedEntityWriteResult Format(IEnumerable<IObservation> observations, IEnumerable<KeyValuePair<string, string>> options = null);

        FormattedEntityWriteResult Format(IAsset asset, IEnumerable<KeyValuePair<string, string>> options = null);


        FormattedEntityReadResult<IDevice> CreateDevice(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null);

        FormattedEntityReadResult<IComponent> CreateComponent(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null);

        FormattedEntityReadResult<IComposition> CreateComposition(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null);

        FormattedEntityReadResult<IDataItem> CreateDataItem(byte[] content, IEnumerable<KeyValuePair<string, string>> options = null);

        FormattedEntityReadResult<IAsset> CreateAsset(string assetType, byte[] content, IEnumerable<KeyValuePair<string, string>> options = null);
    }
}