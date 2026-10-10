// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Devices.Components;
using MTConnect.Devices.DataItems;
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
        /// <returns>A new Device.</returns>
        public static IDevice Create(string name, string uuid = null)
        {
            var prefix = CreateIdPrefix(name);

            var device = new Device();
            device.Id = prefix;
            device.Name = name;
            device.Uuid = !string.IsNullOrEmpty(uuid) ? uuid : name;
            device.Description = new Description { Manufacturer = "Haas" };

            device.AddDataItem(Create(new AvailabilityDataItem(), prefix, HaasDataItemKeys.Availability));

            // Controller
            var controller = new ControllerComponent { Id = $"{prefix}_{ControllerComponent.NameId}", Name = ControllerComponent.NameId };
            var path = new PathComponent { Id = $"{prefix}_{PathComponent.NameId}", Name = PathComponent.NameId };
            path.AddDataItem(Create(new ControllerModeDataItem(), prefix, HaasDataItemKeys.ControllerMode));
            path.AddDataItem(Create(new ExecutionDataItem(), prefix, HaasDataItemKeys.Execution));
            path.AddDataItem(Create(new ProgramDataItem(), prefix, HaasDataItemKeys.Program));
            path.AddDataItem(Create(new PartCountDataItem(), prefix, HaasDataItemKeys.PartCount));
            path.AddDataItem(Create(new SystemDataItem(), prefix, HaasDataItemKeys.System));
            controller.AddComponent(path);
            device.AddComponent(controller);

            // Axes
            var axes = new AxesComponent { Id = $"{prefix}_{AxesComponent.NameId}", Name = AxesComponent.NameId };
            axes.AddComponent(CreateLinearAxis(prefix, "X", HaasDataItemKeys.XPosition));
            axes.AddComponent(CreateLinearAxis(prefix, "Y", HaasDataItemKeys.YPosition));
            axes.AddComponent(CreateLinearAxis(prefix, "Z", HaasDataItemKeys.ZPosition));

            var spindle = new RotaryComponent { Id = $"{prefix}_s", Name = "S" };
            var speed = new RotaryVelocityDataItem();
            speed.SubType = RotaryVelocityDataItem.SubTypes.ACTUAL.ToString();
            spindle.AddDataItem(Create(speed, prefix, HaasDataItemKeys.SpindleSpeed));
            axes.AddComponent(spindle);

            device.AddComponent(axes);

            return device;
        }


        private static IComponent CreateLinearAxis(string prefix, string name, string positionKey)
        {
            var axis = new LinearComponent { Id = $"{prefix}_{name.ToLowerInvariant()}", Name = name };

            var position = new PositionDataItem();
            position.SubType = PositionDataItem.SubTypes.ACTUAL.ToString();
            axis.AddDataItem(Create(position, prefix, positionKey));

            return axis;
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
