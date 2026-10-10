// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.Configurations;
using MTConnect.Devices.DataItems;
using System.Collections.Generic;
using System.Text;

namespace MTConnect.Fanuc
{
    /// <summary>
    /// Builds a Device model containing the DataItems reported by <see cref="FanucDataSource"/>
    /// for the paths, axes and spindles read from the CNC. DataItem Names match
    /// <see cref="FanucDataItemKeys"/>; IDs are prefixed with the Device name so multiple
    /// Fanuc Devices can be hosted by the same Agent.
    /// </summary>
    public static class FanucDevice
    {
        /// <summary>
        /// Creates the Device model.
        /// </summary>
        /// <param name="name">The Device name.</param>
        /// <param name="uuid">The Device UUID. Defaults to <paramref name="name"/>.</param>
        /// <param name="configuration">The settings that determine which optional DataItems are included.</param>
        /// <param name="machineModel">The structure read from the CNC, or <c>null</c> before it has been read (a single path with no axes).</param>
        /// <returns>A new Device.</returns>
        public static IDevice Create(string name, string uuid = null, FanucConfiguration configuration = null, FanucMachineModel machineModel = null)
        {
            if (configuration == null) configuration = new FanucConfiguration();
            if (machineModel == null) machineModel = FanucMachineModel.CreateDefault();

            var prefix = CreateIdPrefix(name);

            var device = new Device();
            device.Id = prefix;
            device.Name = name;
            device.Uuid = !string.IsNullOrEmpty(uuid) ? uuid : name;
            device.Description = new Description { Manufacturer = "Fanuc", Model = machineModel.Model };

            // Work positions reference the work (OBJECT) coordinate system (MTConnect 2.0+)
            var workCoordinateSystemId = configuration.WorkPositions ? $"{prefix}_work" : null;
            if (workCoordinateSystemId != null)
            {
                var workCoordinateSystem = new CoordinateSystem { Id = workCoordinateSystemId, Name = "work", Type = CoordinateSystemType.OBJECT };
                device.Configuration = new Configuration { CoordinateSystems = new List<ICoordinateSystem> { workCoordinateSystem } };
            }

            device.AddDataItem(Create(new AvailabilityDataItem(), prefix, FanucDataItemKeys.Availability));

            // Controller
            var controller = new ControllerComponent { Id = $"{prefix}_{ControllerComponent.NameId}", Name = ControllerComponent.NameId };
            controller.AddDataItem(Create(new EmergencyStopDataItem(), prefix, FanucDataItemKeys.EmergencyStop));
            controller.AddDataItem(Create(new FirmwareDataItem { SubType = FirmwareDataItem.SubTypes.VERSION.ToString() }, prefix, FanucDataItemKeys.Firmware));
            controller.AddDataItem(Create(new EquipmentTimerDataItem { SubType = EquipmentTimerDataItem.SubTypes.POWERED.ToString() }, prefix, FanucDataItemKeys.PowerOnTime));
            controller.AddDataItem(Create(new EquipmentTimerDataItem { SubType = EquipmentTimerDataItem.SubTypes.OPERATING.ToString() }, prefix, FanucDataItemKeys.OperatingTime));
            controller.AddDataItem(Create(new EquipmentTimerDataItem { SubType = EquipmentTimerDataItem.SubTypes.WORKING.ToString() }, prefix, FanucDataItemKeys.CuttingTime));
            controller.AddDataItem(Create(new ProcessTimerDataItem { SubType = ProcessTimerDataItem.SubTypes.PROCESS.ToString() }, prefix, FanucDataItemKeys.CycleTime));

            foreach (var path in machineModel.Paths)
            {
                controller.AddComponent(CreatePath(prefix, path.Number, configuration));
            }

            device.AddComponent(controller);

            // Axes
            var axes = new AxesComponent { Id = $"{prefix}_{AxesComponent.NameId}", Name = AxesComponent.NameId };
            foreach (var path in machineModel.Paths)
            {
                foreach (var axis in path.Axes)
                {
                    axes.AddComponent(CreateAxis(prefix, axis, configuration, workCoordinateSystemId));
                }

                axes.AddComponent(CreateSpindle(prefix, path, configuration));
            }

            device.AddComponent(axes);

            return device;
        }


        private static IComponent CreatePath(string prefix, int number, FanucConfiguration configuration)
        {
            var name = number <= 1 ? PathComponent.NameId : $"{PathComponent.NameId}{number}";
            var path = new PathComponent { Id = $"{prefix}_{name}", Name = name };

            path.AddDataItem(Create(new ControllerModeDataItem(), prefix, FanucDataItemKeys.ForPath(FanucDataItemKeys.ControllerMode, number)));
            path.AddDataItem(Create(new ExecutionDataItem(), prefix, FanucDataItemKeys.ForPath(FanucDataItemKeys.Execution, number)));
            path.AddDataItem(Create(new ProgramDataItem(), prefix, FanucDataItemKeys.ForPath(FanucDataItemKeys.Program, number)));
            path.AddDataItem(Create(new LineLabelDataItem(), prefix, FanucDataItemKeys.ForPath(FanucDataItemKeys.Line, number)));
            if (configuration.PartCountParameter > 0) path.AddDataItem(Create(new PartCountDataItem(), prefix, FanucDataItemKeys.ForPath(FanucDataItemKeys.PartCount, number)));
            if (configuration.ToolNumberVariable > 0) path.AddDataItem(Create(new ToolNumberDataItem(), prefix, FanucDataItemKeys.ForPath(FanucDataItemKeys.ToolNumber, number)));
            if (configuration.WorkOffset) path.AddDataItem(Create(new WorkOffsetDataItem(), prefix, FanucDataItemKeys.ForPath(FanucDataItemKeys.WorkOffset, number)));
            path.AddDataItem(Create(new PathFeedrateDataItem { SubType = PathFeedrateDataItem.SubTypes.ACTUAL.ToString() }, prefix, FanucDataItemKeys.ForPath(FanucDataItemKeys.Feedrate, number)));

            if (configuration.Overrides)
            {
                path.AddDataItem(Create(new PathFeedrateOverrideDataItem { SubType = PathFeedrateOverrideDataItem.SubTypes.PROGRAMMED.ToString() }, prefix, FanucDataItemKeys.ForPath(FanucDataItemKeys.FeedrateOverride, number)));
                path.AddDataItem(Create(new PathFeedrateOverrideDataItem { SubType = PathFeedrateOverrideDataItem.SubTypes.RAPID.ToString() }, prefix, FanucDataItemKeys.ForPath(FanucDataItemKeys.RapidOverride, number)));
            }

            path.AddDataItem(Create(new SystemDataItem(), prefix, FanucDataItemKeys.ForPath(FanucDataItemKeys.System, number)));

            return path;
        }

        private static IComponent CreateAxis(string prefix, FanucAxisModel axis, FanucConfiguration configuration, string workCoordinateSystemId)
        {
            var id = $"{prefix}_{CreateIdPrefix(axis.Name).ToLowerInvariant()}";
            var component = axis.IsRotary
                ? (Component)new RotaryComponent { Id = id, Name = axis.Name }
                : new LinearComponent { Id = id, Name = axis.Name };

            component.AddDataItem(CreatePosition(prefix, FanucDataItemKeys.GetPositionKey(axis.Name), axis.IsRotary, DataItemCoordinateSystem.MACHINE, null));
            if (workCoordinateSystemId != null)
            {
                component.AddDataItem(CreatePosition(prefix, FanucDataItemKeys.GetWorkPositionKey(axis.Name), axis.IsRotary, DataItemCoordinateSystem.WORK, workCoordinateSystemId));
            }

            if (configuration.AxisLoads) component.AddDataItem(Create(new LoadDataItem(), prefix, FanucDataItemKeys.GetLoadKey(axis.Name)));

            return component;
        }

        private static IComponent CreateSpindle(string prefix, FanucPathModel path, FanucConfiguration configuration)
        {
            var name = !string.IsNullOrEmpty(path.Spindle) ? path.Spindle : $"S{path.Number}";
            var spindle = new RotaryComponent { Id = $"{prefix}_{CreateIdPrefix(name).ToLowerInvariant()}", Name = name };

            var speed = new RotaryVelocityDataItem { SubType = RotaryVelocityDataItem.SubTypes.ACTUAL.ToString() };
            spindle.AddDataItem(Create(speed, prefix, FanucDataItemKeys.ForPath(FanucDataItemKeys.SpindleSpeed, path.Number)));
            if (configuration.SpindleLoad) spindle.AddDataItem(Create(new LoadDataItem(), prefix, FanucDataItemKeys.ForPath(FanucDataItemKeys.SpindleLoad, path.Number)));
            if (configuration.Overrides) spindle.AddDataItem(Create(new RotaryVelocityOverrideDataItem(), prefix, FanucDataItemKeys.ForPath(FanucDataItemKeys.SpindleOverride, path.Number)));

            return spindle;
        }

        private static IDataItem CreatePosition(string prefix, string key, bool isRotary, DataItemCoordinateSystem coordinateSystem, string coordinateSystemIdRef)
        {
            // CoordinateSystem is used before MTConnect 2.0; CoordinateSystemIdRef from 2.0
            DataItem dataItem;
            if (isRotary) dataItem = new AngleDataItem { SubType = AngleDataItem.SubTypes.ACTUAL.ToString() };
            else dataItem = new PositionDataItem { SubType = PositionDataItem.SubTypes.ACTUAL.ToString() };

            dataItem.CoordinateSystem = coordinateSystem;
            dataItem.CoordinateSystemIdRef = coordinateSystemIdRef;
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
