// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Components;
using MTConnect.Devices.Compositions;
using MTConnect.Models.Compositions;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Turret is a ToolingDelivery that represents a tool mounting mechanism that holds any number of tools.
    /// Tools are located in STATIONs. Tools are positioned for use in the manufacturing process by rotating the Turret.
    /// </summary>
    public class TurretModel : ToolingDeliveryModel, ITurretModel
    {
        /// <summary>
        /// Storage or mounting locations for tools associated with the Turret.
        /// </summary>
        public IEnumerable<IStationModel> Stations
        {
            get
            {
                var x = new List<IStationModel>();

                if (!ComponentModels.IsNullOrEmpty())
                {
                    var models = ComponentModels.Where(o => o.Type == StationComposition.TypeId);
                    if (!models.IsNullOrEmpty())
                    {
                        foreach (var model in models) x.Add((StationModel)model);
                    }
                }

                return x;
            }
        }


        public TurretModel()
        {
            Type = TurretComponent.TypeId;
        }

        public TurretModel(string componentId)
        {
            Id = componentId;
            Type = TurretComponent.TypeId;
        }


        public IStationModel GetStation(int stationNumber)
        {
            return CompositionModels.FirstOrDefault(o => o.Station == stationNumber.ToString()).Convert<StationModel>();
        }
    }
}