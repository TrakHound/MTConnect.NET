using System;
using System.Threading.Tasks;

namespace Ceen.Httpd.Handler
{
	/// <summary>
	/// Simple handler that performs a redirect
	/// </summary>
	public class RedirectHandler : IHttpModule
	{
		/// <summary>
		/// Gets or sets the status code used to report the redirect.
		/// </summary>
		public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.MovedPermanently;

		/// <summary>
		/// Gets or sets the status message used to report the redirect.
		/// </summary>
		public string StatusMessage { get; set; }

		/// <summary>
		/// Gets or sets a value indicating if the redirect is performed internally
		/// </summary>
		public bool InternalRedirect { get; set; } = false;

		/// <summary>
		/// Gets or sets the redirection target value
		/// </summary>
		public string RedirectTarget { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Httpd.Handler.RedirectHandler"/> class.
		/// </summary>
		public RedirectHandler() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Httpd.Handler.RedirectHandler"/> class.
		/// </summary>
		/// <param name="target">The redirect target url.</param>
		public RedirectHandler(string target) { RedirectTarget = target; }

		/// <summary>
		/// Handles the request by sending a redirect
		/// </summary>
		/// <returns>The awaitable task.</returns>
		/// <param name="context">The http context.</param>
		public Task<bool> HandleAsync(IHttpContext context)
		{
			if (InternalRedirect)
			{
				context.Response.InternalRedirect(RedirectTarget);
			}
			else
			{
				context.Response.StatusCode = StatusCode;
				context.Response.StatusMessage = StatusMessage;
				context.Response.AddHeader("Location", RedirectTarget);
			}

			return Task.FromResult(true);
		}
	}
}
