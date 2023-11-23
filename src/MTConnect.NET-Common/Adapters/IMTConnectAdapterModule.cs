// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Input;
using System.Collections.Generic;

namespace MTConnect.Adapters
{
    public interface IMTConnectAdapterModule
    {
        string Id { get; }

        string Description { get; }


        void Start();

        void Stop();


        bool WriteObservations(IEnumerable<IObservationInput> observations);

        bool WriteConditionObservations(IEnumerable<IConditionObservationInput> conditionObservations);
    }
}
