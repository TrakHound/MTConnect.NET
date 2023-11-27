// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Models.Compositions;
using System.Collections.Generic;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Turret is a ToolingDelivery that represents a tool mounting mechanism that holds any number of tools.
    /// Tools are located in STATIONs. Tools are positioned for use in the manufacturing process by rotating the Turret.
    /// </summary>
    public interface ITurretModel : IAuxiliaryModel
    {
        /// <summary>
        /// Storage or mounting locations for tools associated with the Turret.
        /// </summary>
        IEnumerable<IStationModel> Stations { get; }


        IStationModel GetStation(int stationNumber);
    }
}