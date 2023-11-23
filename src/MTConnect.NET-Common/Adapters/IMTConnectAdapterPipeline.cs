// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Observations.Input;
using System;

namespace MTConnect.Adapters
{
    public interface IMTConnectAdapterClient
    {
        void AddObservation(string dataItemKey, object resultValue, DateTime? timestamp = null);

        void AddObservation(string dataItemKey, object resultValue, long? timestamp = null);

        void AddObservation(IObservationInput observation);


        void SetUnavailable(long timestamp = 0);
    }
}
