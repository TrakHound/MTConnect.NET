// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.DataItems.Events;
using MTConnect.Models.DataItems;
using MTConnect.Observations.Events.Values;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// PartOccurrence is a Component that organizes information about a specific part as it exists at a specific place and time, 
    /// such as a specific instance of a bracket at a specific timestamp. Part is defined as a discrete item that has both defined
    /// and measurable physical characteristics including mass, material and features and is created by applying one or more manufacturing process steps to a workpiece.
    /// </summary>
    public class PartOccurrenceModel : ComponentModel, IPartOccurrenceModel
    {
        /// <summary>
        /// An identifier of a part in a manufacturing operation.
        /// </summary>
        public string PartId
        {
            get => DataItemManager.GetDataItemValue(DataItem.CreateId(Id, PartIdDataItem.NameId));
            set => DataItemManager.AddDataItem(new PartIdDataItem(Id), value);
        }

        /// <summary>
        /// Identifier given to a distinguishable, individual part.
        /// </summary>
        public PartUniqueIdModel PartUniqueId
        {
            get => GetPartUniqueId();
            set => SetPartUniqueId(value);
        }

        /// <summary>
        /// Identifier given to link the individual occurrence to a class of parts, typically distinguished by a particular part design.
        /// </summary>
        public PartKindIdModel PartKindId
        {
            get => GetPartKindId();
            set => SetPartKindId(value);
        }

        /// <summary>
        /// The aggregate count of parts.
        /// </summary>
        public PartCountModel PartCount
        {
            get => GetPartCount();
            set => SetPartCount(value);
        }

        /// <summary>
        /// State or condition of a part.
        /// </summary>
        public PartStatus PartStatus
        {
            get => DataItemManager.GetDataItemValue<PartStatus>(DataItem.CreateId(Id, PartStatusDataItem.NameId));
            set => DataItemManager.AddDataItem(new PartStatusDataItem(Id), value);
        }

        /// <summary>
        /// An identifier of a process being executed by the device.
        /// </summary>
        public string ProcessOccurrenceId
        {
            get => DataItemManager.GetDataItemValue(DataItem.CreateId(Id, ProcessOccurrenceIdDataItem.NameId));
            set => DataItemManager.AddDataItem(new PartStatusDataItem(Id), value);
        }

        /// <summary>
        /// The identifier of the person currently responsible for operating the piece of equipment.
        /// </summary>
        public string OperatorUser
        {
            get => DataItemManager.GetDataItemValue(DataItem.CreateId(Id, UserDataItem.NameId));
            set => DataItemManager.AddDataItem(new UserDataItem(Id, UserDataItem.SubTypes.OPERATOR), value);
        }

        /// <summary>
        /// The identifier of the person currently responsible for performing maintenance on the piece of equipment.
        /// </summary>
        public string MaintenanceUser
        {
            get => DataItemManager.GetDataItemValue(DataItem.CreateId(Id, UserDataItem.NameId));
            set => DataItemManager.AddDataItem(new UserDataItem(Id, UserDataItem.SubTypes.MAINTENANCE), value);
        }

        /// <summary>
        /// The identifier of the person currently responsible for preparing a piece of equipment for production
        /// or restoring the piece of equipment to a neutral state after production.
        /// </summary>
        public string SetupUser
        {
            get => DataItemManager.GetDataItemValue(DataItem.CreateId(Id, UserDataItem.NameId));
            set => DataItemManager.AddDataItem(new UserDataItem(Id, UserDataItem.SubTypes.SET_UP), value);
        }


        public PartOccurrenceModel() 
        {
            Type = PartOccurrenceComponent.TypeId;
        }

        public PartOccurrenceModel(string componentId)
        {
            Id = componentId;
            Type = PartOccurrenceComponent.TypeId;
        }


        private PartUniqueIdModel GetPartUniqueId()
        {
            var x = new PartUniqueIdModel();
            x.RawMaterial = DataItemManager.GetDataItemValue(PartUniqueIdDataItem.NameId, PartUniqueIdDataItem.GetSubTypeId(PartUniqueIdDataItem.SubTypes.RAW_MATERIAL));
            x.SerialNumber = DataItemManager.GetDataItemValue(PartUniqueIdDataItem.NameId, PartUniqueIdDataItem.GetSubTypeId(PartUniqueIdDataItem.SubTypes.SERIAL_NUMBER));
            x.Uuid = DataItemManager.GetDataItemValue(PartUniqueIdDataItem.NameId, PartUniqueIdDataItem.GetSubTypeId(PartUniqueIdDataItem.SubTypes.UUID));
            return x;
        }

        private void SetPartUniqueId(PartUniqueIdModel partUniqueId)
        {
            if (partUniqueId != null)
            {
                DataItemManager.AddDataItem(new PartUniqueIdDataItem(Id, PartUniqueIdDataItem.SubTypes.RAW_MATERIAL), partUniqueId.RawMaterial);
                DataItemManager.AddDataItem(new PartUniqueIdDataItem(Id, PartUniqueIdDataItem.SubTypes.SERIAL_NUMBER), partUniqueId.SerialNumber);
                DataItemManager.AddDataItem(new PartUniqueIdDataItem(Id, PartUniqueIdDataItem.SubTypes.UUID), partUniqueId.Uuid);
            }
        }


        private PartKindIdModel GetPartKindId()
        {
            var x = new PartKindIdModel();
            x.PartFamily = DataItemManager.GetDataItemValue(PartKindIdDataItem.NameId, PartKindIdDataItem.GetSubTypeId(PartKindIdDataItem.SubTypes.PART_FAMILY));
            x.PartName = DataItemManager.GetDataItemValue(PartKindIdDataItem.NameId, PartKindIdDataItem.GetSubTypeId(PartKindIdDataItem.SubTypes.PART_NAME));
            x.PartNumber = DataItemManager.GetDataItemValue(PartKindIdDataItem.NameId, PartKindIdDataItem.GetSubTypeId(PartKindIdDataItem.SubTypes.PART_NUMBER));
            x.Uuid = DataItemManager.GetDataItemValue(PartKindIdDataItem.NameId, PartKindIdDataItem.GetSubTypeId(PartKindIdDataItem.SubTypes.UUID));
            return x;
        }

        private void SetPartKindId(PartKindIdModel partKindId)
        {
            if (partKindId != null)
            {
                DataItemManager.AddDataItem(new PartKindIdDataItem(Id, PartKindIdDataItem.SubTypes.PART_FAMILY), partKindId.PartFamily);
                DataItemManager.AddDataItem(new PartKindIdDataItem(Id, PartKindIdDataItem.SubTypes.PART_NAME), partKindId.PartName);
                DataItemManager.AddDataItem(new PartKindIdDataItem(Id, PartKindIdDataItem.SubTypes.PART_NUMBER), partKindId.PartNumber);
                DataItemManager.AddDataItem(new PartKindIdDataItem(Id, PartKindIdDataItem.SubTypes.UUID), partKindId.Uuid);
            }
        }


        private PartCountModel GetPartCount()
        {
            var x = new PartCountModel();
            x.All = DataItemManager.GetDataItemValue(PartCountDataItem.NameId, PartCountDataItem.GetSubTypeId(PartCountDataItem.SubTypes.ALL)).ToInt();
            x.Good = DataItemManager.GetDataItemValue(PartCountDataItem.NameId, PartCountDataItem.GetSubTypeId(PartCountDataItem.SubTypes.GOOD)).ToInt();
            x.Bad = DataItemManager.GetDataItemValue(PartCountDataItem.NameId, PartCountDataItem.GetSubTypeId(PartCountDataItem.SubTypes.BAD)).ToInt();
            x.Target = DataItemManager.GetDataItemValue(PartCountDataItem.NameId, PartCountDataItem.GetSubTypeId(PartCountDataItem.SubTypes.TARGET)).ToInt();
            x.Remaining = DataItemManager.GetDataItemValue(PartCountDataItem.NameId, PartCountDataItem.GetSubTypeId(PartCountDataItem.SubTypes.REMAINING)).ToInt();
            x.Complete = DataItemManager.GetDataItemValue(PartCountDataItem.NameId, PartCountDataItem.GetSubTypeId(PartCountDataItem.SubTypes.COMPLETE)).ToInt();
            x.Failed = DataItemManager.GetDataItemValue(PartCountDataItem.NameId, PartCountDataItem.GetSubTypeId(PartCountDataItem.SubTypes.FAILED)).ToInt();
            x.Aborted = DataItemManager.GetDataItemValue(PartCountDataItem.NameId, PartCountDataItem.GetSubTypeId(PartCountDataItem.SubTypes.ABORTED)).ToInt();
            return x;
        }

        private void SetPartCount(PartCountModel partCount)
        {
            if (partCount != null)
            {
                DataItemManager.AddDataItem(new PartCountDataItem(Id, PartCountDataItem.SubTypes.ALL), partCount.All);
                DataItemManager.AddDataItem(new PartCountDataItem(Id, PartCountDataItem.SubTypes.GOOD), partCount.Good);
                DataItemManager.AddDataItem(new PartCountDataItem(Id, PartCountDataItem.SubTypes.BAD), partCount.Bad);
                DataItemManager.AddDataItem(new PartCountDataItem(Id, PartCountDataItem.SubTypes.TARGET), partCount.Target);
                DataItemManager.AddDataItem(new PartCountDataItem(Id, PartCountDataItem.SubTypes.REMAINING), partCount.Remaining);
                DataItemManager.AddDataItem(new PartCountDataItem(Id, PartCountDataItem.SubTypes.COMPLETE), partCount.Complete);
                DataItemManager.AddDataItem(new PartCountDataItem(Id, PartCountDataItem.SubTypes.FAILED), partCount.Failed);
                DataItemManager.AddDataItem(new PartCountDataItem(Id, PartCountDataItem.SubTypes.ABORTED), partCount.Aborted);
            }
        }
    }
}