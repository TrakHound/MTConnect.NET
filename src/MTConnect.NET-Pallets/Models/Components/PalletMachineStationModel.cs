// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Models.Components
{
    public class PalletMachineStationModel : ComponentModel
    {
        public const string TypeId = "PalletMachineStation";


        public IEnumerable<PalletMachinePositionModel> PalletMachinePositions => ComponentModels?.Where(o => o.Type == PalletMachinePositionModel.TypeId).Convert<PalletMachinePositionModel>();


        public PalletMachineStationModel()
        {
            Type = TypeId;
        }

        public PalletMachineStationModel(string deviceId)
        {
            Id = deviceId;
            Type = TypeId;
            Name = deviceId;
        }


        public PalletMachinePositionModel GetPalletMachinePosition(string name) => PalletMachinePositions.FirstOrDefault(o => o.Id == CreateId(Id, $"pos_{name}"));

        public PalletMachinePositionModel AddPalletMachinePosition(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var model = new PalletMachinePositionModel
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
