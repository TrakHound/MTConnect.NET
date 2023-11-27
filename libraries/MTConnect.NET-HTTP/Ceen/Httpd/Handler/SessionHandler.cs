using System;
using System.Threading.Tasks;

namespace Ceen.Httpd.Handler
{
	public class SessionHandler : IHttpModule
	{
		/// <summary>
		/// The name of the storage module
		/// </summary>
		public const string STORAGE_MODULE_NAME = "session-storage";

		/// <summary>
		/// Gets or sets the name of the cookie with the token.
		/// </summary>
		public string CookieName { get; set; } = "ceen-session-token";

		/// <summary>
		/// Gets or sets the number of seconds a session is valid.
		/// </summary>
		public TimeSpan ExpirationSeconds { get; set; } = TimeSpan.FromMinutes(30);

		/// <summary>
		/// Gets or sets a value indicating if the session cookie gets the &quot;secure&quot; option set,
		/// meaning that it will only be sent over HTTPS
		/// </summary>
		public bool SessionCookieSecure { get; set; } = false;

        /// <summary>
        /// Gets or sets the value for the cookie &quot;samesite&quot; attribute.
        /// The default is &quot;Strict&quot; meaning that the cookie will not be shared with other sites.
        /// </summary>
        public string SessionCookieSameSite { get; set; } = "Strict";

		/// <summary>
		/// Handles the request
		/// </summary>
		/// <returns>The awaitable task.</returns>
		/// <param name="context">The requests context.</param>
		public async Task<bool> HandleAsync(IHttpContext context)
		{
			if (context.Session != null)
				return false;
			
			var sessiontoken = context.Request.SessionID = context.Request.Cookies[CookieName];

			if (!string.IsNullOrWhiteSpace(sessiontoken))
			{
				// If the session exists, hook it up
				context.Session = await context.Storage.GetStorageAsync(STORAGE_MODULE_NAME, sessiontoken, (int)ExpirationSeconds.TotalSeconds, false);
				if (context.Session != null)
					return false;
			}

			// Create new storage
			sessiontoken = context.Request.SessionID = Guid.NewGuid().ToString();
			context.Response.AddCookie(CookieName, sessiontoken, secure: SessionCookieSecure, httponly: true, samesite: SessionCookieSameSite);
			context.Session = await context.Storage.GetStorageAsync(STORAGE_MODULE_NAME, sessiontoken, (int)ExpirationSeconds.TotalSeconds, true);

			return false;
		}
	}
}
