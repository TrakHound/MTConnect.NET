// Copyright (c) 2020 TrakHound Inc, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace MTConnect.DeviceFinder
{
    class PingQueue
    {
        private object _lock = new object();

        private List<IPAddress> queue = new List<IPAddress>();
        private List<IPAddress> active = new List<IPAddress>();
        private List<IPAddress> successful = new List<IPAddress>();
        private List<Ping> activeRequests = new List<Ping>();

        private ManualResetEvent stop;

        public delegate void PingSentHandler(IPAddress address);
        public delegate void PingReceivedHandler(IPAddress address, PingReply reply);
        public delegate void CompletedHandler(List<IPAddress> successfulAddresses);

        public event PingSentHandler PingSent;
        public event PingReceivedHandler PingReceived;
        public event CompletedHandler Completed;


        /// <summary>
        /// Gets or Sets the Timeout used for the Ping requests
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Gets or Sets the maximum number of Ping Requests to send at a time
        /// </summary>
        public int MaxSimultaneousRequests { get; set; }

        /// <summary>
        /// Gets the Count of the underlying queue
        /// </summary>
        public int Count
        {
            get
            {
                lock (_lock)
                {
                    if (queue != null) return queue.Count;
                }

                return -1;
            }
        }

        /// <summary>
        /// Gets the Count of the underlying active queue
        /// </summary>
        private int ActiveCount
        {
            get
            {
                lock (_lock)
                {
                    if (active != null) return active.Count;
                }

                return -1;
            }
        }


        public PingQueue()
        {
            Timeout = 200;
            MaxSimultaneousRequests = 50;
        }

        public void Start()
        {
            stop = new ManualResetEvent(false);

            ThreadPool.QueueUserWorkItem(new WaitCallback(Worker));
        }

        public void Stop()
        {
            lock (_lock)
            {
                if (stop != null) stop.Set();

                foreach (var ping in activeRequests) ping.SendAsyncCancel();
            }
        }

        public void Add(IPAddress address)
        {
            if (address != null)
            {
                lock (_lock) queue.Add(address);
            }
        }

        public void Add(IEnumerable<IPAddress> addresses)
        {
            if (!addresses.IsNullOrEmpty())
            {
                foreach (var address in addresses)
                {
                    lock (_lock) queue.Add(address);
                }
            }
        }

        private void Worker(object obj)
        {
            do
            {
                List<IPAddress> addresses = null;
                int activeCount = -1;

                lock (_lock)
                {
                    activeCount = active.Count;
                    addresses = queue.Take(MaxSimultaneousRequests - activeCount).ToList();
                    active.AddRange(addresses);
                }

                if (addresses != null && addresses.Count > 0)
                {
                    foreach (var address in addresses)
                    {
                        PingSent?.Invoke(address);

                        var ping = new Ping();
                        lock (_lock) activeRequests.Add(ping);
                        ping.PingCompleted += Ping_PingCompleted;
                        ping.SendAsync(address, Timeout, address);

                        // Remove from queue
                        lock (_lock) queue.RemoveAll(o => o == address);
                    }
                }

            } while (!stop.WaitOne(10, true) && Count > 0);
        }

        private void Ping_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            if (!stop.WaitOne(0, true))
            {
                var ping = (Ping)sender;

                if (e != null && e.UserState != null)
                {
                    var address = (IPAddress)e.UserState;

                    PingReceived?.Invoke(address, e.Reply);

                    if (e.Reply.Status == IPStatus.Success) successful.Add(address);

                    lock (_lock) active.RemoveAll(o => o == address);

                    CheckCompleted();
                }

                lock (_lock) activeRequests.RemoveAll(o => o == ping);
            }
        }

        private void CheckCompleted()
        {
            bool completed = false;
            var successfulAddresses = new List<IPAddress>();

            lock (_lock)
            {
                completed = ActiveCount == 0 && Count == 0;
                successfulAddresses = successful;
            }

            if (completed) Completed?.Invoke(successfulAddresses);
        }
    }
}
