// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Configurations;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Adapters.Shdr
{
    /// <summary>
    /// Engine used to handle a worker thread running at an Interval used to organize reading from a Data Source (ex. PLC) and writing to a SHDR Adapter
    /// </summary>
    /// <typeparam name="TConfiguration">The type of configuration file to use</typeparam>
    public abstract class MTConnectShdrAdapterEngine<TConfiguration> where TConfiguration : ShdrAdapterApplicationConfiguration
    {
        private const int _defaultReadInterval = 1000;

        private CancellationTokenSource _stop;
        private bool _isStarted;


        public ShdrAdapter Adapter { get; set; }

        public TConfiguration Configuration { get; set; }

        public int ReadInterval { get; set; }


        public MTConnectShdrAdapterEngine()
        {
            ReadInterval = _defaultReadInterval;
        }


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
            try
            {
                while (!_stop.Token.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(ReadInterval, _stop.Token);

                        OnRead();
                        await OnReadAsync();
                    }
                    catch (TaskCanceledException) { }
                    catch { }
                }
            }
            catch { }
        }
    }
}
