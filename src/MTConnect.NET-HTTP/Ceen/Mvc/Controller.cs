using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text.Json;

namespace Ceen.Mvc
{
	/// <summary>
	/// A marker interface for setting routes on a controller.
	/// Use this interface and place a RouteAttribute on it
	/// </summary>
	public interface IControllerPrefix
	{
	}

	/// <summary>
	/// Interface for allowing a controller to reconfigure the static parts
	/// from attributes to runtime values
	/// </summary>
    public interface IIDynamicConfiguredController
    {
		/// <summary>
		/// Change a statically parsed route at runtime
		/// </summary>
		/// <param name="x">The route to change</param>
		/// <returns>The patched route</returns>
        PartialParsedRoute PatchRoute(PartialParsedRoute x);
    }

    /// <summary>
    /// Implementation of support methods in a controller
    /// </summary>
    public abstract class Controller
	{
		/// <summary>
		/// Returns the result as JSON encoded with UTF-8
		/// </summary>
		/// <param name="data">The data object to serialize.</param>
		/// <param name="disablecaching">Set to <c>true</c> to emit non-cacheable headers.</param>
		protected IResult Json(object data, bool disablecaching = true)
		{
			return new LambdaResult(ctx =>
			{
				if (disablecaching)
					ctx.Response.SetNonCacheable();
				return ctx.Response.WriteAllJsonAsync(JsonSerializer.Serialize(data));
			});
		}
		/// <summary>
		/// Returns a text string as the result
		/// </summary>
		/// <param name="data">The string to return.</param>
		/// <param name="encoding">The encoding to use, defaults to UTF-8.</param>
		/// <param name="contenttype">The content type to use, defaults to &quot;text/plain&quot;.</param>
		/// <param name="disablecaching">Set to <c>true</c> to emit non-cacheable headers.</param>
		protected IResult Text(string data, System.Text.Encoding encoding = null, string contenttype = "text/plain", bool disablecaching = true)
		{
			encoding = encoding ?? System.Text.Encoding.UTF8;

			return new LambdaResult(ctx =>
			{
				if (disablecaching)
					ctx.Response.SetNonCacheable();

				return ctx.Response.WriteAllAsync(data, encoding, string.Format("{0}; charset={1}", contenttype, encoding.BodyName));
			});
		}

		/// <summary>
		/// Returns a html document as the result
		/// </summary>
		/// <param name="data">The html to return.</param>
		/// <param name="encoding">The encoding to use, defaults to UTF-8.</param>
		/// <param name="disablecaching">Set to <c>true</c> to emit non-cacheable headers.</param>
		protected IResult Html(string data, System.Text.Encoding encoding = null, bool disablecaching = true)
		{
			return Text(data, encoding, "text/html", disablecaching);
		}

		/// <summary>
		/// Sets the status for the request
		/// </summary>
		/// <param name="code">The status code.</param>
		/// <param name="message">An optional status message.</param>
		protected IResult Status(HttpStatusCode code, string message = null, bool disablecaching = true)
			=> new StatusCodeResult(code, message, disablecaching);

		/// <summary>
		/// Sets the status for the request
		/// </summary>
		/// <param name="code">The status code.</param>
		/// <param name="message">An optional status message.</param>
		protected IResult Status(IStatusCodeResult code, string message = null, bool disablecaching = true)
			=> new StatusCodeResult(code.StatusCode, message ?? code.StatusMessage, disablecaching);

		/// <summary>
		/// Sends a &quot;400 - Bad request&quot; response
		/// </summary>
		protected static readonly IStatusCodeResult BadRequest = new StatusCodeResult(HttpStatusCode.BadRequest);

		/// <summary>
		/// Sends a &quot;404 - Not found&quot; response
		/// </summary>
		protected static readonly IStatusCodeResult NotFound = new StatusCodeResult(HttpStatusCode.NotFound);

		/// <summary>
		/// Sends a &quot;200 - OK&quot; response
		/// </summary>
		protected static readonly IStatusCodeResult OK = new StatusCodeResult(HttpStatusCode.OK);

		/// <summary>
		/// Sends a &quot;403 - Forbidden&quot; response
		/// </summary>
		protected static readonly IStatusCodeResult Forbidden = new StatusCodeResult(HttpStatusCode.Forbidden);

		/// <summary>
		/// Sends a &quot;204 - No Content&quot; response
		/// </summary>
		protected static readonly IStatusCodeResult NoContent = new StatusCodeResult(HttpStatusCode.NoContent);

		/// <summary>
		/// Sends a &quot;302 - redirect&quot; to the client
		/// </summary>
		/// <param name="url">URL.</param>
		protected IResult Redirect(string url)
		{
			return new LambdaResult(ctx =>
			{
				ctx.Response.Redirect(url);
			});
		}

		/// <summary>
		/// Wraps a result method
		/// </summary>
		/// <param name="func">The function to use.</param>
		protected IResult Result(Func<Task> func)
		{
			return new LambdaResult(func);
		}

		/// <summary>
		/// Wraps a result method
		/// </summary>
		/// <param name="func">The function to use.</param>
		protected IResult Result(Action func)
		{
			return new LambdaResult(func);
		}
	}
}
