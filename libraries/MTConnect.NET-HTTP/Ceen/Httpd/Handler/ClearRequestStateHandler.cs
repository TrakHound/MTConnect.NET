using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ceen.Httpd.Handler
{
    /// <summary>
    /// A handler that clears the request state
    /// </summary>
    internal class ClearRequestStateHandler : IHttpModule
	{
        /// <summary>
        /// Handles the request.
        /// </summary>
        /// <returns>The awaitable task.</returns>
        /// <param name="context">The http context.</param>
        /// <param name="cancellationToken">The token indicating to stop handling.</param>
        public Task<bool> HandleAsync(IHttpContext context, CancellationToken cancellationToken)
		{
			context.Request.RequestState.Clear();
			return Task.FromResult(false);
		}
	}
}
