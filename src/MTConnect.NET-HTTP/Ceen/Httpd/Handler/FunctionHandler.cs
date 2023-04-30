using System;
using System.Threading.Tasks;

namespace Ceen.Httpd.Handler
{
	/// <summary>
	/// Implementation of a handler that operates on a lambda method or other delegate
	/// </summary>
	public class FunctionHandler : IHttpModule
	{
		/// <summary>
		/// The actual handler
		/// </summary>
		private readonly HttpHandlerDelegate m_handler;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Httpd.Handler.FunctionHandler"/> class.
		/// </summary>
		/// <param name="handler">The handler to invoke.</param>
		public FunctionHandler(HttpHandlerDelegate handler)
		{
			m_handler = handler;			
		}

		#region IHttpModule implementation

		/// <summary>
		/// Handles the operation asynchronously.
		/// </summary>
		/// <returns>The awaitable task.</returns>
		/// <param name="context">The request context.</param>
		public Task<bool> HandleAsync(IHttpContext context)
		{
			return m_handler(context);
		}

		#endregion

		/// <summary>
		/// Implicit conversion from a delegate or lambda to a function handler
		/// </summary>
		/// <returns>The handler instance.</returns>
		/// <param name="handler">The delegate to invoke.</param>
		public static implicit operator FunctionHandler(HttpHandlerDelegate handler)
		{
			return new FunctionHandler(handler);
		}
	}
}

