// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Models.Components;
using MTConnect.Observations;
using MTConnect.Observations.Input;
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

        string DescriptionText { get; set; }


        IEnumerable<IComponentModel> ComponentModels { get; }

        IEnumerable<ICompositionModel> CompositionModels { get; }

        IEnumerable<IDataItemModel> DataItemModels { get; }


        IAdaptersModel Adapters { get; }


        IEnumerable<IObservation> GetObservations();
        //IEnumerable<ObservationInput> GetObservations(long timestamp = 0);

        IEnumerable<ConditionObservationInput> GetConditionObservations(long timestamp = 0);
    }
}