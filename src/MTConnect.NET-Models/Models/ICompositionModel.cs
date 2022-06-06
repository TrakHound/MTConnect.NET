// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations;
using System;
using System.Collections.Generic;

namespace MTConnect.Models
{
    public interface ICompositionModel
    {
        string Id { get; set; }

        string Type { get; set; }

        string Name { get; set; }

        string NativeName { get; set; }

        string Uuid { get; set; }

        string Manufacturer { get; set; }

        string Model { get; set; }

        string SerialNumber { get; set; }

        string Station { get; set; }

        string DescriptionText { get; set; }

        List<IDataItemModel> DataItemModels { get; set; }


        EventHandler<IObservation> ObservationUpdated { get; set; }

        IEnumerable<IObservation> GetObservations();
    }
}
