// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices.Events;

namespace MTConnect.Models.Components
{
    public class PalletLoadStationModel : ComponentModel
    {
        public const string TypeId = "PalletLoadStation";


        /// <summary>
        /// The identifier for a pallet.
        /// </summary>
        public string PalletId
        {
            get => GetDataItemValue(Devices.Events.PalletIdDataItem.NameId);
            set => AddDataItem(new PalletIdDataItem(Id), value);
        }
        public IDataItemModel PalletIdDataItem => GetDataItem(Devices.Events.PalletIdDataItem.NameId);


        /// <summary>
        /// The identifier of an individual pallet asset.
        /// </summary>
        public string PalletAssetId
        {
            get => GetDataItemValue(Devices.Events.PalletAssetIdDataItem.NameId);
            set => AddDataItem(new PalletAssetIdDataItem(Id), value);
        }
        public IDataItemModel PalletAssetIdDataItem => GetDataItem(Devices.Events.PalletAssetIdDataItem.NameId);


        public PalletLoadStationModel()
        {
            Type = TypeId;
        }

        public PalletLoadStationModel(string deviceId)
        {
            Id = deviceId;
            Type = TypeId;
            Name = deviceId;
        }
    }
}