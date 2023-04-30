using System;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using System.IO;
using System.Net.Security;
using System.Collections.Generic;

namespace Ceen.Httpd
{
	/// <summary>
	/// The Http server implementation
	/// </summary>
	public static class HttpServer
	{
        /// <summary>
        /// Handler class that encapsulates a configured server,
        /// such that it is callable by an interprocess setup
        /// </summary>
        public class InterProcessBridge
        {
            /// <summary>
            /// The controller instance
            /// </summary>
            private RunnerControl Controller;
            /// <summary>
            /// The stop token source
            /// </summary>
            private CancellationTokenSource StopToken;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Ceen.Httpd.HttpServer.InterProcessBridge"/> class.
            /// </summary>
            protected InterProcessBridge()
            {
            }

            /// <summary>
            /// Setup this instance
            /// </summary>
            /// <param name="usessl">If set to <c>true</c> use ssl.</param>
            /// <param name="config">The configuration.</param>
            public InterProcessBridge(bool usessl, ServerConfig config)
            {
                Setup(usessl, config);
            }

            /// <summary>
            /// Setup this instance
            /// </summary>
            /// <param name="usessl">If set to <c>true</c> use ssl.</param>
            /// <param name="config">The configuration.</param>
            protected void Setup(bool usessl, ServerConfig config)
            {
                if (StopToken != null)
                    throw new Exception("Cannot call setup more than once");
                if (config == null)
                    throw new ArgumentNullException(nameof(config));

                StopToken = new CancellationTokenSource();
                Controller = new RunnerControl(StopToken.Token, usessl, config);
            }

            /// <summary>
            /// Handles a request
            /// </summary>
            /// <param name="socket">The socket to use.</param>
            /// <param name="remoteEndPoint">The remote endpoint.</param>
            /// <param name="logtaskid">The task ID to use.</param>
            public void HandleRequest(Socket socket, EndPoint remoteEndPoint, string logtaskid)
            {
                RunClient(socket, remoteEndPoint, logtaskid, Controller);
            }

            /// <summary>
            /// Handles a request
            /// </summary>
            /// <param name="stream">The stream to use.</param>
            /// <param name="remoteEndPoint">The remote endpoint.</param>
            /// <param name="logtaskid">The task ID to use.</param>
            /// <param name="isConnected">A method that checks if the socket is connected</param>
            public void HandleRequest(Stream stream, EndPoint remoteEndPoint, string logtaskid, Func<bool> isConnected)
            {
                RunClient(stream, remoteEndPoint, logtaskid, Controller, isConnected);
            }

            /// <summary>
            /// Handles a request
            /// </summary>
            /// <param name="stream">The stream to use.</param>
            /// <param name="remoteEndPoint">The remote endpoint.</param>
            /// <param name="logtaskid">The task ID to use.</param>
            /// <param name="isConnected">A method that checks if the socket is connected</param>
            public Task HandleRequestAsync(Stream stream, EndPoint remoteEndPoint, string logtaskid, Func<bool> isConnected)
            {
                return RunClient(stream, remoteEndPoint, logtaskid, Controller, isConnected);
            }


            /// <summary>
            /// Requests that this instance stops serving requests
            /// </summary>
            public void Stop()
            {
                StopToken.Cancel();
            }

            /// <summary>
            /// Returns an awaitable task that can be used to wait for termination
            /// </summary>
            /// <returns><c>true</c>, if for stop succeeded, <c>false</c> otherwise.</returns>
            public Task StopTask => Controller.FinishedTask;

            /// <summary>
            /// Gets the number of active clients.
            /// </summary>
            public int ActiveClients => Controller.ActiveClients;

        }

		/// <summary>
		/// Handler class that encapsulates a configured server setup,
		/// in a way that is callable from another AppDomain
		/// </summary>
		public class AppDomainBridge : MarshalByRefObject
		{
			/// <summary>
			/// The controller instance
			/// </summary>
			private RunnerControl Controller;
			/// <summary>
			/// The stop token source
			/// </summary>
			private CancellationTokenSource StopToken;

			/// <summary>
			/// Initializes a new instance of the <see cref="T:Ceen.Httpd.HttpServer.AppDomainBridge"/> class.
			/// </summary>
			public AppDomainBridge() { }

			/// <summary>
			/// Initializes a new instance of the <see cref="T:Ceen.Httpd.HttpServer.AppDomainBridge"/> class.
			/// </summary>
			/// <param name="usessl">If set to <c>true</c> use ssl.</param>
			/// <param name="config">The server config.</param>
			public AppDomainBridge(bool usessl, ServerConfig config) { Setup(usessl, config); }

			/// <summary>
			/// Setup this instance
			/// </summary>
			/// <param name="usessl">If set to <c>true</c> usessl.</param>
			/// <param name="config">Config.</param>
			public void Setup(bool usessl, ServerConfig config)
			{
				if (StopToken != null)
					throw new Exception("Cannot call setup more than once");
				if (config == null)
					throw new ArgumentNullException(nameof(config));

				StopToken = new CancellationTokenSource();
				Controller = new RunnerControl(StopToken.Token, usessl, config);
			}

			/// <summary>
			/// Handles a request
			/// </summary>
			/// <param name="socket">The socket handle.</param>
			/// <param name="remoteEndPoint">The remote endpoint.</param>
			/// <param name="logtaskid">The task ID to use.</param>
			public void HandleRequest(SocketInformation socket, EndPoint remoteEndPoint, string logtaskid)
			{
				RunClient(socket, remoteEndPoint, logtaskid, Controller);
			}

			/// <summary>
			/// Requests that this instance stops serving requests
			/// </summary>
			public void Stop()
			{
				StopToken.Cancel();
			}

			/// <summary>
			/// Waits for all clients to finish processing
			/// </summary>
			/// <returns><c>true</c>, if for stop succeeded, <c>false</c> otherwise.</returns>
			/// <param name="waitdelay">The maximum time to wait for the clients to stop.</param>
			public bool WaitForStop(TimeSpan waitdelay)
			{
				return Controller.FinishedTask.Wait(waitdelay);
			}

			/// <summary>
			/// Gets the number of active clients.
			/// </summary>
			public int ActiveClients { get { return Controller.ActiveClients; } }

			/// <summary>
			/// Initializes the lifetime service.
			/// </summary>
			/// <returns>The lifetime service.</returns>
			public override object InitializeLifetimeService()
			{
				return null;
			}
		}

		/// <summary>
		/// Helper class to keep track of all active requests and potentially abort them
		/// </summary>
		private class RunnerControl
		{
			/// <summary>
			/// Backing field for the total number of active clients
			/// </summary>
			private static int m_totalActiveClients;
            /// <summary>
            /// Gets the total number of active clients.
            /// </summary>
            /// <value>The total active clients.</value>
            public static int TotalActiveClients => m_totalActiveClients;
            /// <summary>
            /// Backing field for the number of active clients
            /// </summary>
            private int m_activeClients;
            /// <summary>
            /// The number of active clients
            /// </summary>
            public int ActiveClients => m_activeClients;
            /// <summary>
            /// The task that signals stopping the server
            /// </summary>
            /// <value>The stop task.</value>
            public Task StopTask => m_stoptask.Task;
            /// <summary>
            /// The task signalling that all clients have completed
            /// </summary>
            /// <value>The finished task.</value>
            public Task FinishedTask => m_finishedtask.Task;
            /// <summary>
            /// Gets a task that throttles start of new handlers
            /// </summary>
            /// <value>The throttle task.</value>
            public Task ThrottleTask => m_throttletask.Task;
            /// <summary>
            /// Gets the server configuration
            /// </summary>
            /// <value>The config.</value>
            public ServerConfig Config { get; private set; }

			/// <summary>
			/// The stop token
			/// </summary>
			public readonly CancellationToken StopToken;
			/// <summary>
			/// A flag indicating if SSL is used
			/// </summary>
			public readonly bool m_useSSL;
			/// <summary>
			/// A value indicating if the server is stopped
			/// </summary>
			public volatile bool m_isStopped;
            /// <summary>
            /// Gets a value indicating whether this <see cref="T:Ceen.Httpd.HttpServer.RunnerControl"/> is stopped.
            /// </summary>
            public bool IsStopped => m_isStopped;
            /// <summary>
            /// The maximum number of active handlers
            /// </summary>
            private readonly int m_maxactive;
			/// <summary>
			/// The lock object
			/// </summary>
			private readonly object m_lock = new object();

			/// <summary>
			/// The task used to signal all requests are stopped
			/// </summary>
			private readonly TaskCompletionSource<bool> m_finishedtask = new TaskCompletionSource<bool>();
			/// <summary>
			/// The task used to signal all handlers to stop
			/// </summary>
			private readonly TaskCompletionSource<bool> m_stoptask = new TaskCompletionSource<bool>();
			/// <summary>
			/// The task used to signal waiting for handlers to complete before starting new handlers
			/// </summary>
			private TaskCompletionSource<bool> m_throttletask = new TaskCompletionSource<bool>();

			/// <summary>
			/// A logger for reporting the internal log state
			/// </summary>
			private readonly DebugLogDelegate m_debuglogger;

			/// <summary>
			/// Initializes a new instance of the <see cref="Ceen.Httpd+RunnerControl"/> class.
			/// </summary>
			/// <param name="stoptoken">The stoptoken.</param>
			/// <param name="usessl">A flag indicating if this runner is using SSL</param>
			/// <param name="config">The server config.</param>
			public RunnerControl(CancellationToken stoptoken, bool usessl, ServerConfig config)
			{
				if (config == null)
					throw new ArgumentNullException(nameof(config));

				StopToken = stoptoken;
				StopToken.Register(() => Stop("_"));
				Config = config;
				m_maxactive = config.MaxActiveRequests;
				m_throttletask.SetResult(true);
				m_debuglogger = config.DebugLogHandler;
				m_useSSL = usessl;
			}

			/// <summary>
			/// Called by a handler to signal it is in the active state
			/// </summary>
			/// <param name="logtaskid">The task id used for logging and tracing</param>
			/// <returns><c>true</c>, if active was registered, <c>false</c> otherwise.</returns>
			public bool RegisterActive(string logtaskid)
			{
				if (m_debuglogger != null) m_debuglogger("RegisterActive", logtaskid, null);

				if (m_isStopped)
					return false;

				var res = Interlocked.Increment(ref m_activeClients);
				Interlocked.Increment(ref m_totalActiveClients);

				if (m_debuglogger != null) m_debuglogger(string.Format("RegisterActive: {0}", res), logtaskid, null);

				// If we have too many active, block the throttle task
				if (res >= m_maxactive)
				{
					if (m_debuglogger != null) m_debuglogger("Blocking throttle", logtaskid, null);
					lock (m_lock)
						if (m_throttletask.Task.IsCompleted)
							m_throttletask = new TaskCompletionSource<bool>();
				}

				return true;
			}

			/// <summary>
			/// Called by a handler to signal it has completed handling a request
			/// <param name="logtaskid">The task id used for logging and tracing</param>
			/// </summary>
			public void RegisterStopped(string logtaskid)
			{
				var res = Interlocked.Decrement(ref m_activeClients);
				Interlocked.Decrement(ref m_totalActiveClients);

				if (m_debuglogger != null) m_debuglogger(string.Format("RegisterStopped: {0}", res), logtaskid, null);

				// If the throttle task is blocked and we have few active, unblock it
				if (res < m_maxactive && !m_throttletask.Task.IsCompleted)
				{
					if (m_debuglogger != null) m_debuglogger("Un-blocking throttle", logtaskid, null);
					lock (m_lock)
						if (!m_throttletask.Task.IsCompleted)
							m_throttletask.SetResult(true);
				}


				if (m_isStopped && res == 0)
				{
					if (m_debuglogger != null) m_debuglogger("Stopped and setting finish task", logtaskid, null);
					m_finishedtask.TrySetResult(true);
				}
			}

			/// <summary>
			/// Called to stop handling requests
			/// </summary>
			public void Stop(string logtaskid)
			{
				m_stoptask.TrySetCanceled();
				m_isStopped = true;

				lock (m_lock)
				{
					if (m_activeClients == 0)
					{
						if (m_debuglogger != null) m_debuglogger("Stopping, no active workers", logtaskid, null);
						m_finishedtask.TrySetResult(true);
					}
					else
					{
						if (m_debuglogger != null) m_debuglogger(string.Format("Stopping with {0} active workers", m_activeClients), logtaskid, null);
					}
				}
			}
		}

		/// <summary>
		/// Gets the total number of active clients
		/// </summary>
		/// <value>The total active clients.</value>
		public static int TotalActiveClientCount { get { return RunnerControl.TotalActiveClients; } }

		/// <summary>
		/// The method used to set the current socket handlerID in log4net, if available.
		/// This redirection method is used to avoid depending on log4net.
		/// </summary>
		private static readonly Func<string> SetLoggingSocketHandlerID;

		/// <summary>
		/// The method used to set the current taskID in log4net, if available.
		/// This redirection method is used to avoid depending on log4net.
		/// </summary>
		private static readonly Func<string> SetLoggingTaskHandlerID;

		/// <summary>
		/// The method used to set the current requestID in log4net, if available.
		/// This redirection method is used to avoid depending on log4net.
		/// </summary>
		private static readonly Func<string> SetLoggingRequestID;

		/// <summary>
		/// The method used to copy logdata to log4net properties, if available.
		/// This redirection method is used to avoid depending on log4net.
		/// </summary>
		private static readonly Action<IHttpContext> CopyLogData;

		/// <summary>
		/// The name of the log4net property that has the socket handler ID
		/// </summary>
		public static readonly string Log4Net_SocketHandlerID = "ceen:SocketHandlerID";
		/// <summary>
		/// The name of the log4net property that has the task handler ID
		/// </summary>
		public static readonly string Log4Net_TaskHandlerID = "ceen:TaskHandlerID";
		/// <summary>
		/// The name of the log4net property that has the request ID
		/// </summary>
		public static readonly string Log4Net_RequestID = "ceen:RequestID";

		/// <summary>
		/// Static initialization for the HttpServer class,
		/// used to check for log4net dynamically
		/// </summary>
		static HttpServer()
		{
			Func<string> socketId = () => Guid.NewGuid().ToString("N");
			Func<string> taskId = () => Guid.NewGuid().ToString("N");
			Func<string> requestId = () => Guid.NewGuid().ToString("N");
			Action<IHttpContext> copylogdata = null;

			// Slowly probe through to get the method
			var t = Type.GetType("log4net.LogicalThreadContext, log4net, Culture=neutral");

			var index_socket = new object[] { Log4Net_SocketHandlerID };
			var index_task = new object[] { Log4Net_TaskHandlerID };
			var index_request = new object[] { Log4Net_RequestID };

			if (t != null)
			{
				var m = t.GetProperty("Properties");
				if (m != null)
				{
					var ins = m.GetValue(null, null);
					if (ins != null)
					{
						var rm = ins.GetType().GetProperties().FirstOrDefault(x => x.GetIndexParameters().Length > 0);
						if (rm != null)
						{
							// We have a default indexer, set up the helper methods
							socketId = () =>
							{
								var g = Guid.NewGuid().ToString("N");
								rm.SetValue(ins, g, index_socket);
								return g;
							};

							taskId = () =>
							{
								var g = Guid.NewGuid().ToString("N");
								rm.SetValue(ins, g, index_task);
								return g;
							};

							requestId = () =>
							{
								var g = Guid.NewGuid().ToString("N");
								rm.SetValue(ins, g, index_request);
								return g;
							};

							copylogdata = (context) =>
							{
								if (context != null && context.LogData != null)
									foreach (var kp in context.LogData)
										rm.SetValue(ins, kp.Value, new object[] { kp.Key });
							};
						}
					}
				}
			}

			// Assign whatever value we had
			SetLoggingSocketHandlerID = socketId;
			SetLoggingTaskHandlerID = taskId;
			SetLoggingRequestID = requestId;
			CopyLogData = copylogdata;
		}

		/// <summary>
		/// Creates and initializes a new AppDomain bridge
		/// </summary>
		/// <returns>The app domain bridge.</returns>
		/// <param name="usessl">If set to <c>true</c> use ssl.</param>
		/// <param name="config">The server config.</param>
		public static AppDomainBridge CreateAppDomainBridge(bool usessl, ServerConfig config)
		{
			return new AppDomainBridge(usessl, config);
		}

        /// <summary>
        /// Creates and initializes a new InterProcess bridge
        /// </summary>
        /// <returns>The app domain bridge.</returns>
        /// <param name="usessl">If set to <c>true</c> use ssl.</param>
        /// <param name="config">The server config.</param>
        public static InterProcessBridge CreateInterProcessBridge(bool usessl, ServerConfig config)
        {
            return new InterProcessBridge(usessl, config);
        }

		/// <summary>
		/// Listens to incoming connections and calls the spawner method for each new connection
		/// </summary>
		/// <returns>Awaitable task.</returns>
		/// <param name="addr">The address to listen to.</param>
		/// <param name="usessl">A flag indicating if the socket listens for SSL requests</param>
		/// <param name="stoptoken">The stoptoken.</param>
		/// <param name="config">The server configuration</param>
		/// <param name="spawner">The method handling the new connection.</param>
		public static Task ListenToSocketAsync(EndPoint addr, bool usessl, CancellationToken stoptoken, ServerConfig config, Action<Socket, EndPoint, string> spawner)
		{
			return ListenToSocketInternalAsync(addr, usessl, stoptoken, config, (client, remoteendpoint, logid, controller) => spawner(client, remoteendpoint, logid));
		}

        /// <summary>
        /// Listens to incoming connections and calls the spawner method for each new connection
        /// </summary>
        /// <returns>Awaitable task.</returns>
        /// <param name="acceptAsync">Method that returns an accepted socket.</param>
        /// <param name="usessl">A flag indicating if the socket listens for SSL requests</param>
        /// <param name="stoptoken">The stoptoken.</param>
        /// <param name="config">The server configuration</param>
        /// <param name="spawner">The method handling the new connection.</param>
        public static Task ListenToSocketAsync(Func<CancellationToken, Task<KeyValuePair<long, EndPoint>>> acceptAsync, bool usessl, CancellationToken stoptoken, ServerConfig config, Action<long, EndPoint, string> spawner)
        {
            return ListenToSocketInternalAsync(acceptAsync, usessl, stoptoken, config, (client, remoteendpoint, logid, controller) => spawner(client, remoteendpoint, logid));
        }

        /// <summary>
        /// Listens to incoming connections and calls the spawner method for each new connection
        /// </summary>
        /// <returns>Awaitable task.</returns>
        /// <param name="acceptAsync">Method that returns an accepted socket.</param>
        /// <param name="usessl">A flag indicating if the socket listens for SSL requests</param>
        /// <param name="stoptoken">The stoptoken.</param>
        /// <param name="config">The server configuration</param>
        /// <param name="spawner">The method handling the new connection.</param>
        private static async Task ListenToSocketInternalAsync(Func<CancellationToken, Task<KeyValuePair<long, EndPoint>>> acceptAsync, bool usessl, CancellationToken stoptoken, ServerConfig config, Action<long, EndPoint, string, RunnerControl> spawner)
        {
            if (acceptAsync == null)
                throw new ArgumentNullException(nameof(acceptAsync));

            var rc = new RunnerControl(stoptoken, usessl, config);

            var taskid = SetLoggingSocketHandlerID();

            while (!stoptoken.IsCancellationRequested)
            {
                // Wait if there are too many active
                config.DebugLogHandler?.Invoke("Waiting for throttle", taskid, null);
                await rc.ThrottleTask;
                config.DebugLogHandler?.Invoke("Waiting for socket", taskid, null);
                var ls = acceptAsync(stoptoken);

                if (await Task.WhenAny(rc.StopTask, ls) == ls)
                {
                    config.DebugLogHandler?.Invoke("Re-waiting for socket", taskid, null);
                    var client = await ls;
                    var newtaskid = SetLoggingTaskHandlerID();

                    try
                    {
                        int wt, cpt;
                        ThreadPool.GetAvailableThreads(out wt, out cpt);
                        config.DebugLogHandler?.Invoke(string.Format("Threadpool says {0}, {1}", wt, cpt), taskid, newtaskid);
                        config.DebugLogHandler?.Invoke(string.Format("Spawning runner with id: {0}", newtaskid), taskid, newtaskid);

                        ThreadPool.QueueUserWorkItem(x => spawner(client.Key, client.Value, newtaskid, rc));
                    }
                    catch (Exception ex)
                    {
                        config.DebugLogHandler?.Invoke("Failed to listen to socket", taskid, ex);
                    }
                }
            }

            config.DebugLogHandler?.Invoke("Stopping", taskid, null);
            rc.Stop(taskid);

            config.DebugLogHandler?.Invoke("Socket stopped, waiting for modules ...", taskid, null);
            await Task.WhenAny(config.ShutdownAsync(), Task.Delay(5000));

            config.DebugLogHandler?.Invoke("Socket and modules stopped, waiting for workers ...", taskid, null);
            await rc.FinishedTask;

            config.DebugLogHandler?.Invoke("Stopped", taskid, null);
        }

        /// <summary>
        /// Listens to incoming connections and calls the spawner method for each new connection
        /// </summary>
        /// <returns>Awaitable task.</returns>
        /// <param name="addr">The address to listen to.</param>
        /// <param name="usessl">A flag indicating if the socket listens for SSL requests</param>
        /// <param name="stoptoken">The stoptoken.</param>
        /// <param name="config">The server configuration</param>
        /// <param name="spawner">The method handling the new connection.</param>
		private static async Task ListenToSocketInternalAsync(EndPoint addr, bool usessl, CancellationToken stoptoken, ServerConfig config, Action<Socket, EndPoint, string, RunnerControl> spawner)
        {
            var rc = new RunnerControl(stoptoken, usessl, config);
            var socket = SocketUtil.CreateAndBindSocket(addr, config.SocketBacklog);

            var taskid = SetLoggingSocketHandlerID();

            while (!stoptoken.IsCancellationRequested)
            {
#if NET5_0_OR_GREATER

                // Wait if there are too many active
                config.DebugLogHandler?.Invoke("Waiting for throttle", taskid, null);
                await rc.ThrottleTask;
                config.DebugLogHandler?.Invoke("Waiting for socket", taskid, null);
                var ls = socket.AcceptAsync();

                if (await Task.WhenAny(rc.StopTask, ls) == ls)
                {
                    config.DebugLogHandler?.Invoke("Re-waiting for socket", taskid, null);
                    var client = await ls;
                    var newtaskid = SetLoggingTaskHandlerID();

                    try
                    {
                        int wt, cpt;
                        ThreadPool.GetAvailableThreads(out wt, out cpt);
                        config.DebugLogHandler?.Invoke(string.Format("Threadpool says {0}, {1}", wt, cpt), taskid, newtaskid);

                        config.DebugLogHandler?.Invoke(string.Format("Spawning runner with id: {0}", newtaskid), taskid, newtaskid);

                        // Read the endpoint here to avoid crashes when invoking the spawner
                        var ep = client.RemoteEndPoint;
                        ThreadPool.QueueUserWorkItem(x => spawner(client, ep, newtaskid, rc));
                    }
                    catch (Exception ex)
                    {
                        config.DebugLogHandler?.Invoke("Failed to listen to socket", taskid, ex);
                    }
                }

#else

                // Wait if there are too many active
                config.DebugLogHandler?.Invoke("Waiting for throttle", taskid, null);
                await rc.ThrottleTask;
                config.DebugLogHandler?.Invoke("Waiting for socket", taskid, null);
                var client = socket.Accept();

                var newtaskid = SetLoggingTaskHandlerID();

                try
                {
                    int wt, cpt;
                    ThreadPool.GetAvailableThreads(out wt, out cpt);
                    config.DebugLogHandler?.Invoke(string.Format("Threadpool says {0}, {1}", wt, cpt), taskid, newtaskid);

                    config.DebugLogHandler?.Invoke(string.Format("Spawning runner with id: {0}", newtaskid), taskid, newtaskid);

                    // Read the endpoint here to avoid crashes when invoking the spawner
                    var ep = client.RemoteEndPoint;
                    ThreadPool.QueueUserWorkItem(x => spawner(client, ep, newtaskid, rc));
                }
                catch (Exception ex)
                {
                    config.DebugLogHandler?.Invoke("Failed to listen to socket", taskid, ex);
                }

#endif
            }

            config.DebugLogHandler?.Invoke("Stopping", taskid, null);

            socket.Close();
            rc.Stop(taskid);

            config.DebugLogHandler?.Invoke("Socket stopped, waiting for workers ...", taskid, null);
            await rc.FinishedTask;

            config.DebugLogHandler?.Invoke("Stopped", taskid, null);
        }

        //      private static async Task ListenToSocketInternalAsync(EndPoint addr, bool usessl, CancellationToken stoptoken, ServerConfig config, Action<Socket, EndPoint, string, RunnerControl> spawner)
        //{
        //	var rc = new RunnerControl(stoptoken, usessl, config);
        //	var socket = SocketUtil.CreateAndBindSocket(addr, config.SocketBacklog);

        //	var taskid = SetLoggingSocketHandlerID();

        //	while (!stoptoken.IsCancellationRequested)
        //	{
        //              // Wait if there are too many active
        //              config.DebugLogHandler?.Invoke("Waiting for throttle", taskid, null);
        //              await rc.ThrottleTask;
        //              config.DebugLogHandler?.Invoke("Waiting for socket", taskid, null);
        //              var ls = socket.Accept();
        //              //var ls = socket.AcceptAsync();

        //		if (await Task.WhenAny(rc.StopTask, ls) == ls)
        //		{
        //                  config.DebugLogHandler?.Invoke("Re-waiting for socket", taskid, null);
        //                  var client = await ls;
        //			var newtaskid = SetLoggingTaskHandlerID();

        //			try
        //			{
        //				int wt, cpt;
        //				ThreadPool.GetAvailableThreads(out wt, out cpt);
        //                      config.DebugLogHandler?.Invoke(string.Format("Threadpool says {0}, {1}", wt, cpt), taskid, newtaskid);

        //                      config.DebugLogHandler?.Invoke(string.Format("Spawning runner with id: {0}", newtaskid), taskid, newtaskid);

        //                      // Read the endpoint here to avoid crashes when invoking the spawner
        //                      var ep = client.RemoteEndPoint;
        //				ThreadPool.QueueUserWorkItem(x => spawner(client, ep, newtaskid, rc));
        //			}
        //			catch(Exception ex)
        //			{
        //                      config.DebugLogHandler?.Invoke("Failed to listen to socket", taskid, ex);
        //                  }
        //		}
        //	}

        //          config.DebugLogHandler?.Invoke("Stopping", taskid, null);

        //          socket.Close();
        //	rc.Stop(taskid);

        //          config.DebugLogHandler?.Invoke("Socket stopped, waiting for workers ...", taskid, null);
        //          await rc.FinishedTask;

        //          config.DebugLogHandler?.Invoke("Stopped", taskid, null);
        //      }

        /// <summary>
        /// Logs a message to all attached loggers
        /// </summary>
        /// <param name="controller">The controller to get loggers from</param>
        /// <param name="context">The request context.</param>
        /// <param name="loglevel">The log level to use</param>
        /// <param name="exception">The exception to log, if any</param>
        /// <param name="message">The message to log</param>
        /// <param name="when">The time the message was logged</param>
        /// <returns>An awaitable task</returns>
        private static Task LogProcessingMessage(RunnerControl controller, HttpContext context, Exception ex, LogLevel loglevel, string message, DateTime when)
		{
            var config = controller.Config;
            if (config.Loggers == null)
                return Task.FromResult(true);

            CopyLogData?.Invoke(context);

            var count = config.Loggers.Count;
            if (count == 0)
                return Task.FromResult(true);
            else if (count == 1)
			{
				if (!(config.Loggers[0] is IMessageLogger msl))
					return Task.FromResult(true);
                return msl.LogMessageAsync(context, ex, loglevel, message, when);
			}
            else
                return Task.WhenAll(config.Loggers.OfType<IMessageLogger>().Select(x => x.LogMessageAsync(context, ex, loglevel, message, when)));
		}

		/// <summary>
		/// Logs a message signalling that a request has completed to all configured loggers
		/// </summary>
		/// <returns>The awaitable task.</returns>
		/// <param name="controller">The controller to get loggers from</param>
		/// <param name="context">The request context.</param>
		/// <param name="ex">Exception data, if any.</param>
		/// <param name="start">The request start time.</param>
		/// <param name="duration">The request duration.</param>
		private static Task LogRequestCompletedMessageAsync(RunnerControl controller, HttpContext context, Exception ex, DateTime start, TimeSpan duration)
		{
			var config = controller.Config;
			if (config.Loggers == null)
				return Task.FromResult(true);

            CopyLogData?.Invoke(context);

            var count = config.Loggers.Count;
			if (count == 0)
				return Task.FromResult(true);
			else if (count == 1)
				return config.Loggers[0].LogRequestCompletedAsync(context, ex, start, duration);
			else
				return Task.WhenAll(config.Loggers.Select(x => x.LogRequestCompletedAsync(context, ex, start, duration)));
		}

		/// <summary>
		/// Listens to a port, using the given endpoint. 
		/// </summary>
		/// <returns>The awaitable task.</returns>
		/// <param name="addr">The address to listen to.</param>
		/// <param name="usessl">A flag indicating if this instance should use SSL</param>
		/// <param name="config">The server configuration</param>
		/// <param name="stoptoken">The stoptoken.</param>
		public static Task ListenAsync(EndPoint addr, bool usessl, ServerConfig config, CancellationToken stoptoken = default(CancellationToken))
		{
			if (usessl && (config.SSLCertificate as X509Certificate2 == null || !(config.SSLCertificate as X509Certificate2).HasPrivateKey))
				throw new Exception("Certificate does not have a private key and cannot be used for signing");

			if (config.Storage == null)
				config.Storage = new MemoryStorageCreator();

			return ListenToSocketInternalAsync(addr, usessl, stoptoken, config, RunClient);
		}

		/// <summary>
		/// Runs a client, using a socket handle from DuplicateAndClose
		/// </summary>
		/// <param name="socketinfo">The socket handle.</param>
		/// <param name="remoteEndPoint">The remote endpoint.</param>
		/// <param name="logtaskid">The log task ID.</param>
		/// <param name="controller">The controller instance</param>
		private static void RunClient(SocketInformation socketinfo, EndPoint remoteEndPoint, string logtaskid, RunnerControl controller)
		{
			RunClient(new Socket(socketinfo), remoteEndPoint, logtaskid, controller);
		}

        /// <summary>
        /// Handler method for connections
        /// </summary>
        /// <param name="client">The new connection.</param>
        /// <param name="remoteEndPoint">The remote endpoint.</param>
        /// <param name="logtaskid">The task id for logging and tracing</param>
        /// <param name="controller">The runner controller.</param>
        private static async void RunClient(Socket client, EndPoint remoteEndPoint, string logtaskid, RunnerControl controller)
        {
            using (client)
                await RunClient(new NetworkStream(client), remoteEndPoint, logtaskid, controller, () => client.Connected);
        }

        /// <summary>
        /// Handler method for connections
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="remoteEndPoint">The remote endpoint.</param>
        /// <param name="logtaskid">The task id for logging and tracing</param>
        /// <param name="controller">The runner controller.</param>
        private static async Task RunClient(Stream stream, EndPoint remoteEndPoint, string logtaskid, RunnerControl controller, Func<bool> isConnected)
		{
			var config = controller.Config;
			var storage = config.Storage;

            using (stream)
			using (var ssl = controller.m_useSSL ? new SslStream(stream, false) : null)
			{
                config.DebugLogHandler?.Invoke(string.Format("Running {0}", controller.m_useSSL ? "SSL" : "plain"), logtaskid, remoteEndPoint);

                // Slightly higher value here to avoid races with the other timeout mechanisms
                stream.ReadTimeout = stream.WriteTimeout = (controller.Config.RequestIdleTimeoutSeconds + 1) * 1000;

				X509Certificate clientcert = null;

				// For SSL only: negotiate the connection
				if (ssl != null)
				{
                    config.DebugLogHandler?.Invoke("Authenticate SSL", logtaskid, remoteEndPoint);

                    try
					{
						await ssl.AuthenticateAsServerAsync(config.SSLCertificate, config.SSLRequireClientCert, config.SSLEnabledProtocols, config.SSLCheckCertificateRevocation);
					}
					catch (Exception aex)
					{
                        config.DebugLogHandler?.Invoke("Failed setting up SSL", logtaskid, remoteEndPoint);

                        // Log a message indicating that we failed setting up SSL                        
                        using (var httpRequest = new HttpRequest(remoteEndPoint, logtaskid, logtaskid, null, SslProtocols.None, () => false))
                            await LogRequestCompletedMessageAsync(controller, new HttpContext(httpRequest, null, storage, config), aex, DateTime.Now, new TimeSpan());

                        return;
					}

                    config.DebugLogHandler?.Invoke("Run SSL", logtaskid, remoteEndPoint);
                    clientcert = ssl.RemoteCertificate;
				}

                await Runner(ssl == null ? (Stream)stream : ssl, remoteEndPoint, logtaskid, clientcert, ssl == null ? SslProtocols.None : ssl.SslProtocol, controller, isConnected);

                config.DebugLogHandler?.Invoke("Done running", logtaskid, remoteEndPoint);
            }
		}

		/// <summary>
		/// Dispatcher method for handling a request
		/// </summary>
		/// <param name="stream">The underlying stream.</param>
		/// <param name="endpoint">The remote endpoint.</param>
		/// <param name="logtaskid">The task id for logging and tracing the connection</param>
		/// <param name="clientcert">The client certificate if any.</param>
		/// <param name="controller">The runner controller.</param>
        /// <param name="sslProtocol">The SSL protocol being used</param>
        /// <param name="isConnected">A method for checking if the socket is connected</param>
		private static async Task Runner(Stream stream, EndPoint endpoint, string logtaskid, X509Certificate clientcert, SslProtocols sslProtocol, RunnerControl controller, Func<bool> isConnected)
		{
			var config = controller.Config;
			var storage = config.Storage;
			var requests = config.KeepAliveMaxRequests;

			bool keepingalive = false;

			HttpContext context = null;
			HttpRequest cur = null;
			HttpResponse resp = null;
			DateTime started = new DateTime();

			using (var bs = new BufferedStreamReader(stream))
			{
				try
				{
                    config.DebugLogHandler?.Invoke("Running task", logtaskid, endpoint);
                    if (!controller.RegisterActive(logtaskid))
						return;

					do
                    {
                        cur?.Dispose();

                        var reqid = SetLoggingRequestID();
                        bs.ResetReadLength(config.MaxPostSize);
                        started = DateTime.Now;
                        context = new HttpContext(
                            cur = new HttpRequest(endpoint, logtaskid, reqid, clientcert, sslProtocol, isConnected),
                            resp = new HttpResponse(stream, config),
                            storage,
                            config
                        );

                        // Make sure the response knows the context
                        resp.Context = context;

                        // Setup up the callback for allowing handlers to report errors
                        context.LogHandlerDelegate = (level, message, ex) => LogProcessingMessage(controller, context, ex, level, message, DateTime.Now);

                        // Set up call context access to this instance
                        Context.SetCurrentContext(context);

                        var timeoutcontroltask = new TaskCompletionSource<bool>();
                        var idletime = TimeSpan.FromSeconds(config.RequestHeaderReadTimeoutSeconds);

                        // Set up timeout for processing
                        cur.SetProcessingTimeout(TimeSpan.FromSeconds(config.MaxProcessingTimeSeconds));

                        config.DebugLogHandler?.Invoke("Parsing headers", logtaskid, endpoint);
                        try
                        {
                            var ct = new CancellationTokenSource();
                            ct.CancelAfter(TimeSpan.FromSeconds(keepingalive ? config.KeepAliveTimeoutSeconds : config.RequestIdleTimeoutSeconds));
                            using (ct.Token.Register(() => timeoutcontroltask.TrySetCanceled(), useSynchronizationContext: false))
                                await cur.Parse(bs, config, idletime, timeoutcontroltask.Task, controller.StopTask);
                        }
                        catch (EmptyStreamClosedException)
                        {
                            // Client has closed the connection
                            break;
                        }
                        catch (HttpException hex)
                        {
							// Since we throw, make sure we log this incomplete request
                            await LogRequestStartedAsync(config, cur);

                            // Errors during header parsing are unlikely to
                            // keep the connection in a consistent state
                            resp.KeepAlive = false;
                            resp.StatusCode = hex.StatusCode;
                            resp.StatusMessage = hex.StatusMessage;
                            await resp.FlushHeadersAsync();

                            throw;
                        }
                        catch (Exception ex)
                        {
                            config.DebugLogHandler?.Invoke($"Failed while reading header: {ex}", logtaskid, cur);

                            // Since we throw, make sure we log this incomplete request
                            await LogRequestStartedAsync(config, cur);

                            throw;
                        }

                        string keepalive;
                        cur.Headers.TryGetValue("Connection", out keepalive);
                        if (("keep-alive".Equals(keepalive, StringComparison.OrdinalIgnoreCase) || keepingalive) && requests > 1)
                        {
                            resp.KeepAlive = true;
                            if (!keepingalive)
                                resp.AddHeader("Keep-Alive", string.Format("timeout={0}, max={1}", config.KeepAliveTimeoutSeconds, config.KeepAliveMaxRequests));
                        }
                        else
                            resp.KeepAlive = false;

						// Inform loggers of the request with all fields filled
                        await LogRequestStartedAsync(config, cur);

                        config.DebugLogHandler?.Invoke("Running handler", logtaskid, cur);

                        try
                        {
                            // Trigger the streams to stop reading/writing data when the timeout happens
                            using (cur.TimeoutCancellationToken.Register(() => timeoutcontroltask.TrySetCanceled(), useSynchronizationContext: false))
                            {
                                if (cur.TimeoutCancellationToken.IsCancellationRequested)
                                {
                                    if (timeoutcontroltask.Task.Status == TaskStatus.Canceled)
                                    {
                                        throw new TimeoutException();
                                    }
                                    else
                                    {
                                        timeoutcontroltask.TrySetCanceled();
                                        throw new OperationCanceledException();
                                    }
                                }

                                // Process the request
                                do
                                {
                                    cur.ClearHandlerStack();
                                    var target = resp.ClearInternalRedirect();
                                    if (target != null)
                                        cur.Path = target;

                                    if (!await config.Router.Process(context))
                                        throw new HttpException(Ceen.HttpStatusCode.NotFound);
                                }
                                while (resp.IsRedirectingInternally);
                            }
                        }
                        catch (HttpException hex)
                        {
                            // Try to set the status code if possible
                            if (resp.HasSentHeaders)
                                throw;

                            resp.StatusCode = hex.StatusCode;
                            resp.StatusMessage = hex.StatusMessage;
                        }

                        config.DebugLogHandler?.Invoke("Flushing response", logtaskid, cur);

						// We must consume the entire body, 
						// otherwise we do not know when the next header starts
						var allBytesRead = false;

						// Empty the body, if possible
						if (resp.KeepAlive && cur.Body is LimitedBodyStream lbs)
							allBytesRead = await lbs.DiscardAllAsync(cur.TimeoutCancellationToken);

                        // Toggle the keep-alive flag if possible
                        if (resp.KeepAlive && !resp.HasSentHeaders && !allBytesRead)
                            resp.KeepAlive = false;

                        // If the handler has not flushed, we do it
                        await resp.FlushAndSetLengthAsync(cur.TimeoutCancellationToken);
						await resp.FlushStreamAsync(cur.TimeoutCancellationToken);

						// Request completed without failures
                        await LogRequestCompletedMessageAsync(controller, context, null, started, DateTime.Now - started);

                        // Check if keep-alive is possible
                        keepingalive = resp.KeepAlive && resp.HasWrittenCorrectLength && allBytesRead;
                        requests--;

                    } while (keepingalive);

					// Prevent flushing synchronously by the Close() or Dispose() call
					using(var cts = new CancellationTokenSource(1000))
					{
						try { await stream.FlushAsync(cts.Token); }
						catch (Exception nex) { config.DebugLogHandler?.Invoke($"Failed to flush stream: {nex}", logtaskid, cur); }
					}
				}
				catch (Exception ex)
				{
					// If possible, report a 500 error to the client
                    if (resp != null)
					{
						try
						{
							if (!resp.HasSentHeaders)
							{
								resp.KeepAlive = false;
								resp.StatusCode = Ceen.HttpStatusCode.InternalServerError;
								resp.StatusMessage = HttpStatusMessages.DefaultMessage(Ceen.HttpStatusCode.InternalServerError);
							}
						}
                        catch (Exception nex) { config.DebugLogHandler?.Invoke($"Failed to send headers: {nex}", logtaskid, cur); }

						try { await resp.FlushAsErrorAsync(); }
                        catch (Exception nex) { config.DebugLogHandler?.Invoke($"Failed to FlushAsErrors: {nex}", logtaskid, cur); }
                    }

					// Prevent flushing synchronously by the Close() or Dispose() call
					using(var cts = new CancellationTokenSource(1000))
					{
						if (resp != null)
							try { await resp.FlushStreamAsync(cts.Token); }
							catch (Exception nex) { config.DebugLogHandler?.Invoke($"Failed to flush resp: {nex}", logtaskid, cur); }
						
						try { await stream.FlushAsync(cts.Token); }
						catch (Exception nex) { config.DebugLogHandler?.Invoke($"Failed to flush stream: {nex}", logtaskid, cur); }
					}

                    try { stream.Close(); }
                    catch (Exception nex) { config.DebugLogHandler?.Invoke($"Failed to close stream: {nex}", logtaskid, cur); }

                    context?.LogWarningAsync("Request handler failed", ex);

                    try { await LogRequestCompletedMessageAsync(controller, context, ex, started, DateTime.Now - started); }
                    catch (Exception nex) { config.DebugLogHandler?.Invoke($"Failed to log request: {nex}", logtaskid, cur); }

                    config.DebugLogHandler?.Invoke("Failed handler", logtaskid, cur);
                }
				finally
				{
					controller.RegisterStopped(logtaskid);
                    config.DebugLogHandler?.Invoke("Terminating handler", logtaskid, cur);
                }
			}
		}

        private static async Task LogRequestStartedAsync(ServerConfig config, HttpRequest cur)
        {
            if (config.Loggers != null)
            {
                var count = config.Loggers.Count;
                if (count == 1)
                {
                    var sl = config.Loggers[0] as IStartLogger;
                    if (sl != null)
                        await sl.LogRequestStartedAsync(cur);
                }
                else if (count != 0)
                    await Task.WhenAll(config.Loggers.Where(x => x is IStartLogger).Cast<IStartLogger>().Select(x => x.LogRequestStartedAsync(cur)));
            }
        }
    }

}