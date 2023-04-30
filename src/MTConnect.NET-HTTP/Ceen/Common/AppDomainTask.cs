using System;
using System.Threading.Tasks;

namespace Ceen
{
	/// <summary>
	/// Implementation of a Task that works across app domains
	/// </summary>
	public class AppDomainTask : MarshalByRefObject
	{
		/// <summary>
		/// A task completion source
		/// </summary>
		private TaskCompletionSource<object> m_tcs = new TaskCompletionSource<object>();

		/// <summary>
		/// Marks the operation complete
		/// </summary>
		/// <param name="data">The object to return.</param>
		public void SetComplete(object data)
		{
			m_tcs.TrySetResult(data);
		}

		/// <summary>
		/// Marks the operation cancelled
		/// </summary>
		public void SetCancelled()
		{
			m_tcs.TrySetCanceled();
		}

		/// <summary>
		/// Sets the operation failed.
		/// </summary>
		/// <param name="ex">The exception, must be serializable or marshalbyref.</param>
		public void SetFailed(Exception ex)
		{
			m_tcs.TrySetException(ex);
		}

		/// <summary>
		/// Gets the result async
		/// </summary>
		/// <returns>The awaitable result.</returns>
		public Task<object> ResultAsync()
		{
			return m_tcs.Task;
		}

		/// <summary>
		/// Handles a task by invoking the AppDomainTask after completion
		/// </summary>
		/// <param name="sourcetask">The running task.</param>
		/// <param name="handler">The handler for the result.</param>
		/// <typeparam name="T">The data type parameter.</typeparam>
		public static void HandleTask<T>(Task<T> sourcetask, AppDomainTask handler)
		{
			sourcetask.ContinueWith(task =>
			{
				if (task.IsCanceled)
					handler.SetCancelled();
				else if (task.IsFaulted)
				{
					if (task.Exception == null)
						handler.SetFailed(new Exception());
					else if (task.Exception.GetType().IsSerializable)
						handler.SetFailed(task.Exception);
					else
						handler.SetFailed(new Exception(task.Exception.Message));
				}
				else
					handler.SetComplete(task.Result);
			});
		}
	}
}
