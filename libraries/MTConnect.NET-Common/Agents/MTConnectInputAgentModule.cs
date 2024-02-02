// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Input;
using MTConnect.Logging;
using MTConnect.Observations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Agents
{
    public abstract class MTConnectInputAgentModule : IMTConnectAgentModule
    {
        private readonly IMTConnectAgentBroker _agent;
        private IDevice _device;
        private CancellationTokenSource _stop;
        private bool _isStarted;


        public string Id { get; set; }

        public string Description { get; set; }

        public IDataSourceConfiguration Configuration { get; set; }

        public IMTConnectAgentBroker Agent => _agent;

        public IDevice Device => _device;


        public event MTConnectLogEventHandler LogReceived;


        public MTConnectInputAgentModule(IMTConnectAgentBroker agent)
        {
            _agent = agent;
            Configuration = new DataSourceConfiguration();
        }


        public void StartBeforeLoad(bool initializeDataItems)
        {
            OnStartBeforeLoad(initializeDataItems);

            _device = _agent.AddDevice(OnAddDevice());
        }

        public void StartAfterLoad(bool initializeDataItems)
        {
            OnStartAfterLoad(initializeDataItems);

            Start();
        }


        protected virtual void OnStartBeforeLoad(bool initializeDataItems) { }

        protected virtual void OnStartAfterLoad(bool initializeDataItems) { }

        protected virtual void OnStop() 
        {
            Stop();
        }

        protected virtual void OnRead() { }

        protected virtual Task OnReadAsync() { return Task.CompletedTask; }

        protected virtual IDevice OnAddDevice() { return null; }


        public void Start()
        {
            if (!_isStarted)
            {
                _isStarted = true;

                _stop = new CancellationTokenSource();

                _ = Task.Run(Worker, _stop.Token);
            }
        }

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

        public void SetUnavailable(IDataItem dataItem)
        {
            var timestamp = UnixDateTime.Now;
            SetUnavailable(dataItem, timestamp);
        }

        public void SetUnavailable(IDataItem dataItem, DateTime timestamp)
        {
            SetUnavailable(dataItem, timestamp.ToUnixTime());
        }

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

        public void AddValueObservation(IDataItem dataItem, object resultValue)
        {
            if (dataItem != null && !string.IsNullOrEmpty(dataItem.Id))
            {
                var observation = new ObservationInput(dataItem.Id, resultValue);
                AddObservation(observation);
            }
        }

        public void AddValueObservation(IDataItem dataItem, object resultValue, long? timestamp = null)
        {
            if (dataItem != null && !string.IsNullOrEmpty(dataItem.Id))
            {
                var observation = new ObservationInput(dataItem.Id, resultValue, timestamp.Value);
                AddObservation(observation);
            }
        }

        public void AddValueObservation(IDataItem dataItem, object resultValue, DateTime? timestamp = null)
        {
            if (dataItem != null && !string.IsNullOrEmpty(dataItem.Id))
            {
                var observation = new ObservationInput(dataItem.Id, resultValue, timestamp.Value);
                AddObservation(observation);
            }
        }

        public void AddValueObservation(string dataItemKey, object resultValue)
        {
            var observation = new ObservationInput(dataItemKey, resultValue);
            AddObservation(observation);
        }

        public void AddValueObservation(string dataItemKey, object resultValue, long? timestamp = null)
        {
            var observation = new ObservationInput(dataItemKey, resultValue, timestamp.Value);
            AddObservation(observation);
        }

        public void AddValueObservation(string dataItemKey, object resultValue, DateTime? timestamp = null)
        {
            var observation = new ObservationInput(dataItemKey, resultValue, timestamp.Value);
            AddObservation(observation);
        }

        public void AddValueObservation<TDataItem>(object result, string subType = null) where TDataItem : IDataItem
        {
            if (Agent != null && Device != null)
            {
                Agent.AddObservation(Device.GetDataItem<TDataItem>(subType), result);
            }
        }

        public void AddValueObservation<TComponent, TDataItem>(object result, string componentName = null, string subType = null)
            where TComponent : IComponent
            where TDataItem : IDataItem
        {
            if (Agent != null && Device != null)
            {
                Agent.AddObservation(Device.GetComponent<TComponent>(componentName)?.GetDataItem<TDataItem>(subType), result);
            }
        }

        public void AddValueObservation<TComponent, TComposition, TDataItem>(object result, string componentName = null, string subType = null)
            where TComponent : IComponent
            where TComposition : IComposition
            where TDataItem : IDataItem
        {
            if (Agent != null && Device != null)
            {
                Agent.AddObservation(Device.GetComponent<TComponent>(componentName)?.GetComposition<TComposition>()?.GetDataItem<TDataItem>(subType), result);
            }
        }

        #endregion

        #region "Condition Observations"

        public void AddConditionObservation<TDataItem>(
            ConditionLevel level,
            string nativeCode = null,
            string message = null,
            string nativeSeverity = null,
            ConditionQualifier qualifier = ConditionQualifier.NOT_SPECIFIED,
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
                    observationInput.NativeCode = nativeCode;
                    observationInput.Message = message;
                    observationInput.NativeSeverity = nativeSeverity;
                    observationInput.Qualifier = qualifier;

                    Agent.AddObservation(observationInput);
                }
            }
        }

        public void AddConditionObservation<TComponent, TDataItem>(
            ConditionLevel level,
            string nativeCode = null,
            string message = null,
            string nativeSeverity = null,
            ConditionQualifier qualifier = ConditionQualifier.NOT_SPECIFIED,
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


        public void AddObservation(IObservationInput observation)
        {
            Agent.AddObservation(observation);
        }

        
        public void AddAsset(IAsset asset)
        {
            if (asset != null)
            {
                AddAsset(new AssetInput(asset));
            }
        }

        public void AddAsset(IAssetInput asset)
        {
            Agent.AddAsset(asset.DeviceKey, asset.Asset);
        }


        protected void Log(MTConnectLogLevel logLevel, string message)
        {
            if (LogReceived != null) LogReceived.Invoke(this, logLevel, message);
        }
    }
}
