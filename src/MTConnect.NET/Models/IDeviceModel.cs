// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Assets;
using MTConnect.Models.Components;
using MTConnect.Models.DataItems;
using MTConnect.Observations;
using MTConnect.Streams.Events;
using System;
using System.Collections.Generic;

namespace MTConnect.Models
{
    public interface IDeviceModel
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


        Availability Availability { get; set; }
        IDataItemModel AvailabilityDataItem { get; }


        Version MTConnectVersion { get; set; }
        IDataItemModel MTConnectVersionDataItem { get; }


        NetworkModel Network { get; set; }

        OperatingSystemModel OperatingSystem { get; set; }


        IAxesModel Axes { get; }

        IControllerModel Controller { get; }

        ISystemsModel Systems { get; }

        IAuxiliariesModel Auxiliaries { get; }


        #region "Assets"

        IAsset GetAsset(string assetId);

        T GetAsset<T>(string assetId) where T : IAsset;

        IEnumerable<IAsset> GetAssets();

        IEnumerable<MTConnect.Assets.CuttingTools.CuttingToolAsset> GetCuttingTools();

        void AddAsset(IAsset asset);

        #endregion


        IEnumerable<Observation> GetObservations(long timestamp = 0);

        IEnumerable<ConditionObservation> GetConditionObservations(long timestamp = 0);

        IEnumerable<IAsset> GetAdapterAssets();
    }
}
