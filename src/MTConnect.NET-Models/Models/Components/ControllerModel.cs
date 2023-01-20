// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.DataItems.Conditions;
using MTConnect.Devices.DataItems.Events;
using MTConnect.Observations.Events.Values;
using MTConnect.Models.DataItems;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Models.Components
{
    /// <summary>
    /// Controller represents the computational regulation and management function of a piece of equipment.
    /// </summary>
    public class ControllerModel : ComponentModel, IControllerModel
    {
        /// <summary>
        /// The current state of the emergency stop signal for a piece of equipment, controller path, or any other component or subsystem of a piece of equipment.
        /// </summary>
        public EmergencyStop EmergencyStop
        {
            get => DataItemManager.GetDataItemValue<EmergencyStop>(Devices.DataItems.Events.EmergencyStopDataItem.TypeId);
            set => DataItemManager.AddDataItem(new EmergencyStopDataItem(Id), value);
        }
        public IDataItemModel EmergencyStopDataItem => DataItemManager.GetDataItem(Devices.DataItems.Events.EmergencyStopDataItem.TypeId);


        /// <summary>
        /// The current operating mode of the Controller component.
        /// </summary>
        public ControllerMode ControllerMode
        {
            get => DataItemManager.GetDataItemValue<ControllerMode>(Devices.DataItems.Events.ControllerModeDataItem.NameId);
            set => DataItemManager.AddDataItem(new ControllerModeDataItem(Id), value);
        }
        public IDataItemModel ControllerModeDataItem => DataItemManager.GetDataItem(Devices.DataItems.Events.ControllerModeDataItem.NameId);


        /// <summary>
        /// A setting or operator selection that changes the behavior of a piece of equipment.
        /// </summary>
        public ControllerModeOverrideModel ControllerModeOverride
        {
            get => GetControllerModeOverride();
            set => SetControllerModeOverride(value);
        }


        /// <summary>
        /// The current intended production status of the device or component.
        /// </summary>
        public FunctionalMode FunctionalMode
        {
            get => DataItemManager.GetDataItemValue<FunctionalMode>(Devices.DataItems.Events.FunctionalModeDataItem.NameId);
            set => DataItemManager.AddDataItem(new FunctionalModeDataItem(Id), value);
        }
        public IDataItemModel FunctionalModeDataItem => DataItemManager.GetDataItem(Devices.DataItems.Events.FunctionalModeDataItem.NameId);


        /// <summary>
        /// An indication of a fault associated with a piece of equipment or component that cannot be classified as a specific type.
        /// </summary>
        public Observations.IConditionObservation SystemCondition
        {
            get => DataItemManager.GetCondition(Devices.DataItems.Conditions.SystemCondition.NameId);
            set => DataItemManager.AddCondition(new SystemCondition(Id), value);
        }

        /// <summary>
        /// An indication that an error occurred in the logic program or programmable logic controller(PLC) associated with a piece of equipment.
        /// </summary>
        public Observations.IConditionObservation LogicCondition
        {
            get => DataItemManager.GetCondition(LogicProgramCondition.NameId);
            set => DataItemManager.AddCondition(new LogicProgramCondition(Id), value);
        }

        /// <summary>
        /// An indication that an error occurred in the motion program associated with a piece of equipment.
        /// </summary>
        public Observations.IConditionObservation MotionCondition
        {
            get => DataItemManager.GetCondition(MotionProgramCondition.NameId);
            set => DataItemManager.AddCondition(new MotionProgramCondition(Id), value);
        }

        /// <summary>
        /// An indication that the piece of equipment has experienced a communications failure.
        /// </summary>
        public Observations.IConditionObservation CommunicationsCondition
        {
            get => DataItemManager.GetCondition(Devices.DataItems.Conditions.CommunicationsCondition.NameId);
            set => DataItemManager.AddCondition(new CommunicationsCondition(Id), value);
        }

        /// <summary>
        /// Path is a Component that represents the information for an independent operation or function within a Controller.
        /// </summary>
        public IEnumerable<IPathModel> Paths
        {
            get
            {
                var x = new List<IPathModel>();

                if (!ComponentModels.IsNullOrEmpty())
                {
                    var paths = ComponentModels.Where(o => o.Type == PathComponent.TypeId);
                    if (!paths.IsNullOrEmpty())
                    {
                        foreach (var path in paths) x.Add((PathModel)path);
                    }
                }

                return x;
            }
        }


        public ControllerModel() 
        {
            Type = ControllerComponent.TypeId;
        }

        public ControllerModel(string componentId)
        {
            Id = componentId;
            Type = ControllerComponent.TypeId;
        }


        //public IPathModel AddPath(string name) => 

        ////public virtual PathModel AddPath(string name)
        ////{
        ////    if (!string.IsNullOrEmpty(name))
        ////    {
        ////        var model = new PathModel
        ////        {
        ////            Id = CreateId(Id, name),
        ////            Name = name
        ////        };

        ////        AddComponentModel(model);
        ////        return model;
        ////    }

        ////    return null;
        ////}

        public IPathModel GetPath(string name) => ComponentManager.GetComponentModel<PathModel>(typeof(PathComponent), name);

        //public IPathModel GetPath(string name)
        //{
        //    return Paths?.OfType<PathModel>().FirstOrDefault(o => o.Name == name);
        //}

        //public IEnumerable<T> GetPaths<T>() where T : PathModel
        //{
        //    var x = new List<T>();
        //    if (!Paths.IsNullOrEmpty())
        //    {
        //        foreach (var path in Paths) x.Add((T)path);
        //    }
        //    return x;
        //}

        //public T GetPath<T>(string name) where T : PathModel
        //{
        //    return (T)GetPath(name);
        //}


        private ControllerModeOverrideModel GetControllerModeOverride()
        {
            var x = new ControllerModeOverrideModel();

            x.DryRun = DataItemManager.GetDataItemValue<ControllerModeOverrideValue>(ControllerModeOverrideDataItem.TypeId, ControllerModeOverrideDataItem.SubTypes.DRY_RUN.ToString());
            x.DryRunDataItem = DataItemManager.GetDataItem(ControllerModeOverrideDataItem.TypeId, ControllerModeOverrideDataItem.SubTypes.DRY_RUN.ToString());

            x.SingleBlock = DataItemManager.GetDataItemValue<ControllerModeOverrideValue>(ControllerModeOverrideDataItem.TypeId, ControllerModeOverrideDataItem.SubTypes.SINGLE_BLOCK.ToString());
            x.SingleBlockDataItem = DataItemManager.GetDataItem(ControllerModeOverrideDataItem.TypeId, ControllerModeOverrideDataItem.SubTypes.SINGLE_BLOCK.ToString());

            x.MachineAxisLock = DataItemManager.GetDataItemValue<ControllerModeOverrideValue>(ControllerModeOverrideDataItem.TypeId, ControllerModeOverrideDataItem.SubTypes.MACHINE_AXIS_LOCK.ToString());
            x.MachineAxisLockDataItem = DataItemManager.GetDataItem(ControllerModeOverrideDataItem.TypeId, ControllerModeOverrideDataItem.SubTypes.MACHINE_AXIS_LOCK.ToString());

            x.OptionalStop = DataItemManager.GetDataItemValue<ControllerModeOverrideValue>(ControllerModeOverrideDataItem.TypeId, ControllerModeOverrideDataItem.SubTypes.OPTIONAL_STOP.ToString());
            x.OptionalStopDataItem = DataItemManager.GetDataItem(ControllerModeOverrideDataItem.TypeId, ControllerModeOverrideDataItem.SubTypes.OPTIONAL_STOP.ToString());

            x.ToolChangeStop = DataItemManager.GetDataItemValue<ControllerModeOverrideValue>(ControllerModeOverrideDataItem.TypeId, ControllerModeOverrideDataItem.SubTypes.TOOL_CHANGE_STOP.ToString());
            x.ToolChangeStopDataItem = DataItemManager.GetDataItem(ControllerModeOverrideDataItem.TypeId, ControllerModeOverrideDataItem.SubTypes.TOOL_CHANGE_STOP.ToString());

            return x;

        }

        private void SetControllerModeOverride(ControllerModeOverrideModel controllerModeOverride)
        {
            if (controllerModeOverride != null)
            {
                DataItemManager.AddDataItem(new ControllerModeOverrideDataItem(Id, ControllerModeOverrideDataItem.SubTypes.DRY_RUN), controllerModeOverride?.DryRun);
                DataItemManager.AddDataItem(new ControllerModeOverrideDataItem(Id, ControllerModeOverrideDataItem.SubTypes.SINGLE_BLOCK), controllerModeOverride?.SingleBlock);
                DataItemManager.AddDataItem(new ControllerModeOverrideDataItem(Id, ControllerModeOverrideDataItem.SubTypes.MACHINE_AXIS_LOCK), controllerModeOverride?.MachineAxisLock);
                DataItemManager.AddDataItem(new ControllerModeOverrideDataItem(Id, ControllerModeOverrideDataItem.SubTypes.OPTIONAL_STOP), controllerModeOverride?.OptionalStop);
                DataItemManager.AddDataItem(new ControllerModeOverrideDataItem(Id, ControllerModeOverrideDataItem.SubTypes.TOOL_CHANGE_STOP), controllerModeOverride?.ToolChangeStop);
            }
        }
    }
}
