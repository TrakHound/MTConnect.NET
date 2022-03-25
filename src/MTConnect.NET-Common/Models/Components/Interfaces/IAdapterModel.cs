// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Observations.Events.Values;
using System;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Adapter is a Component that represents the connectivity state of a data source for the MTConnect Agent.
    /// </summary>
    public interface IAdapterModel : IComponentModel
    {
        /// <summary>
        /// An indicator of the controlled state of a Linear or Rotary component representing an axis.
        /// </summary>
        ConnectionStatus ConnectionStatus { get; set; }
        IDataItemModel ConnectionStatusDataItem { get; }

        /// <summary>
        /// An indicator of the controlled state of a Linear or Rotary component representing an axis.
        /// </summary>
        double ObservationUpdateRate { get; set; }
        IDataItemModel ObservationUpdateRateDataItem { get; }

        /// <summary>
        /// An indicator of the controlled state of a Linear or Rotary component representing an axis.
        /// </summary>
        double AssetUpdateRate { get; set; }
        IDataItemModel AssetUpdateRateDataItem { get; }

        /// <summary>
        /// An indicator of the controlled state of a Linear or Rotary component representing an axis.
        /// </summary>
        Version AdapterSoftwareVersion { get; set; }
        IDataItemModel AdapterSoftwareVersionDataItem { get; }

        /// <summary>
        /// An indicator of the controlled state of a Linear or Rotary component representing an axis.
        /// </summary>
        Version MTConnectVersion { get; set; }
        IDataItemModel MTConnectVersionDataItem { get; }
    }
}
