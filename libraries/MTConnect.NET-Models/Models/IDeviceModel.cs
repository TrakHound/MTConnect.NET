// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Models.Components;
using MTConnect.Models.DataItems;
using MTConnect.Observations;
using MTConnect.Observations.Input;
using MTConnect.Observations.Events.Values;
using System;
using System.Collections.Generic;

namespace MTConnect.Models
{
    public interface IDeviceModel : Devices.IDevice
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


        EventHandler<IObservation> ObservationUpdated { get; set; }


        #region "Assets"

        IAsset GetAsset(string assetId);

        T GetAsset<T>(string assetId) where T : IAsset;

        IEnumerable<IAsset> GetAssets();

        IEnumerable<MTConnect.Assets.CuttingTools.CuttingToolAsset> GetCuttingTools();

        void AddAsset(IAsset asset);

        #endregion


        IEnumerable<IObservation> GetObservations();

        //IEnumerable<ObservationInput> GetObservations(long timestamp = 0);

        IEnumerable<ConditionObservationInput> GetConditionObservations(long timestamp = 0);

        IEnumerable<IAsset> GetAdapterAssets();
    }
}