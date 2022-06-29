// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Configurations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Http
{
    public abstract class HttpServer : IDisposable
    {
        private const string DefaultServer = "127.0.0.1";
        private const int DefaultPort = 5000;
        private const string EmptyServer = "0.0.0.0";
        private const int DefaultMaxThreads = 5;

        private static readonly string DefaultPrefix = "http://" + DefaultServer + ":" + DefaultPort + "/";

        private readonly Thread _listenerThread;
        private readonly Thread[] _workers;
        private readonly ManualResetEvent _stop, _ready;
        private readonly Queue<HttpListenerContext> _queue;
        private readonly IEnumerable<string> _prefixes;
        private HttpListener _listener;


        /// <summary>
        /// Event Handler for when the HttpListener is started
        /// </summary>
        /// <returns>URL Prefix that the HttpListener is listening for requests on</returns>
        public EventHandler<string> ListenerStarted { get; set; }

        /// <summary>
        /// Event Handler for when the HttpListener is stopped
        /// </summary>
        /// <returns>URL Prefix that the HttpListener is listening for requests on</returns>
        public EventHandler<string> ListenerStopped { get; set; }

        /// <summary>
        /// Event Handler for when an error occurs with the HttpListener
        /// </summary>
        public EventHandler<Exception> ListenerException { get; set; }


        public HttpServer(HttpAgentConfiguration configuration, IEnumerable<string> prefixes = null, int port = 0)
        {
            var maxThreads = DefaultMaxThreads;
            if (configuration != null)
            {
                maxThreads = Math.Max(configuration.MaxListenerThreads, 1);                
            }

            _prefixes = CreatePrefixes(configuration, prefixes, port);
            _workers = new Thread[maxThreads];
            _queue = new Queue<HttpListenerContext>();
            _stop = new ManualResetEvent(false);
            _ready = new ManualResetEvent(false);
            //_listener = new HttpListener();
            _listenerThread = new Thread(HandleRequests);
        }


        public void Start()
        {
            //// Set the Prefixes
            //if (!_prefixes.IsNullOrEmpty())
            //{
            //    foreach (var prefix in _prefixes)
            //    {
            //        _listener.Prefixes.Add(prefix);
            //    }
            //}
            
            //// Start the HttpListener
            //_listener.Start();

            // Start the listening thread
            _listenerThread.Start();

            // Start all of the worker threads
            for (int i = 0; i < _workers.Length; i++)
            {
                _workers[i] = new Thread(Worker);
                _workers[i].Start();
            }
        }

        public void Stop()
        {
            _stop.Set();
            _listenerThread.Join();
            foreach (Thread worker in _workers) worker.Join();

            if (_listener != null)
            {
                try
                {
                    _listener.Stop();
                }
                catch { }
            }
        }

        public void Dispose() { Stop(); }


        private void HandleRequests()
        {
            if (!_prefixes.IsNullOrEmpty())
            {
                do
                {
                    var errorOccurred = false;

                    try
                    {
                        // Intinialize as new HttpListener
                        _listener = new HttpListener();

                        // Add Prefixes
                        foreach (var prefix in _prefixes) _listener.Prefixes.Add(prefix);

                        // Start the HttpListener
                        _listener.Start();

                        foreach (var prefix in _prefixes) ListenerStarted?.Invoke(this, prefix);

                        while (_listener.IsListening)
                        {
                            var context = _listener.BeginGetContext(ContextReady, null);

                            if (WaitHandle.WaitAny(new[] { _stop, context.AsyncWaitHandle }) == 0) return;
                        }

                        foreach (var prefix in _prefixes) ListenerStopped?.Invoke(this, prefix);
                    }
                    catch (Exception ex)
                    {
                        errorOccurred = true;
                        if (ListenerException != null) ListenerException.Invoke(this, ex);
                    }
                    finally
                    {
                        _listener.Abort();

                        // Raise Events to notify when a prefix is stopped being listened on
                        if (ListenerStopped != null)
                        {
                            foreach (var prefix in _prefixes) ListenerStopped.Invoke(this, prefix);
                        }
                    }

                    // Delay 1 second when listener errors (to prevent a reoccurring error from overloading)
                    if (errorOccurred) Thread.Sleep(1000);

                } while (!_stop.WaitOne(100, false));
            }
        }

        private void ContextReady(IAsyncResult ar)
        {
            try
            {
                lock (_queue)
                {
                    _queue.Enqueue(_listener.EndGetContext(ar));
                    _ready.Set();
                }
            }
            catch { return; }
        }

        private void Worker()
        {
            WaitHandle[] wait = new[] { _ready, _stop };
            while (WaitHandle.WaitAny(wait) == 0)
            {
                HttpListenerContext context;
                lock (_queue)
                {
                    if (_queue.Count > 0)
                        context = _queue.Dequeue();
                    else
                    {
                        _ready.Reset();
                        continue;
                    }
                }

                try 
                {
                    OnRequestReceived(context); 
                }
                catch (Exception e) 
                { 
                    Console.Error.WriteLine(e);
                }
            }
        }

        protected virtual async Task OnRequestReceived(HttpListenerContext context) 
        {
            await Task.Delay(1); // Placeholder as this method is meant to be overridden
        }

        private static IEnumerable<string> CreatePrefixes(HttpAgentConfiguration configuration, IEnumerable<string> prefixes = null, int port = 0)
        {
            var x = new List<string>();

            if (!prefixes.IsNullOrEmpty())
            {
                x.AddRange(prefixes);
            }
            else if (configuration != null)
            {
                var serverIp = DefaultServer;
                var serverPort = DefaultPort;

                // Configuration Server IP
                if (!string.IsNullOrEmpty(configuration.ServerIp))
                {
                    if (configuration.ServerIp != EmptyServer)
                    {
                        serverIp = configuration.ServerIp;
                    }
                }

                // Set Port (if not overridden in method, read from Configuration)
                if (port > 0)
                {
                    serverPort = port;
                }
                else if (configuration.Port > 0)
                {
                    serverPort = configuration.Port;
                }

                // Construct Prefix URL
                x.Add("http://" + serverIp + ":" + serverPort + "/");

            }
            else x.Add(DefaultPrefix);

            return x;
        }

    }
}
