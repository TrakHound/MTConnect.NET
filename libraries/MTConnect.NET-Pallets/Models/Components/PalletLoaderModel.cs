// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Models.Components
{
    public class PalletLoaderModel : LoaderModel
    {
        public const string TypeId = "PalletLoader";


        public PalletTransferModel PalletTransfer => ComponentModels?.FirstOrDefault(o => o.Type == PalletTransferModel.TypeId).Convert<PalletTransferModel>();


        public IEnumerable<PalletStockStationModel> PalletStockStations => ComponentModels?.Where(o => o.Type == PalletStockStationModel.TypeId).Convert<PalletStockStationModel>();

        public IEnumerable<PalletMachineStationModel> PalletMachineStations => ComponentModels?.Where(o => o.Type == PalletMachineStationModel.TypeId).Convert<PalletMachineStationModel>();

        public IEnumerable<PalletLoadStationModel> PalletLoadStations => ComponentModels?.Where(o => o.Type == PalletLoadStationModel.TypeId).Convert<PalletLoadStationModel>();


        public PalletLoaderModel()
        {
            Type = TypeId;
        }

        public PalletLoaderModel(string deviceId)
        {
            Id = deviceId;
            Type = TypeId;
            Name = deviceId;
        }



        public PalletStockStationModel GetPalletStockStation(int stationNumber) => PalletStockStations.FirstOrDefault(o => o.Id == CreateId(Id, $"stock_{stationNumber}"));

        public PalletMachineStationModel GetPalletMachineStation(int stationNumber) => PalletMachineStations.FirstOrDefault(o => o.Id == CreateId(Id, $"machine_{stationNumber}"));

        public PalletLoadStationModel GetPalletLoadStation(int stationNumber) => PalletLoadStations.FirstOrDefault(o => o.Id == CreateId(Id, $"load_{stationNumber}"));


        public PalletTransferModel AddPalletTransfer(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var model = new PalletTransferModel
                {
                    Id = CreateId(Id, name),
                    Name = name
                };

                AddComponentModel(model);
                return model;
            }

            return null;
        }


        public PalletStockStationModel AddPalletStockStation(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var model = new PalletStockStationModel
                {
                    Id = CreateId(Id, name),
                    Name = name
                };

                AddComponentModel(model);
                return model;
            }

            return null;
        }

        public PalletMachineStationModel AddPalletMachineStation(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var model = new PalletMachineStationModel
                {
                    Id = CreateId(Id, name),
                    Name = name
                };

                AddComponentModel(model);
                return model;
            }

            return null;
        }

        public PalletLoadStationModel AddPalletLoadStation(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var model = new PalletLoadStationModel
                {
                    Id = CreateId(Id, name),
                    Name = name
                };

                AddComponentModel(model);
                return model;
            }

            return null;
        }
    }
}