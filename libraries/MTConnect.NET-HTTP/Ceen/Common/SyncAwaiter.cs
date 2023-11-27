using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Ceen.Common
{
    /// <summary>
    /// Helper class to provide synchronous waiting for asynchronous methods without deadlocking
    /// </summary>
    public sealed class SyncAwaiter : SynchronizationContext, IDisposable
    {
        /// <summary>
        /// The queue keeping track of the waiters
        /// </summary>
        private readonly BlockingCollection<(SendOrPostCallback callback, object state)> m_queue = new BlockingCollection<(SendOrPostCallback, object)>();

        /// <summary>
        /// Creates a new waiter
        /// </summary>
        private SyncAwaiter() { }

        /// <summary>
        /// Waits for an operation synchronously
        /// </summary>
        /// <param name="asyncOperation">The operation to wait for</param>
        public static void WaitSync(Func<Task> asyncOperation)
        {
            var prevContext = Current;

            SyncAwaiter sync = null;
            try
            {
                sync = new SyncAwaiter();
                SetSynchronizationContext(sync);

                var awaiter = asyncOperation().GetAwaiter();
                sync.ExecuteCallbacks(awaiter);

                // handle non-success result
                awaiter.GetResult();
            }
            finally
            {
                SetSynchronizationContext(prevContext);
                sync?.Dispose();
            }
        }

        /// <summary>
        /// Invokes the queue waiters
        /// </summary>
        /// <param name="awaiter"></param>
        private void ExecuteCallbacks(TaskAwaiter awaiter)
        {
            while (!awaiter.IsCompleted)
            {
                var item = m_queue.Take();
                item.callback(item.state);
            }
        }

        /// <summary>
        /// Adds a callback to the queue
        /// </summary>
        /// <param name="d">The callback</param>
        /// <param name="state">The state object</param>
        public override void Post(SendOrPostCallback d, object state)
        {
            m_queue.Add((d, state));
        }

        /// <summary>
        /// Adds a callback to the queue, optionally running it synchronously
        /// </summary>
        /// <param name="d">The callback</param>
        /// <param name="state">The state object</param>
        public override void Send(SendOrPostCallback d, object state)
        {
            if (Current == this)
            {
	            // already on execution thread
                d(state);
                return;
            }

            var task = new Task(new Action<object>(d), state);
            m_queue.Add((x => ((Task) x).RunSynchronously(), task));

            task.Wait();
        }

        /// <summary>
        /// Returns the synchronization context
        /// </summary>
        public override SynchronizationContext CreateCopy()
        {
            return this;
        }

        /// <summary>
        /// Disposes this instance
        /// </summary>
        public void Dispose()
        {
            m_queue.Dispose();
        }
    }
}