// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Agents;
using MTConnect.Assets;
using MTConnect.Devices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTConnect.Modules.WebUI
{
    /// <summary>
    /// Collects the validation failures the Agent reports (invalid Devices, Components,
    /// Compositions, DataItems, Observations and Assets) so the UI can list them.
    /// Repeated failures of the same kind, target and code are merged into one entry
    /// with a count. Entries are kept in memory only and reset when the Agent restarts.
    /// </summary>
    internal class ValidationProblemTracker
    {
        /// <summary>The maximum number of distinct problems kept; the least recently seen is dropped beyond this.</summary>
        private const int MaxProblems = 1000;

        internal const string KindObservation = "observation";
        internal const string KindDataItem = "dataItem";
        internal const string KindComponent = "component";
        internal const string KindComposition = "composition";
        internal const string KindDevice = "device";
        internal const string KindAsset = "asset";

        private readonly IMTConnectAgentBroker _agent;
        private readonly Dictionary<string, ValidationProblem> _problems = new Dictionary<string, ValidationProblem>();
        private readonly object _lock = new object();


        /// <summary>
        /// Subscribes to the Agent's validation events.
        /// </summary>
        public ValidationProblemTracker(IMTConnectAgentBroker agent)
        {
            _agent = agent;
            if (_agent == null) return;

            _agent.InvalidObservationAdded += InvalidObservationAdded;
            _agent.InvalidDataItemAdded += InvalidDataItemAdded;
            _agent.InvalidComponentAdded += InvalidComponentAdded;
            _agent.InvalidCompositionAdded += InvalidCompositionAdded;
            _agent.InvalidDeviceAdded += InvalidDeviceAdded;
            _agent.InvalidAssetAdded += InvalidAssetAdded;
        }

        /// <summary>
        /// Unsubscribes from the Agent's validation events.
        /// </summary>
        public void Detach()
        {
            if (_agent == null) return;

            _agent.InvalidObservationAdded -= InvalidObservationAdded;
            _agent.InvalidDataItemAdded -= InvalidDataItemAdded;
            _agent.InvalidComponentAdded -= InvalidComponentAdded;
            _agent.InvalidCompositionAdded -= InvalidCompositionAdded;
            _agent.InvalidDeviceAdded -= InvalidDeviceAdded;
            _agent.InvalidAssetAdded -= InvalidAssetAdded;
        }


        /// <summary>
        /// Returns a snapshot of the problems, most recently seen first, optionally for a single Device.
        /// </summary>
        public List<ValidationProblem> GetProblems(string deviceUuid = null)
        {
            lock (_lock)
            {
                return _problems.Values
                    .Where(o => string.IsNullOrEmpty(deviceUuid) || o.DeviceUuid == deviceUuid)
                    .OrderByDescending(o => o.LastSeen)
                    .Select(o => o.Clone())
                    .ToList();
            }
        }

        /// <summary>
        /// Returns the most recent Observation problem for each DataItem, keyed by <see cref="ObservationKey"/>.
        /// </summary>
        public Dictionary<string, ValidationProblem> GetObservationProblems()
        {
            var result = new Dictionary<string, ValidationProblem>();
            lock (_lock)
            {
                foreach (var problem in _problems.Values)
                {
                    if (problem.Kind != KindObservation || problem.TargetId == null) continue;

                    var key = ObservationKey(problem.DeviceUuid, problem.TargetId);
                    if (!result.TryGetValue(key, out var existing) || problem.LastSeen > existing.LastSeen)
                    {
                        result[key] = problem.Clone();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Lookup key for a DataItem's Observation problems.
        /// </summary>
        public static string ObservationKey(string deviceUuid, string dataItemId) => deviceUuid + "|" + dataItemId;


        private void InvalidObservationAdded(string deviceUuid, string dataItemKey, ValidationResult result)
        {
            // The key may be a DataItem Id, Name or Source; resolve it so problems line up with the Data page.
            // An unknown key (no such DataItem on the Device) is still recorded under the key that was sent.
            IDataItem dataItem = null;
            try { if (deviceUuid != null && dataItemKey != null) dataItem = _agent.GetDataItem(deviceUuid, dataItemKey); } catch { }

            Add(KindObservation, deviceUuid, dataItem?.Id ?? dataItemKey, dataItem?.Name, dataItem?.Type, dataItem != null, result);
        }

        private void InvalidDataItemAdded(string deviceUuid, IDataItem dataItem, ValidationResult result)
        {
            Add(KindDataItem, deviceUuid, dataItem?.Id, dataItem?.Name, dataItem?.Type, true, result);
        }

        private void InvalidComponentAdded(string deviceUuid, IComponent component, ValidationResult result)
        {
            Add(KindComponent, deviceUuid, component?.Id, component?.Name, component?.Type, true, result);
        }

        private void InvalidCompositionAdded(string deviceUuid, IComposition composition, ValidationResult result)
        {
            Add(KindComposition, deviceUuid, composition?.Id, composition?.Name, composition?.Type, true, result);
        }

        private void InvalidDeviceAdded(IDevice device, ValidationResult result)
        {
            Add(KindDevice, device?.Uuid, device?.Id, device?.Name, device?.Type, true, result, device?.Name);
        }

        private void InvalidAssetAdded(IAsset asset, ValidationResult result)
        {
            Add(KindAsset, asset?.DeviceUuid, asset?.AssetId, null, asset?.Type, true, result);
        }

        private void Add(string kind, string deviceUuid, string targetId, string targetName, string targetType, bool targetFound, ValidationResult result, string deviceName = null)
        {
            if (result.IsValid) return;

            var now = DateTime.UtcNow;
            var message = !string.IsNullOrEmpty(result.Message) ? result.Message : result.Code;
            // One entry per failing item (and code, when the Agent provides one). Messages are not part of the
            // key because they usually embed the offending value, which would split one problem into many rows.
            var key = string.Join("|", kind, deviceUuid, targetId, result.Code);

            if (deviceName == null && deviceUuid != null)
            {
                try { deviceName = _agent.GetDevice(deviceUuid)?.Name; } catch { }
            }

            lock (_lock)
            {
                if (_problems.TryGetValue(key, out var problem))
                {
                    problem.Count++;
                    problem.LastSeen = now;
                    problem.Message = message;
                    return;
                }

                if (_problems.Count >= MaxProblems)
                {
                    var oldest = _problems.OrderBy(o => o.Value.LastSeen).First().Key;
                    _problems.Remove(oldest);
                }

                _problems[key] = new ValidationProblem
                {
                    Kind = kind,
                    DeviceUuid = deviceUuid,
                    DeviceName = deviceName,
                    TargetId = targetId,
                    TargetName = targetName,
                    TargetType = targetType,
                    TargetFound = targetFound,
                    Code = result.Code,
                    Message = message,
                    Count = 1,
                    FirstSeen = now,
                    LastSeen = now
                };
            }
        }
    }

    /// <summary>
    /// One distinct validation failure and how often it has been reported.
    /// </summary>
    internal class ValidationProblem
    {
        /// <summary>What failed: observation, dataItem, component, composition, device or asset.</summary>
        public string Kind { get; set; }

        /// <summary>UUID of the Device the failure belongs to (may be null for Assets without a Device).</summary>
        public string DeviceUuid { get; set; }

        /// <summary>Name of the Device, when it could be resolved.</summary>
        public string DeviceName { get; set; }

        /// <summary>Id of the failing entity (DataItem Id, Component Id, Asset Id, ...), or the key that was sent for an unknown DataItem.</summary>
        public string TargetId { get; set; }

        /// <summary>Name of the failing entity, when it has one.</summary>
        public string TargetName { get; set; }

        /// <summary>Type of the failing entity.</summary>
        public string TargetType { get; set; }

        /// <summary>False when an Observation was sent for a DataItem the Device does not define.</summary>
        public bool TargetFound { get; set; }

        /// <summary>Validation code reported by the Agent, if any.</summary>
        public string Code { get; set; }

        /// <summary>Most recent validation message (falls back to the code).</summary>
        public string Message { get; set; }

        /// <summary>Number of times this failure has been reported.</summary>
        public long Count { get; set; }

        /// <summary>When the failure was first reported (UTC).</summary>
        public DateTime FirstSeen { get; set; }

        /// <summary>When the failure was last reported (UTC).</summary>
        public DateTime LastSeen { get; set; }

        /// <summary>Returns a copy safe to serialize outside the tracker's lock.</summary>
        public ValidationProblem Clone() => (ValidationProblem)MemberwiseClone();
    }
}
