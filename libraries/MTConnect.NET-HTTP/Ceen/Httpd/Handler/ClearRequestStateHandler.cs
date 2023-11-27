using System;
using System.Threading.Tasks;

namespace Ceen.Httpd.Handler
{
	/// <summary>
	/// A handler that clears the request state
	/// </summary>
	public class ClearRequestStateHandler : IHttpModule
	{
		/// <summary>
		/// Handles the request.
		/// </summary>
		/// <returns>The awaitable task.</returns>
		/// <param name="context">The http context.</param>
		public Task<bool> HandleAsync(IHttpContext context)
		{
			context.Request.RequestState.Clear();
			return Task.FromResult(false);
		}
	}
}
