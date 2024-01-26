// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets;
using MTConnect.Configurations;
using MTConnect.Devices;
using MTConnect.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Input
{
    /// <summary>
    /// Engine used to handle a worker thread running at an Interval used to organize reading from a Data Source (ex. PLC) and writing to an Adapter
    /// </summary>
    public abstract class MTConnectDataSource : IMTConnectDataSource
    {
        private CancellationTokenSource _stop;
        private bool _isStarted;


        public IDataSourceConfiguration Configuration { get; set; }


        public event EventHandler<IObservationInput> ObservationAdded;

        public event EventHandler<IAssetInput> AssetAdded;

        public event EventHandler<IDeviceInput> DeviceAdded;

        public event MTConnectLogEventHandler LogReceived;


        protected virtual void OnStart() { }

        protected virtual void OnStop() { }

        protected virtual void OnRead() { }

        protected virtual Task OnReadAsync() { return Task.CompletedTask; }


        public void Start()
        {
            if (!_isStarted)
            {
                _isStarted = true;

                _stop = new CancellationTokenSource();

                OnStart();

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


        public void AddObservation(string dataItemKey, object resultValue)
        {
            var observation = new ObservationInput(dataItemKey, resultValue);
            AddObservation(observation);
        }

        public void AddObservation(string dataItemKey, object resultValue, long? timestamp = null)
        {
            var observation = new ObservationInput(dataItemKey, resultValue, timestamp.Value);
            AddObservation(observation);
        }

        public void AddObservation(string dataItemKey, object resultValue, DateTime? timestamp = null)
        {
            var observation = new ObservationInput(dataItemKey, resultValue, timestamp.Value);
            AddObservation(observation);
        }

        public void AddObservation(IObservationInput observation)
        {
            if (ObservationAdded != null) ObservationAdded.Invoke(this, observation);
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
            if (AssetAdded != null) AssetAdded.Invoke(this, asset);
        }


        public void AddDevice(IDevice device)
        {
            if (device != null)
            {
                AddDevice(new DeviceInput(device));
            }
        }

        public void AddDevice(IDeviceInput device)
        {
            if (DeviceAdded != null) DeviceAdded.Invoke(this, device);
        }


        protected void Log(MTConnectLogLevel logLevel, string message)
        {
            if (LogReceived != null) LogReceived.Invoke(this, logLevel, message);
        }
    }
}
