// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Configurations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Servers.Http
{
    public abstract class HttpServer : IDisposable
    {
        private const string Loopback = "127.0.0.1";
        private const string Localhost = "localhost";
        private const string EmptyServer = "0.0.0.0";
        private const int DefaultPort = 5000;
        private const int DefaultMaxStreamThreads = 10;

        private readonly object _lock = new object();
        private readonly Dictionary<string, Thread> _threads = new Dictionary<string, Thread>();
        private readonly IEnumerable<string> _prefixes;
        private HttpListener _listener;
        private CancellationTokenSource _stop;
        private int _maxStreamThreads = DefaultMaxStreamThreads;
        private int _streamThreadCount = 0;


        /// <summary>
        /// Event Handler for when the HttpListener is started
        /// </summary>
        /// <returns>URL Prefix that the HttpListener is listening for requests on</returns>
        public event EventHandler<string> ListenerStarted;

        /// <summary>
        /// Event Handler for when the HttpListener is stopped
        /// </summary>
        /// <returns>URL Prefix that the HttpListener is listening for requests on</returns>
        public event EventHandler<string> ListenerStopped;

        /// <summary>
        /// Event Handler for when an error occurs with the HttpListener
        /// </summary>
        public event EventHandler<Exception> ListenerException;


        /// <summary>
        /// Event Handler for when a client makes a request to the server
        /// </summary>
        public event EventHandler<HttpListenerRequest> ClientConnected;

        /// <summary>
        /// Event Handler for when a client completes a request or disconnects from the server
        /// </summary>
        public event EventHandler<string> ClientDisconnected;

        /// <summary>
        /// Event Handler for when an error occurs with the HttpListenerRequest
        /// </summary>
        public event EventHandler<Exception> ClientException;


        public HttpServer(IHttpAgentConfiguration configuration, IEnumerable<string> prefixes = null, int port = 0)
        {
            if (configuration != null)
            {
                _maxStreamThreads = Math.Max(configuration.MaxStreamingThreads, 1);                
            }

            _prefixes = CreatePrefixes(configuration, prefixes, port);
        }


        public void Start()
        {
            _stop = new CancellationTokenSource();

            _ = Task.Run(() => Worker(_stop.Token));
        }

        public void Stop()
        {
            if (_stop != null) _stop.Cancel();

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



        private async Task Worker(CancellationToken cancellationToken)
        {
            do
            {
                bool errorOccurred = false;

                try
                {
                    // (Access Denied - Exception)
                    // Must grant permissions to use URL (for each Prefix) in Windows using the command below
                    // CMD: netsh http add urlacl url = "http://localhost/" user = everyone

                    // (Service Unavailable - HTTP Status)
                    // Multiple urls are configured using netsh that point to the same place

                    _listener = new HttpListener();
                    _listener.TimeoutManager.IdleConnection = TimeSpan.FromSeconds(10000);

                    // Add Prefixes
                    foreach (var prefix in _prefixes)
                    {
                        _listener.Prefixes.Add(prefix);
                    }

                    // Start Listener
                    _listener.Start();

                    // Raise Events to notify when a prefix is being listened on
                    if (ListenerStarted != null)
                    {
                        foreach (var prefix in _prefixes) ListenerStarted.Invoke(this, prefix);
                    }

                    // Listen for Requests
                    while (_listener.IsListening && !cancellationToken.IsCancellationRequested)
                    {
                        var context = await _listener.GetContextAsync();
                        if (context != null) ProcessContext(context);
                    }
                }
                catch (Exception ex)
                {
                    errorOccurred = true;

                    // Ignore Aborted Exception
                    if (ListenerException != null && ex.HResult != -2147467259)
                    {
                        ListenerException.Invoke(this, ex);
                    }
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
                if (errorOccurred) await Task.Delay(1000);

            } while (!cancellationToken.IsCancellationRequested);
        }

        private void ProcessContext(HttpListenerContext context)
        {
            try
            {
                // Determine if the request is for a stream
                // -- IsStreamRequest() = true   :  Create new Thread (custom Thread pool)
                // -- IsStreamRequest() = false  :  Run as Task (uses .NET ThreadPool)
                if (IsStreamRequest(context))
                {
                    StartStreamThread(context);
                }
                else
                {
                    StartRequestThread(context);
                }
            }
            catch (HttpListenerException ex)
            {
                // Ignore Disposed Object Exception (happens when the listener is stopped)
                if (ex.ErrorCode != 995)
                {
                    if (ClientException != null) ClientException.Invoke(this, ex);
                }
            }
            catch (ObjectDisposedException) { }
            catch (Exception ex)
            {
                if (ClientException != null) ClientException.Invoke(this, ex);
            }
        }

        private void StartStreamThread(HttpListenerContext context)
        {
            var contextId = Guid.NewGuid().ToString();

            try
            {
                int threadCount;
                lock (_lock) threadCount = _streamThreadCount;

                if (threadCount < _maxStreamThreads)
                {
                    // Create a unique ID for the Thread
                    var threadId = Guid.NewGuid().ToString();

                    // Create new Thread
                    var thread = new Thread(async () =>
                    {
                        // Run the Overridable method
                        await OnRequestReceived(context);

                        // Close the Response Stream
                        context.Response.Close();
                        lock (_lock) _streamThreadCount--;
                    });

                    // Add Thread to Dictionary
                    lock (_lock)
                    {
                        _threads.Add(threadId, thread);
                        _streamThreadCount++;
                    }

                    // Start the Thread
                    thread.Start();
                }
                else
                {
                    // Return Service Unavailable (503) Http Status Code
                    context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;

                    // Close the Response Stream
                    context.Response.Close();
                }
            }
            catch (HttpListenerException ex)
            {
                // Ignore Disposed Object Exception (happens when the listener is stopped)
                if (ex.ErrorCode != 995)
                {
                    if (ClientException != null) ClientException.Invoke(this, ex);
                }
            }
            catch (ObjectDisposedException) { }
            catch (Exception ex)
            {
                if (ClientException != null) ClientException.Invoke(this, ex);
            }
        }

        private void StartRequestThread(HttpListenerContext context)
        {
            // Create new Task to run Request on
            _ = Task.Run(async () =>
            {
                try
                {
                    // Run the Overridable method
                    await OnRequestReceived(context);

                    // Close the Response Stream
                    if (context != null) context.Response.Close();
                }
                catch (TaskCanceledException) { }
                catch (HttpListenerException ex)
                {
                    // Ignore Disposed Object Exception (happens when the listener is stopped)
                    if (ex.ErrorCode != 995)
                    {
                        if (ClientException != null) ClientException.Invoke(this, ex);
                    }
                }
                catch (ObjectDisposedException) { }
                catch (Exception ex)
                {
                    if (ClientException != null) ClientException.Invoke(this, ex);
                }
            });
        }


        protected virtual async Task OnRequestReceived(HttpListenerContext context) 
        {
            await Task.Delay(1); // Placeholder as this method is meant to be overridden
        }

        protected void OnClientConnected(HttpListenerRequest request)
        {
            if (ClientConnected != null) ClientConnected.Invoke(this, request);
        }

        protected void OnClientDisconnected(string clientId)
        {
            if (ClientDisconnected != null) ClientDisconnected.Invoke(this, clientId);
        }

        protected virtual bool IsStreamRequest(HttpListenerContext context)
        {
            return false;
        }

        protected static IEnumerable<string> GetRequestHeaderValues(HttpListenerRequest request, string name)
        {
            if (request != null && request.Headers != null && !string.IsNullOrEmpty(name))
            {
                var values = request.Headers.Get(name);
                if (values != null)
                {
                    // Trim Values (StringSplitOptions.TrimEntities is not compatible with older .NET versions)
                    var x = values.Split(',');
                    var y = new string[x.Length];
                    for (var i = 0; i < x.Length; i++)
                    {
                        if (x[i] != null) y[i] = x[i].Trim();
                    }
                    return y;
                }
            }

            return null;
        }

        private static IEnumerable<string> CreatePrefixes(IHttpAgentConfiguration configuration, IEnumerable<string> prefixes = null, int port = 0)
        {
            var serverPort = DefaultPort;
            var hostnames = new List<string>();
            hostnames.Add(Localhost);
            hostnames.Add(Loopback);

            if (!prefixes.IsNullOrEmpty())
            {
                // Add custom Prefixes
                hostnames.AddRange(prefixes);
            }
            else if (configuration != null)
            {
                // Configuration Server IP
                if (!string.IsNullOrEmpty(configuration.Server))
                {
                    if (configuration.Server != EmptyServer && 
                        configuration.Server != Loopback &&
                        configuration.Server != Localhost
                        )
                    {
                        hostnames.Add(configuration.Server);
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
            }

            // Construct Prefix URLs
            var x = new List<string>();
            foreach (var hostname in hostnames)
            {
                x.Add("http://" + hostname + ":" + serverPort + "/");
            }
            return x;
        }
    }
}