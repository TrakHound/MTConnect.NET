using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ceen.Httpd.Handler
{
    /// <summary>
    /// Handler module that returns a static response, such as redirect or 404
    /// </summary>
    internal class StaticHandler : IHttpModule
    {
        /// <summary>
        /// Gets or sets the status code reported.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.NotFound;

        /// <summary>
        /// Gets or sets the status message reported.
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// Any extra headers to include in the response, should be in the form "Header-Name: Value"
        /// </summary>
        public string[] ExtraHeaders { get; set; } = new string[] { };

        /// <summary>
        /// Handles the request by sending a redirect
        /// </summary>
        /// <returns>The awaitable task.</returns>
        /// <param name="context">The http context.</param>
        /// <param name="cancellationToken">The token indicating to stop handling.</param>
        public Task<bool> HandleAsync(IHttpContext context, CancellationToken cancellationToken)
        {
            context.Response.StatusCode = StatusCode;
            context.Response.StatusMessage = StatusMessage;
            if (ExtraHeaders != null)
            {
                foreach (var h in ExtraHeaders)
                {
                    var c = h?.Split(new char[] { ':' }, 2);
                    if (c == null || c.Length != 2 || string.IsNullOrWhiteSpace(c[0]))
                        continue;

                    context.Response.Headers[c[0]] = c[1];
                }
            }
            return Task.FromResult(true);
        }
    }
}
