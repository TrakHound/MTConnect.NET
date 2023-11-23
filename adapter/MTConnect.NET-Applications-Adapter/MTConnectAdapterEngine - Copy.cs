// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Configurations;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Adapters.Shdr
{
    /// <summary>
    /// Engine used to handle a worker thread running at an Interval used to organize reading from a Data Source (ex. PLC) and writing to an Adapter
    /// </summary>
    /// <typeparam name="TConfiguration">The type of configuration file to use</typeparam>
    public abstract class MTConnectAdapterEngine<TConfiguration> where TConfiguration : AdapterApplicationConfiguration
    {
        private CancellationTokenSource _stop;
        private bool _isStarted;


        public IMTConnectAdapter Adapter { get; set; }

        public TConfiguration Configuration { get; set; }


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


                            await Task.Delay(Configuration.ReadInterval, _stop.Token);

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
}
