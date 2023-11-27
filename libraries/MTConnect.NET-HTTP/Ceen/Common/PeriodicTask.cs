using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ceen
{
	/// <summary>
	/// Helper class that can run a task periodically
	/// or when signalled explicitly
	/// </summary>
	public class PeriodicTask
	{
		/// <summary>
		/// Gets or sets the expiration check interval.
		/// </summary>
		public TimeSpan Interval { get; set; }

		/// <summary>
		/// A task to signal checking for expiration
		/// </summary>
		private TaskCompletionSource<bool> m_check = new TaskCompletionSource<bool>();

		/// <summary>
		/// A token to signal stopping the check for expiration
		/// </summary>
		private CancellationTokenSource m_stop_token = new CancellationTokenSource();

		/// <summary>
		/// The task signaling expiration completion
		/// </summary>
		private readonly Task m_task;

		/// <summary>
		/// The task to run
		/// </summary>
		private readonly Func<bool, Task> m_handler;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.PeriodTask"/> class.
		/// </summary>
		/// <param name="handler">The method to invoke, the argument indicates if this was a requested run or a periodic invocation.</param>
		/// <param name="interval">The interval used.</param>
		public PeriodicTask(Func<bool, Task> handler, TimeSpan interval)
		{
			m_handler = handler;
			Interval = interval;

			m_task = Task.Run(async () => await RunLoopAsync());
		}

		/// <summary>
		/// Runs an expire loop
		/// </summary>
		/// <returns>The loop.</returns>
		private async Task RunLoopAsync()
		{
			while (true)
			{
				var requested = await Task.WhenAny(Task.Delay(Interval), m_check.Task) == m_check.Task;
				m_check = new TaskCompletionSource<bool>();

				if (m_stop_token.IsCancellationRequested)
					return;

				await m_handler(requested);
			}
		}

		/// <summary>
		/// Triggers an expire now
		/// </summary>
		public void RunNow()
		{
			m_check.TrySetResult(true);
		}

		/// <summary>
		/// Stop this instance.
		/// </summary>
		public Task StopAsync()
		{
			m_stop_token.Cancel();
			m_check.TrySetResult(true);
			return m_task;
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:Ceen.PeriodTask"/> is stopped.
		/// </summary>
		/// <value><c>true</c> if is stopped; otherwise, <c>false</c>.</value>
		public bool IsStopped
		{
			get { return m_task == null || m_task.IsFaulted || m_task.IsCanceled || m_task.IsCompleted; }
		}

		/// <summary>
		/// Releases all resource used by the <see cref="T:Ceen.PeriodTask"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="T:Ceen.PeriodTask"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="T:Ceen.PeriodTask"/> in an unusable state. After calling
		/// <see cref="Dispose"/>, you must release all references to the <see cref="T:Ceen.PeriodTask"/> so the garbage
		/// collector can reclaim the memory that the <see cref="T:Ceen.PeriodTask"/> was occupying.</remarks>
		public void Dispose()
		{
			StopAsync();
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the <see cref="T:Ceen.PeriodTask"/> is
		/// reclaimed by garbage collection.
		/// </summary>
		~PeriodicTask()
		{
			StopAsync();
			GC.SuppressFinalize(this);
		}

	}
}
