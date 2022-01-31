// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Models.Components;
using MTConnect.Observations;
using System.Collections.Generic;

namespace MTConnect.Models
{
    public interface IAgentModel
    {
        string Id { get; set; }

        string Name { get; set; }

        string Uuid { get; set; }

        string Iso841Class { get; set; }

        string NativeName { get; set; }

        double SampleInterval { get; set; }

        double SampleRate { get; set; }

        string Manufacturer { get; set; }

        string Model { get; set; }

        string SerialNumber { get; set; }

        string Station { get; set; }

        string Description { get; set; }


        List<IComponentModel> ComponentModels { get; set; }

        List<ICompositionModel> CompositionModels { get; set; }

        List<IDataItemModel> DataItemModels { get; set; }


        IAdaptersModel Adapters { get; }


        IEnumerable<Observation> GetObservations(long timestamp = 0);

        IEnumerable<ConditionObservation> GetConditionObservations(long timestamp = 0);
    }
}
