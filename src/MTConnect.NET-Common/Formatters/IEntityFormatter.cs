// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

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


        string Format(IDevice device);

        string Format(IComponent component);

        string Format(IComposition composition);

        string Format(IDataItem dataItem);

        string Format(IObservation observation);

        string Format(IAsset asset);


        FormattedEntityReadResult<IDevice> CreateDevice(string content, IEnumerable<KeyValuePair<string, string>> options = null);

        FormattedEntityReadResult<IAsset> CreateAsset(string assetType, string content, IEnumerable<KeyValuePair<string, string>> options = null);
    }
}
