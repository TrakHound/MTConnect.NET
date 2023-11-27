using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace Ceen
{
	// Implementation based on: http://blogs.msdn.com/b/pfxteam/archive/2012/02/12/10266988.aspx

	// Added support for cancellation tokens

	/// <summary>
	/// Implementation of a Semaphore that is usable with await statements
	/// </summary>
	public class AsyncSemaphore
	{
		/// <summary>
		/// A task signaling completion
		/// </summary>
		private readonly static Task m_completed = Task.FromResult(true);
		/// <summary>
		/// The list of waiters
		/// </summary>
		private readonly Queue<KeyValuePair<TaskCompletionSource<bool>, IDisposable>> m_waiters = new Queue<KeyValuePair<TaskCompletionSource<bool>, IDisposable>>();
		/// <summary>
		/// The additional number of release calls
		/// </summary>
		private int m_currentCount;

		/// <summary>
		/// Initializes a new instance of the <see cref="Ceen.AsyncSemaphore"/> class.
		/// </summary>
		/// <param name="initialCount">The number of callers to allow before blocking.</param>
		public AsyncSemaphore(int initialCount)
		{
			if (initialCount < 0)
				throw new ArgumentOutOfRangeException(nameof(initialCount));

			m_currentCount = initialCount;
		}

		/// <summary>
		/// Waits for the semaphire to be released
		/// </summary>
		/// <param name="token">The cancellation token</param>
		/// <returns>The awaitable task.</returns>
		public Task WaitAsync(CancellationToken token = default(CancellationToken))
		{
			if (token.IsCancellationRequested)
			{
				var tcs = new TaskCompletionSource<bool>();
				tcs.SetCanceled();
				return tcs.Task;
			}

			lock (m_waiters)
			{
				if (m_currentCount > 0)
				{
					--m_currentCount;
					return m_completed;
				}
				else
				{
					var waiter = new TaskCompletionSource<bool>();

					// Make sure the waiter returns asap on cancellation
					var registrar =
						token.CanBeCanceled
							 ? token.Register(() => waiter.TrySetCanceled())
							 : (IDisposable)null;

					m_waiters.Enqueue(new KeyValuePair<TaskCompletionSource<bool>, IDisposable>(waiter, registrar));
					return waiter.Task;
				}
			}
		}

		/// <summary>
		/// Releases the semaphore to a new 
		/// </summary>
		public void Release()
		{
			TaskCompletionSource<bool> result = null;
			lock (m_waiters)
			{
				while (m_waiters.Count > 0)
				{
					var res = m_waiters.Dequeue();

					// Clear the cancellation event to avoid races
					if (res.Value != null)
						res.Value.Dispose();

					if (!res.Key.Task.IsCanceled && !res.Key.Task.IsFaulted)
					{
						result = res.Key;
						break;
					}
				}

				if (result == null)
					++m_currentCount;
			}
			if (result != null)
				Task.Run(() => result.SetResult(true));
		}
	}

	/// <summary>
	/// Implementation of a lock construct that can be used with await statements,
	/// note that this lock is not re-entrant like the regular monitors used
	/// with the lock statement.
	/// </summary>
	public class AsyncLock
	{
		/// <summary>
		/// The semaphore that provides the general functionality of this lock
		/// </summary>
		private readonly AsyncSemaphore m_semaphore;

		/// <summary>
		/// The task used to encapsulate the lock
		/// </summary>
		private readonly Task<Releaser> m_releaser;

		/// <summary>
		/// Initializes a new instance of the <see cref="Ceen.AsyncLock"/> class.
		/// </summary>
		public AsyncLock()
		{
			m_semaphore = new AsyncSemaphore(1);
			m_releaser = Task.FromResult(new Releaser(this));
		}

        /// <summary>
        /// Runs an action with the lock active
        /// </summary>
        /// <returns>The lock async.</returns>
        /// <param name="action">The action to execute.</param>
        /// <param name="token">The cancellation token.</param>
        public async Task LockedAsync(Action action, CancellationToken token = default(CancellationToken))
        {
            using (await LockAsync(token))
                action();
        }

		/// <summary>
		/// Runs an action with the lock active
		/// </summary>
		/// <returns>The lock async.</returns>
		/// <param name="action">The action to execute.</param>
		/// <param name="token">The cancellation token.</param>
		public async Task LockedAsync(Func<Task> action, CancellationToken token = default(CancellationToken))
		{
			using (await LockAsync(token))
				await action();
		}

		/// <summary>
		/// Runs an action with the lock active
		/// </summary>
		/// <returns>The lock async.</returns>
		/// <param name="action">The action to execute.</param>
		/// <param name="token">The cancellation token.</param>
        /// <typeparam name="T">The type of the return data</typeparam>
		public async Task<T> LockedAsync<T>(Func<Task<T>> action, CancellationToken token = default(CancellationToken))
		{
			using (await LockAsync(token))
				return await action();
		}

        /// <summary>
        /// Runs an action with the lock active
        /// </summary>
        /// <returns>The lock async.</returns>
        /// <param name="action">The action to execute.</param>
        /// <param name="token">The cancellation token.</param>
        /// <typeparam name="T">The type of the return data</typeparam>
        public async Task<T> LockedAsync<T>(Func<T> action, CancellationToken token = default(CancellationToken))
        {
            using (await LockAsync(token))
                return action();
        }

        /// <summary>
        /// Aquires the exclusive lock, and awaits until it is available
        /// </summary>
        /// <param name="token">The cancellation token</param>
        /// <returns>The async.</returns>
        public Task<Releaser> LockAsync(CancellationToken token = default(CancellationToken))
		{
			var wait = m_semaphore.WaitAsync(token);

			return wait.IsCompleted ?
				m_releaser :
				wait.ContinueWith(
						   (_, state) => new Releaser((AsyncLock)state),
						   this,

						   // The token is signalling the wait task, so we do not cancel here
						   CancellationToken.None,

						   // Only if we actually have the lock, should we return the releaser
						   TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
						   TaskScheduler.Default);
		}

		/// <summary>
		/// Internal releaser construct which needs to be disposed to unlock
		/// </summary>
		public struct Releaser : IDisposable
		{
			/// <summary>
			/// The parent instance
			/// </summary>
			private readonly AsyncLock m_parent;

			/// <summary>
			/// A value indicating if the lock can be disposed
			/// </summary>
			private bool m_canDispose;

			/// <summary>
			/// Initializes a new instance of the <see cref="Ceen.AsyncLock.Releaser"/> struct.
			/// </summary>
			/// <param name="parent">The parent lock.</param>
			internal Releaser(AsyncLock parent)
			{
				m_parent = parent;
				m_canDispose = true;
			}

			/// <summary>
			/// Releases all resource used by the <see cref="Ceen.AsyncLock.Releaser"/> object.
			/// </summary>
			/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Ceen.AsyncLock.Releaser"/>. The
			/// <see cref="Dispose"/> method leaves the <see cref="Ceen.AsyncLock.Releaser"/> in an unusable state. After
			/// calling <see cref="Dispose"/>, you must release all references to the <see cref="Ceen.AsyncLock.Releaser"/> so
			/// the garbage collector can reclaim the memory that the <see cref="Ceen.AsyncLock.Releaser"/> was occupying.</remarks>
			public void Dispose()
			{
				if (m_parent != null)
					lock (m_parent)
						if (m_canDispose)
						{
							m_parent.m_semaphore.Release();
							m_canDispose = false;
						}
			}
		}
	}
}
