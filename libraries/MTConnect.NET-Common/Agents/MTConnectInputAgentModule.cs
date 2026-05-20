// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;
using MTConnect.Input;
using MTConnect.Logging;
using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Agents
{
    /// <summary>
    /// Base class for Agent modules that act as an input data source: the module contributes a Device, then periodically polls for new data and publishes observations and Assets to the host Agent. Derived classes supply the Device and reading logic by overriding the hook methods.
    /// </summary>
    public abstract class MTConnectInputAgentModule : IMTConnectAgentModule
    {
        private readonly IMTConnectAgentBroker _agent;
        private IDevice _device;
        private CancellationTokenSource _stop;
        private bool _isStarted;


        /// <summary>
        /// A unique identifier that distinguishes this module from other modules loaded by the Agent.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// A human-readable description of the data source this module represents.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The data-source configuration, including the read interval used by the polling loop.
        /// </summary>
        public IDataSourceConfiguration Configuration { get; set; }

        /// <summary>
        /// The Agent broker this module publishes observations and Assets to.
        /// </summary>
        public IMTConnectAgentBroker Agent => _agent;

        /// <summary>
        /// The Device this module contributed to the Agent, available once <see cref="StartBeforeLoad(bool)"/> has run.
        /// </summary>
        public IDevice Device => _device;


        /// <summary>
        /// Raised when the module emits a log entry, allowing the host Agent to surface module diagnostics.
        /// </summary>
        public event MTConnectLogEventHandler LogReceived;


        /// <summary>
        /// Initializes a new instance bound to the given Agent broker, with a default data-source configuration.
        /// </summary>
        /// <param name="agent">The Agent broker that hosts this module.</param>
        public MTConnectInputAgentModule(IMTConnectAgentBroker agent)
        {
            _agent = agent;
            Configuration = new DataSourceConfiguration();
        }


        /// <summary>
        /// Run <see cref="OnStartBeforeLoad(bool)"/> and add this module's Device to the Agent before Devices are loaded.
        /// </summary>
        /// <param name="initializeDataItems">When <c>true</c>, the module should initialize DataItems with their default observations.</param>
        public void StartBeforeLoad(bool initializeDataItems)
        {
            OnStartBeforeLoad(initializeDataItems);

            _device = _agent.AddDevice(OnAddDevice());
        }

        /// <summary>
        /// Run <see cref="OnStartAfterLoad(bool)"/> and start the polling loop after Devices are loaded.
        /// </summary>
        /// <param name="initializeDataItems">When <c>true</c>, the module should initialize DataItems with their default observations.</param>
        public void StartAfterLoad(bool initializeDataItems)
        {
            OnStartAfterLoad(initializeDataItems);

            Start();
        }


        /// <summary>
        /// Override to perform work before the module's Device is added to the Agent. The default implementation does nothing.
        /// </summary>
        /// <param name="initializeDataItems">When <c>true</c>, the module should initialize DataItems with their default observations.</param>
        protected virtual void OnStartBeforeLoad(bool initializeDataItems) { }

        /// <summary>
        /// Override to perform work after the module's Device is added and before polling starts. The default implementation does nothing.
        /// </summary>
        /// <param name="initializeDataItems">When <c>true</c>, the module should initialize DataItems with their default observations.</param>
        protected virtual void OnStartAfterLoad(bool initializeDataItems) { }

        /// <summary>
        /// Invoked during shutdown; the default implementation stops the polling loop. Override to release additional resources.
        /// </summary>
        protected virtual void OnStop()
        {
            Stop();
        }

        /// <summary>
        /// Override to perform a synchronous read on each polling cycle. The default implementation does nothing.
        /// </summary>
        protected virtual void OnRead() { }

        /// <summary>
        /// Override to perform an asynchronous read on each polling cycle. The default implementation completes immediately.
        /// </summary>
        /// <returns>A task representing the asynchronous read.</returns>
        protected virtual Task OnReadAsync() { return Task.CompletedTask; }

        /// <summary>
        /// Override to supply the Device this module contributes to the Agent. The default implementation returns <c>null</c>.
        /// </summary>
        /// <returns>The Device to add, or <c>null</c> if the module contributes no Device.</returns>
        protected virtual IDevice OnAddDevice() { return null; }


        /// <summary>
        /// Start the background polling loop if it is not already running.
        /// </summary>
        public void Start()
        {
            if (!_isStarted)
            {
                _isStarted = true;

                _stop = new CancellationTokenSource();

                _ = Task.Run(Worker, _stop.Token);
            }
        }

        /// <summary>
        /// Stop the background polling loop and run the shutdown hook if the module is running.
        /// </summary>
        public void Stop()
        {
            if (_isStarted)
            {
                if (_stop != null) _stop.Cancel();
                _isStarted = false;

                OnStop();
            }
        }

        private async Task Worker()
        {
            if (Configuration != null)
            {
                try
                {
                    while (!_stop.Token.IsCancellationRequested)
                    {
                        try
                        {
                            OnRead();
                            await OnReadAsync();

                            await Task.Delay(Configuration.ReadInterval, _stop.Token);
                        }
                        catch (TaskCanceledException) { }
                        catch { }
                    }
                }
                catch { }
            }
        }


        #region "Unavailable"

        /// <summary>
        /// Publish an <c>UNAVAILABLE</c> observation for every DataItem on the module's Device, typically used when the data source goes offline.
        /// </summary>
        public void SetUnavailable()
        {
            var dataItems = _device.GetDataItems();
            if (!dataItems.IsNullOrEmpty())
            {
                var timestamp = UnixDateTime.Now;

                foreach (var dataItem in dataItems)
                {
                    SetUnavailable(dataItem, timestamp);
                }
            }
        }

        /// <summary>
        /// Publish an <c>UNAVAILABLE</c> observation for the DataItem identified by the given key, using the current time.
        /// </summary>
        /// <param name="dataItemKey">The Id or Name of the DataItem.</param>
        public void SetUnavailable(string dataItemKey)
        {
            if (!string.IsNullOrEmpty(dataItemKey))
            {
                var dataItem = Device.GetDataItemByKey(dataItemKey);
                if (dataItem != null)
                {
                    SetUnavailable(dataItem);
                }
            }
        }

        /// <summary>
        /// Publish an <c>UNAVAILABLE</c> observation for the DataItem identified by the given key at the specified time.
        /// </summary>
        /// <param name="dataItemKey">The Id or Name of the DataItem.</param>
        /// <param name="timestamp">The time the DataItem became unavailable.</param>
        public void SetUnavailable(string dataItemKey, DateTime timestamp)
        {
            if (!string.IsNullOrEmpty(dataItemKey))
            {
                var dataItem = Device.GetDataItemByKey(dataItemKey);
                if (dataItem != null)
                {
                    SetUnavailable(dataItem, timestamp);
                }
            }
        }

        /// <summary>
        /// Publish an <c>UNAVAILABLE</c> observation for the DataItem identified by the given key at the specified time.
        /// </summary>
        /// <param name="dataItemKey">The Id or Name of the DataItem.</param>
        /// <param name="timestamp">The time the DataItem became unavailable, in Unix ticks.</param>
        public void SetUnavailable(string dataItemKey, long timestamp)
        {
            if (!string.IsNullOrEmpty(dataItemKey))
            {
                var dataItem = Device.GetDataItemByKey(dataItemKey);
                if (dataItem != null)
                {
                    SetUnavailable(dataItem, timestamp);
                }
            }
        }

        /// <summary>
        /// Publish an <c>UNAVAILABLE</c> observation for the given DataItem, using the current time.
        /// </summary>
        /// <param name="dataItem">The DataItem to mark unavailable.</param>
        public void SetUnavailable(IDataItem dataItem)
        {
            var timestamp = UnixDateTime.Now;
            SetUnavailable(dataItem, timestamp);
        }

        /// <summary>
        /// Publish an <c>UNAVAILABLE</c> observation for the given DataItem at the specified time.
        /// </summary>
        /// <param name="dataItem">The DataItem to mark unavailable.</param>
        /// <param name="timestamp">The time the DataItem became unavailable.</param>
        public void SetUnavailable(IDataItem dataItem, DateTime timestamp)
        {
            SetUnavailable(dataItem, timestamp.ToUnixTime());
        }

        /// <summary>
        /// Publish an <c>UNAVAILABLE</c> observation for the given DataItem at the specified time, emitting an <c>UNAVAILABLE</c> condition level for condition DataItems and the unavailable result value otherwise.
        /// </summary>
        /// <param name="dataItem">The DataItem to mark unavailable.</param>
        /// <param name="timestamp">The time the DataItem became unavailable, in Unix ticks.</param>
        public void SetUnavailable(IDataItem dataItem, long timestamp)
        {
            if (dataItem != null && !string.IsNullOrEmpty(dataItem.Id))
            {
                var observation = new ObservationInput();
                observation.DeviceKey = _device.Uuid;
                observation.DataItemKey = dataItem.Id;
                observation.Timestamp = timestamp;

                if (dataItem.Category == DataItemCategory.CONDITION)
                {
                    observation.AddValue(ValueKeys.Level, ConditionLevel.UNAVAILABLE);
                }
                else
                {
                    observation.AddValue(ValueKeys.Result, Observation.Unavailable);
                }

                AddObservation(observation);
            }
        }

        #endregion

        #region "Value Observations"

        /// <summary>
        /// Publish a value observation for the given DataItem, using the current time.
        /// </summary>
        /// <param name="dataItem">The DataItem to report against.</param>
        /// <param name="resultValue">The observed value.</param>
        public void AddValueObservation(IDataItem dataItem, object resultValue)
        {
            if (dataItem != null && !string.IsNullOrEmpty(dataItem.Id))
            {
                var observation = new ObservationInput(dataItem.Id, resultValue);
                AddObservation(observation);
            }
        }

        /// <summary>
        /// Publish a value observation for the given DataItem at the specified time.
        /// </summary>
        /// <param name="dataItem">The DataItem to report against.</param>
        /// <param name="resultValue">The observed value.</param>
        /// <param name="timestamp">The time of the observation, in Unix ticks.</param>
        public void AddValueObservation(IDataItem dataItem, object resultValue, long timestamp)
        {
            if (dataItem != null && !string.IsNullOrEmpty(dataItem.Id))
            {
                var observation = new ObservationInput(dataItem.Id, resultValue, timestamp);
                AddObservation(observation);
            }
        }

        /// <summary>
        /// Publish a value observation for the given DataItem at the specified time.
        /// </summary>
        /// <param name="dataItem">The DataItem to report against.</param>
        /// <param name="resultValue">The observed value.</param>
        /// <param name="timestamp">The time of the observation.</param>
        public void AddValueObservation(IDataItem dataItem, object resultValue, DateTime timestamp)
        {
            if (dataItem != null && !string.IsNullOrEmpty(dataItem.Id))
            {
                var observation = new ObservationInput(dataItem.Id, resultValue, timestamp);
                AddObservation(observation);
            }
        }

        /// <summary>
        /// Publish a value observation for the DataItem identified by the given key, using the current time.
        /// </summary>
        /// <param name="dataItemKey">The Id or Name of the DataItem.</param>
        /// <param name="resultValue">The observed value.</param>
        public void AddValueObservation(string dataItemKey, object resultValue)
        {
            var observation = new ObservationInput(dataItemKey, resultValue);
            AddObservation(observation);
        }

        /// <summary>
        /// Publish a value observation for the DataItem identified by the given key at the specified time.
        /// </summary>
        /// <param name="dataItemKey">The Id or Name of the DataItem.</param>
        /// <param name="resultValue">The observed value.</param>
        /// <param name="timestamp">The time of the observation, in Unix ticks.</param>
        public void AddValueObservation(string dataItemKey, object resultValue, long timestamp)
        {
            var observation = new ObservationInput(dataItemKey, resultValue, timestamp);
            AddObservation(observation);
        }

        /// <summary>
        /// Publish a value observation for the DataItem identified by the given key at the specified time.
        /// </summary>
        /// <param name="dataItemKey">The Id or Name of the DataItem.</param>
        /// <param name="resultValue">The observed value.</param>
        /// <param name="timestamp">The time of the observation.</param>
        public void AddValueObservation(string dataItemKey, object resultValue, DateTime timestamp)
        {
            var observation = new ObservationInput(dataItemKey, resultValue, timestamp);
            AddObservation(observation);
        }

        /// <summary>
        /// Publish a value observation, resolving the DataItem on the module's Device by its type and optional sub-type.
        /// </summary>
        /// <typeparam name="TDataItem">The DataItem type to resolve.</typeparam>
        /// <param name="result">The observed value.</param>
        /// <param name="subType">An optional DataItem sub-type used to disambiguate the lookup.</param>
        public void AddValueObservation<TDataItem>(object result, object subType = null) where TDataItem : IDataItem
        {
            if (Agent != null && Device != null)
            {
                Agent.AddObservation(Device.GetDataItem<TDataItem>(subType?.ToString()), result);
            }
        }

        /// <summary>
        /// Publish a value observation, resolving the DataItem on a named Component of the module's Device.
        /// </summary>
        /// <typeparam name="TComponent">The Component type that owns the DataItem.</typeparam>
        /// <typeparam name="TDataItem">The DataItem type to resolve.</typeparam>
        /// <param name="result">The observed value.</param>
        /// <param name="componentName">An optional Component name used to disambiguate the lookup.</param>
        /// <param name="subType">An optional DataItem sub-type used to disambiguate the lookup.</param>
        public void AddValueObservation<TComponent, TDataItem>(object result, string componentName = null, object subType = null)
            where TComponent : IComponent
            where TDataItem : IDataItem
        {
            if (Agent != null && Device != null)
            {
                Agent.AddObservation(Device.GetComponent<TComponent>(componentName)?.GetDataItem<TDataItem>(subType?.ToString()), result);
            }
        }

        /// <summary>
        /// Publish a value observation, resolving the DataItem on a Composition of a named Component of the module's Device.
        /// </summary>
        /// <typeparam name="TComponent">The Component type that owns the Composition.</typeparam>
        /// <typeparam name="TComposition">The Composition type that owns the DataItem.</typeparam>
        /// <typeparam name="TDataItem">The DataItem type to resolve.</typeparam>
        /// <param name="result">The observed value.</param>
        /// <param name="componentName">An optional Component name used to disambiguate the lookup.</param>
        /// <param name="subType">An optional DataItem sub-type used to disambiguate the lookup.</param>
        public void AddValueObservation<TComponent, TComposition, TDataItem>(object result, string componentName = null, object subType = null)
            where TComponent : IComponent
            where TComposition : IComposition
            where TDataItem : IDataItem
        {
            if (Agent != null && Device != null)
            {
                Agent.AddObservation(Device.GetComponent<TComponent>(componentName)?.GetComposition<TComposition>()?.GetDataItem<TDataItem>(subType?.ToString()), result);
            }
        }

        #endregion

        #region "Condition Observations"

        /// <summary>
        /// Publish a condition fault-state observation for the DataItem identified by the given key, using the current time.
        /// </summary>
        /// <param name="dataItemKey">The Id or Name of the condition DataItem.</param>
        /// <param name="level">The condition level being reported.</param>
        /// <param name="nativeCode">An optional native code identifying the condition on the source equipment.</param>
        /// <param name="message">An optional human-readable description of the condition.</param>
        /// <param name="nativeSeverity">An optional native severity reported by the source equipment.</param>
        /// <param name="qualifier">An optional qualifier that further classifies the condition.</param>
        /// <param name="conditionId">An optional identifier used to correlate updates to the same condition.</param>
        public void AddConditionObservation(
            string dataItemKey,
            ConditionLevel level,
            string nativeCode = null,
            string message = null,
            string nativeSeverity = null,
            ConditionQualifier qualifier = ConditionQualifier.NOT_SPECIFIED,
            string conditionId = null
            )
        {
            AddConditionObservation(dataItemKey, level, UnixDateTime.Now, nativeCode, message, nativeSeverity, qualifier, conditionId);
        }

        /// <summary>
        /// Publish a condition fault-state observation for the DataItem identified by the given key at the specified time.
        /// </summary>
        /// <param name="dataItemKey">The Id or Name of the condition DataItem.</param>
        /// <param name="level">The condition level being reported.</param>
        /// <param name="timestamp">The time of the observation.</param>
        /// <param name="nativeCode">An optional native code identifying the condition on the source equipment.</param>
        /// <param name="message">An optional human-readable description of the condition.</param>
        /// <param name="nativeSeverity">An optional native severity reported by the source equipment.</param>
        /// <param name="qualifier">An optional qualifier that further classifies the condition.</param>
        /// <param name="conditionId">An optional identifier used to correlate updates to the same condition.</param>
        public void AddConditionObservation(
            string dataItemKey,
            ConditionLevel level,
            DateTime timestamp,
            string nativeCode = null,
            string message = null,
            string nativeSeverity = null,
            ConditionQualifier qualifier = ConditionQualifier.NOT_SPECIFIED,
            string conditionId = null
            )
        {
            AddConditionObservation(dataItemKey, level, timestamp.ToUnixTime(), nativeCode, message, nativeSeverity, qualifier, conditionId);
        }

        /// <summary>
        /// Publish a condition fault-state observation for the DataItem identified by the given key at the specified time.
        /// </summary>
        /// <param name="dataItemKey">The Id or Name of the condition DataItem.</param>
        /// <param name="level">The condition level being reported.</param>
        /// <param name="timestamp">The time of the observation, in Unix ticks.</param>
        /// <param name="nativeCode">An optional native code identifying the condition on the source equipment.</param>
        /// <param name="message">An optional human-readable description of the condition.</param>
        /// <param name="nativeSeverity">An optional native severity reported by the source equipment.</param>
        /// <param name="qualifier">An optional qualifier that further classifies the condition.</param>
        /// <param name="conditionId">An optional identifier used to correlate updates to the same condition.</param>
        public void AddConditionObservation(
            string dataItemKey,
            ConditionLevel level,
            long timestamp,
            string nativeCode = null,
            string message = null,
            string nativeSeverity = null,
            ConditionQualifier qualifier = ConditionQualifier.NOT_SPECIFIED,
            string conditionId = null
            )
        {
            if (Agent != null && Device != null && !string.IsNullOrEmpty(dataItemKey))
            {
                var observationInput = new ConditionFaultStateObservationInput();
                observationInput.DeviceKey = Device.Uuid;
                observationInput.DataItemKey = dataItemKey;
                observationInput.Level = level;
                observationInput.ConditionId = conditionId;
                observationInput.NativeCode = nativeCode;
                observationInput.Message = message;
                observationInput.NativeSeverity = nativeSeverity;
                observationInput.Qualifier = qualifier;
                observationInput.Timestamp = timestamp;

                Agent.AddObservation(observationInput);
            }
        }

        /// <summary>
        /// Publish a condition fault-state observation, resolving the DataItem on the module's Device by its type and optional sub-type.
        /// </summary>
        /// <typeparam name="TDataItem">The condition DataItem type to resolve.</typeparam>
        /// <param name="level">The condition level being reported.</param>
        /// <param name="nativeCode">An optional native code identifying the condition on the source equipment.</param>
        /// <param name="message">An optional human-readable description of the condition.</param>
        /// <param name="nativeSeverity">An optional native severity reported by the source equipment.</param>
        /// <param name="qualifier">An optional qualifier that further classifies the condition.</param>
        /// <param name="conditionId">An optional identifier used to correlate updates to the same condition.</param>
        /// <param name="subType">An optional DataItem sub-type used to disambiguate the lookup.</param>
        public void AddConditionObservation<TDataItem>(
            ConditionLevel level,
            string nativeCode = null,
            string message = null,
            string nativeSeverity = null,
            ConditionQualifier qualifier = ConditionQualifier.NOT_SPECIFIED,
            string conditionId = null,
            string subType = null
            )
            where TDataItem : IDataItem
        {
            if (Agent != null && Device != null)
            {
                var dataItem = Device.GetDataItem<TDataItem>(subType);
                if (dataItem != null)
                {
                    var observationInput = new ConditionFaultStateObservationInput();
                    observationInput.DeviceKey = Device.Uuid;
                    observationInput.DataItemKey = dataItem.Id;
                    observationInput.Level = level;
                    observationInput.ConditionId = conditionId;
                    observationInput.NativeCode = nativeCode;
                    observationInput.Message = message;
                    observationInput.NativeSeverity = nativeSeverity;
                    observationInput.Qualifier = qualifier;

                    Agent.AddObservation(observationInput);
                }
            }
        }

        /// <summary>
        /// Publish a condition fault-state observation, resolving the DataItem on a named Component of the module's Device.
        /// </summary>
        /// <typeparam name="TComponent">The Component type that owns the DataItem.</typeparam>
        /// <typeparam name="TDataItem">The condition DataItem type to resolve.</typeparam>
        /// <param name="level">The condition level being reported.</param>
        /// <param name="nativeCode">An optional native code identifying the condition on the source equipment.</param>
        /// <param name="message">An optional human-readable description of the condition.</param>
        /// <param name="nativeSeverity">An optional native severity reported by the source equipment.</param>
        /// <param name="qualifier">An optional qualifier that further classifies the condition.</param>
        /// <param name="conditionId">An optional identifier used to correlate updates to the same condition.</param>
        /// <param name="componentName">An optional Component name used to disambiguate the lookup.</param>
        /// <param name="subType">An optional DataItem sub-type used to disambiguate the lookup.</param>
        public void AddConditionObservation<TComponent, TDataItem>(
            ConditionLevel level,
            string nativeCode = null,
            string message = null,
            string nativeSeverity = null,
            ConditionQualifier qualifier = ConditionQualifier.NOT_SPECIFIED,
            string conditionId = null,
            string componentName = null,
            string subType = null
            )
            where TComponent : IComponent
            where TDataItem : IDataItem
        {
            if (Agent != null && Device != null)
            {
                var dataItem = Device.GetComponent<TComponent>(componentName)?.GetDataItem<TDataItem>(subType);
                if (dataItem != null)
                {
                    var observationInput = new ConditionFaultStateObservationInput();
                    observationInput.DeviceKey = Device.Uuid;
                    observationInput.DataItemKey = dataItem.Id;
                    observationInput.Level = level;
                    observationInput.ConditionId = conditionId;
                    observationInput.NativeCode = nativeCode;
                    observationInput.Message = message;
                    observationInput.NativeSeverity = nativeSeverity;
                    observationInput.Qualifier = qualifier;

                    Agent.AddObservation(observationInput);
                }
            }
        }

        #endregion

        #region "DataSet Observations"

        /// <summary>
        /// Publish a Data Set observation containing a single entry for the DataItem identified by the given key.
        /// </summary>
        /// <param name="dataItemKey">The Id or Name of the Data Set DataItem.</param>
        /// <param name="entryKey">The key of the Data Set entry.</param>
        /// <param name="value">The value of the Data Set entry.</param>
        public void AddDataSetObservation(string dataItemKey, string entryKey, object value)
        {
            if (Agent != null && Device != null && !string.IsNullOrEmpty(dataItemKey) && !string.IsNullOrEmpty(entryKey) && value != null)
            {
                var observationInput = new DataSetObservationInput();
                observationInput.DeviceKey = Device.Uuid;
                observationInput.DataItemKey = dataItemKey;
                observationInput.Entries = new IDataSetEntry[] { new DataSetEntry(entryKey, value) };

                Agent.AddObservation(observationInput);
            }
        }

        /// <summary>
        /// Publish a Data Set observation containing a single entry for the DataItem identified by the given key.
        /// </summary>
        /// <param name="dataItemKey">The Id or Name of the Data Set DataItem.</param>
        /// <param name="entry">The Data Set entry to publish.</param>
        public void AddDataSetObservation(string dataItemKey, IDataSetEntry entry)
        {
            if (Agent != null && Device != null && !string.IsNullOrEmpty(dataItemKey) && entry != null)
            {
                var observationInput = new DataSetObservationInput();
                observationInput.DeviceKey = Device.Uuid;
                observationInput.DataItemKey = dataItemKey;
                observationInput.Entries = new IDataSetEntry[] { entry };

                Agent.AddObservation(observationInput);
            }
        }

        /// <summary>
        /// Publish a Data Set observation containing the given entries for the DataItem identified by the given key.
        /// </summary>
        /// <param name="dataItemKey">The Id or Name of the Data Set DataItem.</param>
        /// <param name="entries">The Data Set entries to publish.</param>
        public void AddDataSetObservation(string dataItemKey, IEnumerable<IDataSetEntry> entries)
        {
            if (Agent != null && Device != null && !string.IsNullOrEmpty(dataItemKey) && !entries.IsNullOrEmpty())
            {
                var observationInput = new DataSetObservationInput();
                observationInput.DeviceKey = Device.Uuid;
                observationInput.DataItemKey = dataItemKey;
                observationInput.Entries = entries;

                Agent.AddObservation(observationInput);
            }
        }

        /// <summary>
        /// Publish a Data Set observation, resolving the DataItem on the module's Device by its type and optional sub-type.
        /// </summary>
        /// <typeparam name="TDataItem">The Data Set DataItem type to resolve.</typeparam>
        /// <param name="entries">The Data Set entries to publish.</param>
        /// <param name="subType">An optional DataItem sub-type used to disambiguate the lookup.</param>
        public void AddDataSetObservation<TDataItem>(
            IEnumerable<IDataSetEntry> entries,
            string subType = null
            )
            where TDataItem : IDataItem
        {
            if (Agent != null && Device != null && !entries.IsNullOrEmpty())
            {
                var dataItem = Device.GetDataItem<TDataItem>(subType);
                if (dataItem != null)
                {
                    var observationInput = new DataSetObservationInput();
                    observationInput.DeviceKey = Device.Uuid;
                    observationInput.DataItemKey = dataItem.Id;
                    observationInput.Entries = entries;

                    Agent.AddObservation(observationInput);
                }
            }
        }

        /// <summary>
        /// Publish a Data Set observation, resolving the DataItem on a named Component of the module's Device.
        /// </summary>
        /// <typeparam name="TComponent">The Component type that owns the DataItem.</typeparam>
        /// <typeparam name="TDataItem">The Data Set DataItem type to resolve.</typeparam>
        /// <param name="entries">The Data Set entries to publish.</param>
        /// <param name="componentName">An optional Component name used to disambiguate the lookup.</param>
        /// <param name="subType">An optional DataItem sub-type used to disambiguate the lookup.</param>
        public void AddDataSetObservation<TComponent, TDataItem>(
            IEnumerable<IDataSetEntry> entries,
            string componentName = null,
            string subType = null
            )
            where TComponent : IComponent
            where TDataItem : IDataItem
        {
            if (Agent != null && Device != null && !entries.IsNullOrEmpty())
            {
                var dataItem = Device.GetComponent<TComponent>(componentName)?.GetDataItem<TDataItem>(subType);
                if (dataItem != null)
                {
                    var observationInput = new DataSetObservationInput();
                    observationInput.DeviceKey = Device.Uuid;
                    observationInput.DataItemKey = dataItem.Id;
                    observationInput.Entries = entries;

                    Agent.AddObservation(observationInput);
                }
            }
        }

        #endregion

        #region "Table Observations"

        /// <summary>
        /// Publish a Table observation containing a single entry for the DataItem identified by the given key.
        /// </summary>
        /// <param name="dataItemKey">The Id or Name of the Table DataItem.</param>
        /// <param name="entry">The Table entry to publish.</param>
        public void AddTableObservation(string dataItemKey, ITableEntry entry)
        {
            if (Agent != null && Device != null && !string.IsNullOrEmpty(dataItemKey) && entry != null)
            {
                var observationInput = new TableObservationInput();
                observationInput.DeviceKey = Device.Uuid;
                observationInput.DataItemKey = dataItemKey;
                observationInput.Entries = new ITableEntry[] { entry };

                Agent.AddObservation(observationInput);
            }
        }

        /// <summary>
        /// Publish a Table observation containing the given entries for the DataItem identified by the given key.
        /// </summary>
        /// <param name="dataItemKey">The Id or Name of the Table DataItem.</param>
        /// <param name="entries">The Table entries to publish.</param>
        public void AddTableObservation(string dataItemKey, IEnumerable<ITableEntry> entries)
        {
            if (Agent != null && Device != null && !string.IsNullOrEmpty(dataItemKey) && !entries.IsNullOrEmpty())
            {
                var observationInput = new TableObservationInput();
                observationInput.DeviceKey = Device.Uuid;
                observationInput.DataItemKey = dataItemKey;
                observationInput.Entries = entries;

                Agent.AddObservation(observationInput);
            }
        }

        /// <summary>
        /// Publish a Table observation, resolving the DataItem on the module's Device by its type and optional sub-type.
        /// </summary>
        /// <typeparam name="TDataItem">The Table DataItem type to resolve.</typeparam>
        /// <param name="entries">The Table entries to publish.</param>
        /// <param name="subType">An optional DataItem sub-type used to disambiguate the lookup.</param>
        public void AddTableObservation<TDataItem>(
            IEnumerable<ITableEntry> entries,
            string subType = null
            )
            where TDataItem : IDataItem
        {
            if (Agent != null && Device != null && !entries.IsNullOrEmpty())
            {
                var dataItem = Device.GetDataItem<TDataItem>(subType);
                if (dataItem != null)
                {
                    var observationInput = new TableObservationInput();
                    observationInput.DeviceKey = Device.Uuid;
                    observationInput.DataItemKey = dataItem.Id;
                    observationInput.Entries = entries;

                    Agent.AddObservation(observationInput);
                }
            }
        }

        /// <summary>
        /// Publish a Table observation, resolving the DataItem on a named Component of the module's Device.
        /// </summary>
        /// <typeparam name="TComponent">The Component type that owns the DataItem.</typeparam>
        /// <typeparam name="TDataItem">The Table DataItem type to resolve.</typeparam>
        /// <param name="entries">The Table entries to publish.</param>
        /// <param name="componentName">An optional Component name used to disambiguate the lookup.</param>
        /// <param name="subType">An optional DataItem sub-type used to disambiguate the lookup.</param>
        public void AddTableObservation<TComponent, TDataItem>(
            IEnumerable<ITableEntry> entries,
            string componentName = null,
            string subType = null
            )
            where TComponent : IComponent
            where TDataItem : IDataItem
        {
            if (Agent != null && Device != null && !entries.IsNullOrEmpty())
            {
                var dataItem = Device.GetComponent<TComponent>(componentName)?.GetDataItem<TDataItem>(subType);
                if (dataItem != null)
                {
                    var observationInput = new TableObservationInput();
                    observationInput.DeviceKey = Device.Uuid;
                    observationInput.DataItemKey = dataItem.Id;
                    observationInput.Entries = entries;

                    Agent.AddObservation(observationInput);
                }
            }
        }

        #endregion

        #region "TimeSeries Observations"

        /// <summary>
        /// Publish a Time Series observation containing the given samples for the DataItem identified by the given key.
        /// </summary>
        /// <param name="dataItemKey">The Id or Name of the Time Series DataItem.</param>
        /// <param name="samples">The ordered sample values.</param>
        public void AddTimeSeriesObservation(string dataItemKey, IEnumerable<double> samples)
        {
            if (Agent != null && Device != null && !string.IsNullOrEmpty(dataItemKey) && !samples.IsNullOrEmpty())
            {
                var observationInput = new TimeSeriesObservationInput();
                observationInput.DeviceKey = Device.Uuid;
                observationInput.DataItemKey = dataItemKey;
                observationInput.Samples = samples;

                Agent.AddObservation(observationInput);
            }
        }

        /// <summary>
        /// Publish a Time Series observation, resolving the DataItem on the module's Device by its type and optional sub-type.
        /// </summary>
        /// <typeparam name="TDataItem">The Time Series DataItem type to resolve.</typeparam>
        /// <param name="samples">The ordered sample values.</param>
        /// <param name="subType">An optional DataItem sub-type used to disambiguate the lookup.</param>
        public void AddTimeSeriesObservation<TDataItem>(
            IEnumerable<double> samples,
            string subType = null
            )
            where TDataItem : IDataItem
        {
            if (Agent != null && Device != null && !samples.IsNullOrEmpty())
            {
                var dataItem = Device.GetDataItem<TDataItem>(subType);
                if (dataItem != null)
                {
                    var observationInput = new TimeSeriesObservationInput();
                    observationInput.DeviceKey = Device.Uuid;
                    observationInput.DataItemKey = dataItem.Id;
                    observationInput.Samples = samples;

                    Agent.AddObservation(observationInput);
                }
            }
        }

        /// <summary>
        /// Publish a Time Series observation, resolving the DataItem on a named Component of the module's Device.
        /// </summary>
        /// <typeparam name="TComponent">The Component type that owns the DataItem.</typeparam>
        /// <typeparam name="TDataItem">The Time Series DataItem type to resolve.</typeparam>
        /// <param name="samples">The ordered sample values.</param>
        /// <param name="componentName">An optional Component name used to disambiguate the lookup.</param>
        /// <param name="subType">An optional DataItem sub-type used to disambiguate the lookup.</param>
        public void AddTimeSeriesObservation<TComponent, TDataItem>(
            IEnumerable<double> samples,
            string componentName = null,
            string subType = null
            )
            where TComponent : IComponent
            where TDataItem : IDataItem
        {
            if (Agent != null && Device != null && !samples.IsNullOrEmpty())
            {
                var dataItem = Device.GetComponent<TComponent>(componentName)?.GetDataItem<TDataItem>(subType);
                if (dataItem != null)
                {
                    var observationInput = new TimeSeriesObservationInput();
                    observationInput.DeviceKey = Device.Uuid;
                    observationInput.DataItemKey = dataItem.Id;
                    observationInput.Samples = samples;

                    Agent.AddObservation(observationInput);
                }
            }
        }

        #endregion


        /// <summary>
        /// Publish a prepared observation input to the Agent against the module's Device.
        /// </summary>
        /// <param name="observation">The observation input to publish.</param>
        public void AddObservation(IObservationInput observation)
        {
            Agent.AddObservation(_device.Uuid, observation);
        }


        /// <summary>
        /// Publish an Asset to the Agent, wrapping it in an Asset input bound to the module's Device.
        /// </summary>
        /// <param name="asset">The Asset to publish.</param>
        public void AddAsset(IAsset asset)
        {
            if (asset != null)
            {
                AddAsset(new AssetInput(asset));
            }
        }

        /// <summary>
        /// Publish a prepared Asset input to the Agent.
        /// </summary>
        /// <param name="asset">The Asset input, including the target Device key.</param>
        public void AddAsset(IAssetInput asset)
        {
            Agent.AddAsset(asset.DeviceKey, asset.Asset);
        }


        /// <summary>
        /// Raise <see cref="LogReceived"/> with the given log entry.
        /// </summary>
        /// <param name="logLevel">The severity of the log entry.</param>
        /// <param name="message">The log message.</param>
        protected void Log(MTConnectLogLevel logLevel, string message)
        {
            if (LogReceived != null) LogReceived.Invoke(this, logLevel, message);
        }
    }
}
