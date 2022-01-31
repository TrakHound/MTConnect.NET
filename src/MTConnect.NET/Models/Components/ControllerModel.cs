// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.Conditions;
using MTConnect.Devices.Events;
using MTConnect.Streams.Events;
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
            get => GetDataItemValue<EmergencyStop>(DataItem.CreateId(Id, Devices.Events.EmergencyStopDataItem.NameId));
            set => AddDataItem(new EmergencyStopDataItem(Id), value);
        }
        public IDataItemModel EmergencyStopDataItem => GetDataItem(Devices.Events.EmergencyStopDataItem.NameId);


        /// <summary>
        /// The current operating mode of the Controller component.
        /// </summary>
        public ControllerMode ControllerMode
        {
            get => GetDataItemValue<ControllerMode>(DataItem.CreateId(Id, Devices.Events.ControllerModeDataItem.NameId));
            set => AddDataItem(new ControllerModeDataItem(Id), value);
        }
        public IDataItemModel ControllerModeDataItem => GetDataItem(Devices.Events.ControllerModeDataItem.NameId);


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
            get => GetDataItemValue<FunctionalMode>(DataItem.CreateId(Id, Devices.Events.FunctionalModeDataItem.NameId));
            set => AddDataItem(new FunctionalModeDataItem(Id), value);
        }
        public IDataItemModel FunctionalModeDataItem => GetDataItem(Devices.Events.FunctionalModeDataItem.NameId);


        /// <summary>
        /// An indication of a fault associated with a piece of equipment or component that cannot be classified as a specific type.
        /// </summary>
        public Streams.Condition SystemCondition
        {
            get => GetCondition(Devices.Conditions.SystemCondition.NameId);
            set => AddCondition(new SystemCondition(Id), value);
        }

        /// <summary>
        /// An indication that an error occurred in the logic program or programmable logic controller(PLC) associated with a piece of equipment.
        /// </summary>
        public Streams.Condition LogicCondition
        {
            get => GetCondition(LogicProgramCondition.NameId);
            set => AddCondition(new LogicProgramCondition(Id), value);
        }

        /// <summary>
        /// An indication that an error occurred in the motion program associated with a piece of equipment.
        /// </summary>
        public Streams.Condition MotionCondition
        {
            get => GetCondition(MotionProgramCondition.NameId);
            set => AddCondition(new MotionProgramCondition(Id), value);
        }

        /// <summary>
        /// An indication that the piece of equipment has experienced a communications failure.
        /// </summary>
        public Streams.Condition CommunicationsCondition
        {
            get => GetCondition(Devices.Conditions.CommunicationsCondition.NameId);
            set => AddCondition(new CommunicationsCondition(Id), value);
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

        public IPathModel GetPath(string name) => GetComponentModel<PathModel>(typeof(PathComponent), name);

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

            x.DryRun = GetDataItemValue<ControllerModeOverrideValue>(DataItem.CreateId(ControllerModeOverrideDataItem.NameId, ControllerModeOverrideDataItem.GetSubTypeId(ControllerModeOverrideDataItem.SubTypes.DRY_RUN)));
            x.DryRunDataItem = GetDataItem(ControllerModeOverrideDataItem.NameId, ControllerModeOverrideDataItem.GetSubTypeId(ControllerModeOverrideDataItem.SubTypes.DRY_RUN));

            x.SingleBlock = GetDataItemValue<ControllerModeOverrideValue>(DataItem.CreateId(ControllerModeOverrideDataItem.NameId, ControllerModeOverrideDataItem.GetSubTypeId(ControllerModeOverrideDataItem.SubTypes.SINGLE_BLOCK)));
            x.SingleBlockDataItem = GetDataItem(ControllerModeOverrideDataItem.NameId, ControllerModeOverrideDataItem.GetSubTypeId(ControllerModeOverrideDataItem.SubTypes.SINGLE_BLOCK));

            x.MachineAxisLock = GetDataItemValue<ControllerModeOverrideValue>(DataItem.CreateId(ControllerModeOverrideDataItem.NameId, ControllerModeOverrideDataItem.GetSubTypeId(ControllerModeOverrideDataItem.SubTypes.MACHINE_AXIS_LOCK)));
            x.MachineAxisLockDataItem = GetDataItem(ControllerModeOverrideDataItem.NameId, ControllerModeOverrideDataItem.GetSubTypeId(ControllerModeOverrideDataItem.SubTypes.MACHINE_AXIS_LOCK));

            x.OptionalStop = GetDataItemValue<ControllerModeOverrideValue>(DataItem.CreateId(ControllerModeOverrideDataItem.NameId, ControllerModeOverrideDataItem.GetSubTypeId(ControllerModeOverrideDataItem.SubTypes.OPTIONAL_STOP)));
            x.OptionalStopDataItem = GetDataItem(ControllerModeOverrideDataItem.NameId, ControllerModeOverrideDataItem.GetSubTypeId(ControllerModeOverrideDataItem.SubTypes.OPTIONAL_STOP));

            x.ToolChangeStop = GetDataItemValue<ControllerModeOverrideValue>(DataItem.CreateId(ControllerModeOverrideDataItem.NameId, ControllerModeOverrideDataItem.GetSubTypeId(ControllerModeOverrideDataItem.SubTypes.TOOL_CHANGE_STOP)));
            x.ToolChangeStopDataItem = GetDataItem(ControllerModeOverrideDataItem.NameId, ControllerModeOverrideDataItem.GetSubTypeId(ControllerModeOverrideDataItem.SubTypes.TOOL_CHANGE_STOP));

            return x;

        }

        private void SetControllerModeOverride(ControllerModeOverrideModel controllerModeOverride)
        {
            if (controllerModeOverride != null)
            {
                AddDataItem(new ControllerModeOverrideDataItem(Id, ControllerModeOverrideDataItem.SubTypes.DRY_RUN), controllerModeOverride?.DryRun);
                AddDataItem(new ControllerModeOverrideDataItem(Id, ControllerModeOverrideDataItem.SubTypes.SINGLE_BLOCK), controllerModeOverride?.SingleBlock);
                AddDataItem(new ControllerModeOverrideDataItem(Id, ControllerModeOverrideDataItem.SubTypes.MACHINE_AXIS_LOCK), controllerModeOverride?.MachineAxisLock);
                AddDataItem(new ControllerModeOverrideDataItem(Id, ControllerModeOverrideDataItem.SubTypes.OPTIONAL_STOP), controllerModeOverride?.OptionalStop);
                AddDataItem(new ControllerModeOverrideDataItem(Id, ControllerModeOverrideDataItem.SubTypes.TOOL_CHANGE_STOP), controllerModeOverride?.ToolChangeStop);
            }
        }
    }
}
