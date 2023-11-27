// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Devices.DataItems.Conditions;
using MTConnect.Devices.DataItems.Events;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// System is an abstract Component that represents part(s) of a piece of equipment that is permanently integrated into the piece of equipment.
    /// </summary>
    public abstract class SystemModel : ComponentModel, ISystemModel
    {
        /// <summary>
        /// An indication of a fault associated with a piece of equipment or component that cannot be classified as a specific type.
        /// </summary>
        public Observations.IConditionObservation SystemCondition
        {
            get => DataItemManager.GetCondition(Devices.DataItems.Conditions.SystemCondition.NameId);
            set => DataItemManager.AddCondition(new SystemCondition(Id), value);
        }
        public IDataItemModel SystemConditionDataItem => DataItemManager.GetDataItem(Devices.DataItems.Conditions.SystemCondition.NameId);

        /// <summary>
        /// An indication of a fault associated with the hardware subsystem of the Structural Element.
        /// </summary>
        public Observations.IConditionObservation HardwareCondition
        {
            get => DataItemManager.GetCondition(Devices.DataItems.Conditions.HardwareCondition.NameId);
            set => DataItemManager.AddCondition(new HardwareCondition(Id), value);
        }
        public IDataItemModel HardwareConditionDataItem => DataItemManager.GetDataItem(Devices.DataItems.Conditions.HardwareCondition.NameId);

        /// <summary>
        /// An indication that the piece of equipment has experienced a communications failure.
        /// </summary>
        public Observations.IConditionObservation CommunicationsCondition
        {
            get => DataItemManager.GetCondition(Devices.DataItems.Conditions.CommunicationsCondition.NameId);
            set => DataItemManager.AddCondition(new CommunicationsCondition(Id), value);
        }
        public IDataItemModel CommunicationsConditionDataItem => DataItemManager.GetDataItem(Devices.DataItems.Conditions.CommunicationsCondition.NameId);

        /// <summary>
        /// Any text string of information to be transferred from a piece of equipment to a client software application.
        /// </summary>
        public string Message
        {
            get => DataItemManager.GetDataItemValue(DataItem.CreateId(Id, MessageDataItem.NameId));
            set => DataItemManager.AddDataItem(new MessageDataItem(Id), value);
        }
    }
}