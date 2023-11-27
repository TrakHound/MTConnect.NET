using System;
using System.Threading.Tasks;

namespace Ceen.Mvc
{
	/// <summary>
	/// Interface for describing the result of an invocation.
	/// </summary>
	public interface IResult
	{
		Task Execute(IHttpContext context);
	}

	/// <summary>
	/// Interface for representing a simple status response without content
	/// </summary>
	public interface IStatusCodeResult : IResult
	{
		/// <summary>
		/// Gets the status code.
		/// </summary>
		HttpStatusCode StatusCode { get; }

		/// <summary>
		/// Gets the status message.
		/// </summary>
		string StatusMessage { get; }
	}

	/// <summary>
	/// Result wrapper for providing a status code result
	/// </summary>
	public struct StatusCodeResult : IStatusCodeResult
	{
		/// <summary>
		/// Gets the status code.
		/// </summary>
		/// <value>The status code.</value>
		public HttpStatusCode StatusCode { get; private set; }

		/// <summary>
		/// Gets the status message.
		/// </summary>
		public string StatusMessage { get; private set; }

		/// <summary>
		/// A flag indicating if caching of the result should be disabled
		/// </summary>
		/// <value></value>
		public bool DisableCaching { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Mvc.StatusCodeResult"/> struct.
		/// </summary>
		/// <param name="code">The status code.</param>
		/// <param name="message">The status message.</param>
		public StatusCodeResult(HttpStatusCode code, string message = null, bool disableCaching = true)
		{
			StatusCode = code;
			StatusMessage = message ?? HttpStatusMessages.DefaultMessage(code);
			DisableCaching = disableCaching;
		}

		/// <summary>
		/// Execute the method with the specified context.
		/// </summary>
		/// <param name="context">The context to use.</param>
		public Task Execute(IHttpContext context)
		{
			if (DisableCaching)
				context.Response.SetNonCacheable();

			context.Response.StatusCode = StatusCode;
			context.Response.StatusMessage = StatusMessage;
			return Task.FromResult(true);
		}		
	}

	/// <summary>
	/// Result wrapper for providing an IResult from a function
	/// </summary>
	internal struct LambdaResult : IResult
	{
		/// <summary>
		/// The function to invoke
		/// </summary>
		private readonly Func<IHttpContext, Task> m_func;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Mvc.LambdaResult"/> struct.
		/// </summary>
		/// <param name="func">The function to invoke.</param>
		public LambdaResult(Func<Task> func)
		{
			m_func = (x) => func();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Mvc.LambdaResult"/> struct.
		/// </summary>
		/// <param name="func">The action to invoke.</param>
		public LambdaResult(Action func)
		{
			m_func = (x) =>
			{
				func();
				return Task.FromResult(true);
			};
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Mvc.LambdaResult"/> struct.
		/// </summary>
		/// <param name="func">The function to invoke.</param>
		public LambdaResult(Func<IHttpContext, Task> func)
		{
			m_func = func;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Mvc.LambdaResult"/> struct.
		/// </summary>
		/// <param name="func">The action to invoke.</param>
		public LambdaResult(Action<IHttpContext> func)
		{
			m_func = (ctx) =>
			{
				func(ctx);
				return Task.FromResult(true);
			};
		}

		/// <summary>
		/// Execute the method with the specified context.
		/// </summary>
		/// <param name="context">The context to use.</param>
		public Task Execute(IHttpContext context)
		{
			return m_func(context);
		}
	}
}
