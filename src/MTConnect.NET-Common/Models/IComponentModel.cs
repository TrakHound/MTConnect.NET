// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using System.Collections.Generic;
using MTConnect.Observations;

namespace MTConnect.Models
{
    public interface IComponentModel
    {
        string Id { get; set; }

        string Type { get; set; }

        string Name { get; set; }

        string NativeName { get; set; }

        string Uuid { get; set; }

        string CoordinateSystemIdRef { get; set; }

        double SampleInterval { get; set; }

        double SampleRate { get; set; }

        string Manufacturer { get; set; }

        string Model { get; set; }

        string SerialNumber { get; set; }

        string Station { get; set; }

        string DescriptionText { get; set; }

        Configuration Configuration { get; set; }

        List<Reference> References { get; set; }

        List<IComponentModel> ComponentModels { get; set; }

        List<ICompositionModel> CompositionModels { get; set; }

        List<IDataItemModel> DataItemModels { get; set; }


        IEnumerable<Observation> GetObservations();
    }
}
