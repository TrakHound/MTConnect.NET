// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.Configurations;
using MTConnect.Devices.DataItems;
using System.Collections.Generic;
using System.Text;

namespace MTConnect.Tormach
{
    /// <summary>
    /// Builds a Device model containing the DataItems reported by <see cref="TormachDataSource"/>
    /// for the axes configured in PathPilot. DataItem Names match <see cref="TormachDataItemKeys"/>;
    /// IDs are prefixed with the Device name so multiple Tormach Devices can be hosted by the same Agent.
    /// </summary>
    public static class TormachDevice
    {
        /// <summary>
        /// Creates the Device model.
        /// </summary>
        /// <param name="name">The Device name.</param>
        /// <param name="uuid">The Device UUID. Defaults to <paramref name="name"/>.</param>
        /// <param name="configuration">The settings that determine which optional DataItems are included.</param>
        /// <param name="machineInfo">The machine configuration from the status server, or <c>null</c> before it has been received (X, Y, Z).</param>
        /// <returns>A new Device.</returns>
        public static IDevice Create(string name, string uuid = null, TormachConfiguration configuration = null, TormachInfo machineInfo = null)
        {
            if (configuration == null) configuration = new TormachConfiguration();
            var axisLetters = machineInfo != null ? machineInfo.GetAxes() : new[] { "X", "Y", "Z" };

            var prefix = CreateIdPrefix(name);

            var device = new Device();
            device.Id = prefix;
            device.Name = name;
            device.Uuid = !string.IsNullOrEmpty(uuid) ? uuid : name;
            device.Description = new Description { Manufacturer = "Tormach", Model = machineInfo?.Machine };

            // Work positions reference the work (OBJECT) coordinate system (MTConnect 2.0+)
            var workCoordinateSystemId = configuration.WorkPositions ? $"{prefix}_work" : null;
            if (workCoordinateSystemId != null)
            {
                var workCoordinateSystem = new CoordinateSystem { Id = workCoordinateSystemId, Name = "work", Type = CoordinateSystemType.OBJECT };
                device.Configuration = new Configuration { CoordinateSystems = new List<ICoordinateSystem> { workCoordinateSystem } };
            }

            device.AddDataItem(Create(new AvailabilityDataItem(), prefix, TormachDataItemKeys.Availability));

            // Controller
            var controller = new ControllerComponent { Id = $"{prefix}_{ControllerComponent.NameId}", Name = ControllerComponent.NameId };
            controller.AddDataItem(Create(new EmergencyStopDataItem(), prefix, TormachDataItemKeys.EmergencyStop));

            var path = new PathComponent { Id = $"{prefix}_{PathComponent.NameId}", Name = PathComponent.NameId };
            path.AddDataItem(Create(new ControllerModeDataItem(), prefix, TormachDataItemKeys.ControllerMode));
            path.AddDataItem(Create(new ExecutionDataItem(), prefix, TormachDataItemKeys.Execution));
            path.AddDataItem(Create(new ProgramDataItem(), prefix, TormachDataItemKeys.Program));
            path.AddDataItem(Create(new LineNumberDataItem { SubType = LineNumberDataItem.SubTypes.ABSOLUTE.ToString() }, prefix, TormachDataItemKeys.Line));
            path.AddDataItem(Create(new ToolNumberDataItem(), prefix, TormachDataItemKeys.ToolNumber));
            path.AddDataItem(Create(new WorkOffsetDataItem(), prefix, TormachDataItemKeys.WorkOffset));
            path.AddDataItem(Create(new ControllerModeOverrideDataItem { SubType = ControllerModeOverrideDataItem.SubTypes.OPTIONAL_STOP.ToString() }, prefix, TormachDataItemKeys.OptionalStop));
            path.AddDataItem(Create(new PathFeedrateDataItem { SubType = PathFeedrateDataItem.SubTypes.ACTUAL.ToString() }, prefix, TormachDataItemKeys.Feedrate));
            path.AddDataItem(Create(new PathFeedrateOverrideDataItem { SubType = PathFeedrateOverrideDataItem.SubTypes.PROGRAMMED.ToString() }, prefix, TormachDataItemKeys.FeedrateOverride));
            path.AddDataItem(Create(new PathFeedrateOverrideDataItem { SubType = PathFeedrateOverrideDataItem.SubTypes.RAPID.ToString() }, prefix, TormachDataItemKeys.RapidOverride));
            controller.AddComponent(path);
            device.AddComponent(controller);

            // Axes
            var axes = new AxesComponent { Id = $"{prefix}_{AxesComponent.NameId}", Name = AxesComponent.NameId };
            foreach (var axis in axisLetters)
            {
                axes.AddComponent(CreateAxis(prefix, axis, workCoordinateSystemId));
            }

            var spindle = new RotaryComponent { Id = $"{prefix}_s", Name = "S" };
            spindle.AddDataItem(Create(new RotaryVelocityDataItem { SubType = RotaryVelocityDataItem.SubTypes.COMMANDED.ToString() }, prefix, TormachDataItemKeys.SpindleSpeed));
            spindle.AddDataItem(Create(new RotaryVelocityOverrideDataItem(), prefix, TormachDataItemKeys.SpindleOverride));
            axes.AddComponent(spindle);

            device.AddComponent(axes);

            return device;
        }


        private static IComponent CreateAxis(string prefix, string axis, string workCoordinateSystemId)
        {
            var id = $"{prefix}_{axis.ToLowerInvariant()}";
            var rotary = TormachInfo.IsRotary(axis);
            var component = rotary
                ? (Component)new RotaryComponent { Id = id, Name = axis }
                : new LinearComponent { Id = id, Name = axis };

            component.AddDataItem(CreatePosition(prefix, TormachDataItemKeys.GetPositionKey(axis), rotary, DataItemCoordinateSystem.MACHINE, null));
            if (workCoordinateSystemId != null)
            {
                component.AddDataItem(CreatePosition(prefix, TormachDataItemKeys.GetWorkPositionKey(axis), rotary, DataItemCoordinateSystem.WORK, workCoordinateSystemId));
            }

            return component;
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
