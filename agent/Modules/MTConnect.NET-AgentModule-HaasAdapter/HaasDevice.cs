// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.Configurations;
using MTConnect.Devices.DataItems;
using System.Collections.Generic;
using System.Text;

namespace MTConnect.Haas
{
    /// <summary>
    /// Builds a Device model containing the DataItems reported by <see cref="HaasDataSource"/>.
    /// DataItem Names match <see cref="HaasDataItemKeys"/>; IDs are prefixed with the Device name
    /// so multiple Haas Devices can be hosted by the same Agent.
    /// </summary>
    public static class HaasDevice
    {
        /// <summary>
        /// Creates the Device model.
        /// </summary>
        /// <param name="name">The Device name.</param>
        /// <param name="uuid">The Device UUID. Defaults to <paramref name="name"/>.</param>
        /// <param name="configuration">The settings that determine which optional DataItems are included.</param>
        /// <param name="machineInfo">The model and serial number read from the control, if known.</param>
        /// <returns>A new Device.</returns>
        public static IDevice Create(string name, string uuid = null, HaasConfiguration configuration = null, HaasMachineInfo machineInfo = null)
        {
            if (configuration == null) configuration = new HaasConfiguration();

            var prefix = CreateIdPrefix(name);

            var device = new Device();
            device.Id = prefix;
            device.Name = name;
            device.Uuid = !string.IsNullOrEmpty(uuid) ? uuid : name;
            device.Description = new Description
            {
                Manufacturer = "Haas",
                Model = machineInfo?.Model,
                SerialNumber = machineInfo?.SerialNumber
            };

            // Work positions reference the work (OBJECT) coordinate system (MTConnect 2.0+)
            var workCoordinateSystemId = $"{prefix}_work";
            if (configuration.WorkPositions)
            {
                var workCoordinateSystem = new CoordinateSystem { Id = workCoordinateSystemId, Name = "work", Type = CoordinateSystemType.OBJECT };
                device.Configuration = new Configuration { CoordinateSystems = new List<ICoordinateSystem> { workCoordinateSystem } };
            }

            device.AddDataItem(Create(new AvailabilityDataItem(), prefix, HaasDataItemKeys.Availability));

            // Controller
            var controller = new ControllerComponent { Id = $"{prefix}_{ControllerComponent.NameId}", Name = ControllerComponent.NameId };
            var firmware = new FirmwareDataItem();
            firmware.SubType = FirmwareDataItem.SubTypes.VERSION.ToString();
            controller.AddDataItem(Create(firmware, prefix, HaasDataItemKeys.Firmware));
            controller.AddDataItem(CreateTimer(new EquipmentTimerDataItem(), EquipmentTimerDataItem.SubTypes.POWERED.ToString(), prefix, HaasDataItemKeys.PowerOnTime));
            controller.AddDataItem(CreateTimer(new EquipmentTimerDataItem(), EquipmentTimerDataItem.SubTypes.OPERATING.ToString(), prefix, HaasDataItemKeys.CycleStartTime));

            var path = new PathComponent { Id = $"{prefix}_{PathComponent.NameId}", Name = PathComponent.NameId };
            path.AddDataItem(Create(new ControllerModeDataItem(), prefix, HaasDataItemKeys.ControllerMode));
            path.AddDataItem(Create(new ExecutionDataItem(), prefix, HaasDataItemKeys.Execution));
            path.AddDataItem(Create(new ProgramDataItem(), prefix, HaasDataItemKeys.Program));
            path.AddDataItem(Create(new PartCountDataItem(), prefix, HaasDataItemKeys.PartCount));
            path.AddDataItem(Create(new PartCountDataItem(), prefix, HaasDataItemKeys.PartCount2));
            path.AddDataItem(Create(new ToolNumberDataItem(), prefix, HaasDataItemKeys.ToolNumber));
            path.AddDataItem(Create(new WorkOffsetDataItem(), prefix, HaasDataItemKeys.WorkOffset));
            path.AddDataItem(CreateOverride(ControllerModeOverrideDataItem.SubTypes.SINGLE_BLOCK.ToString(), prefix, HaasDataItemKeys.SingleBlock));
            path.AddDataItem(CreateOverride(ControllerModeOverrideDataItem.SubTypes.OPTIONAL_STOP.ToString(), prefix, HaasDataItemKeys.OptionalStop));
            path.AddDataItem(CreateTimer(new ProcessTimerDataItem(), ProcessTimerDataItem.SubTypes.PROCESS.ToString(), prefix, HaasDataItemKeys.LastCycleTime));
            path.AddDataItem(CreateTimer(new ProcessTimerDataItem(), ProcessTimerDataItem.SubTypes.PROCESS.ToString(), prefix, HaasDataItemKeys.PreviousCycleTime));
            path.AddDataItem(Create(new SystemDataItem(), prefix, HaasDataItemKeys.System));
            controller.AddComponent(path);
            device.AddComponent(controller);

            // Axes
            var axes = new AxesComponent { Id = $"{prefix}_{AxesComponent.NameId}", Name = AxesComponent.NameId };
            foreach (var axis in configuration.GetAxes())
            {
                axes.AddComponent(CreateAxis(prefix, axis, configuration.WorkPositions ? workCoordinateSystemId : null));
            }

            var spindle = new RotaryComponent { Id = $"{prefix}_s", Name = "S" };
            var speed = new RotaryVelocityDataItem();
            speed.SubType = RotaryVelocityDataItem.SubTypes.ACTUAL.ToString();
            spindle.AddDataItem(Create(speed, prefix, HaasDataItemKeys.SpindleSpeed));
            if (configuration.SpindleLoad) spindle.AddDataItem(Create(new LoadDataItem(), prefix, HaasDataItemKeys.SpindleLoad));
            axes.AddComponent(spindle);

            device.AddComponent(axes);

            // Systems
            if (configuration.CoolantLevel)
            {
                var coolant = new CoolantComponent { Id = $"{prefix}_coolant", Name = "coolant" };
                coolant.AddDataItem(Create(new FillLevelDataItem(), prefix, HaasDataItemKeys.CoolantLevel));
                device.AddComponent(coolant);
            }

            return device;
        }


        private static IComponent CreateAxis(string prefix, string name, string workCoordinateSystemId)
        {
            var id = $"{prefix}_{name.ToLowerInvariant()}";

            if (HaasConfiguration.IsRotaryAxis(name))
            {
                var rotary = new RotaryComponent { Id = id, Name = name };
                rotary.AddDataItem(CreateAngle(prefix, HaasDataItemKeys.GetPositionKey(name), DataItemCoordinateSystem.MACHINE, null));
                if (workCoordinateSystemId != null) rotary.AddDataItem(CreateAngle(prefix, HaasDataItemKeys.GetWorkPositionKey(name), DataItemCoordinateSystem.WORK, workCoordinateSystemId));
                return rotary;
            }

            var linear = new LinearComponent { Id = id, Name = name };
            linear.AddDataItem(CreatePosition(prefix, HaasDataItemKeys.GetPositionKey(name), DataItemCoordinateSystem.MACHINE, null));
            if (workCoordinateSystemId != null) linear.AddDataItem(CreatePosition(prefix, HaasDataItemKeys.GetWorkPositionKey(name), DataItemCoordinateSystem.WORK, workCoordinateSystemId));
            return linear;
        }

        private static IDataItem CreatePosition(string prefix, string key, DataItemCoordinateSystem coordinateSystem, string coordinateSystemIdRef)
        {
            // CoordinateSystem is used before MTConnect 2.0; CoordinateSystemIdRef from 2.0
            var position = new PositionDataItem();
            position.SubType = PositionDataItem.SubTypes.ACTUAL.ToString();
            position.CoordinateSystem = coordinateSystem;
            position.CoordinateSystemIdRef = coordinateSystemIdRef;
            return Create(position, prefix, key);
        }

        private static IDataItem CreateAngle(string prefix, string key, DataItemCoordinateSystem coordinateSystem, string coordinateSystemIdRef)
        {
            var angle = new AngleDataItem();
            angle.SubType = AngleDataItem.SubTypes.ACTUAL.ToString();
            angle.CoordinateSystem = coordinateSystem;
            angle.CoordinateSystemIdRef = coordinateSystemIdRef;
            return Create(angle, prefix, key);
        }

        private static IDataItem CreateTimer(DataItem dataItem, string subType, string prefix, string key)
        {
            dataItem.SubType = subType;
            return Create(dataItem, prefix, key);
        }

        private static IDataItem CreateOverride(string subType, string prefix, string key)
        {
            var dataItem = new ControllerModeOverrideDataItem();
            dataItem.SubType = subType;
            return Create(dataItem, prefix, key);
        }

        private static IDataItem Create(DataItem dataItem, string prefix, string key)
        {
            dataItem.Id = $"{prefix}_{key}";
            dataItem.Name = key;
            return dataItem;
        }

        private static string CreateIdPrefix(string name)
        {
            // IDs must be valid XML IDs (letters, digits, '_', '-', '.'; not starting with a digit)
            var builder = new StringBuilder();
            foreach (var c in name ?? "")
            {
                builder.Append(char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == '.' ? c : '_');
            }

            if (builder.Length == 0 || !(char.IsLetter(builder[0]) || builder[0] == '_')) builder.Insert(0, '_');

            return builder.ToString();
        }
    }
}
