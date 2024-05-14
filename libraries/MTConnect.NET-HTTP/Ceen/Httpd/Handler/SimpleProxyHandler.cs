using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Ceen;

namespace Ceen.Httpd.Handler
{
    /// <summary>
    /// A basic proxy handler that simply forwards all data from another http server
    /// </summary>
    internal class SimpleProxyHandler : IHttpModule
    {
        /// <summary>
        /// The url we proxy requests for, should not end with a forward slash
        /// </summary>
        public string HostUrl { get; set; }

        /// <summary>
        /// A prefix to remove from the source url.
        /// If this is set, only requests matching the prefix
        /// are proxied
        /// </summary>
        public string LeftTrimSourceUrl { get; set; }

        /// <summary>
        /// Helper method to rewrite the url
        /// </summary>
        /// <param name="context">The context with the request to rewrite</param>
        /// <returns>The url for the proxy instance, or null if the request should not be proxied</returns>
        protected string RewriteUrl(IHttpContext context)
        {
            var path = context.Request.Path;
            if (!string.IsNullOrWhiteSpace(LeftTrimSourceUrl))
            {
                if (!path.StartsWith(LeftTrimSourceUrl))
                    return null;

                path = path.Substring(LeftTrimSourceUrl.Length);
            }

            return HostUrl + path + context.Request.RawQueryString;
        }

        /// <summary>
        /// Handles a proxy request
        /// </summary>
        /// <param name="context">The request content</param>
        /// <returns><c>true</c> if the request was handled; <c>false</c> otherwise</returns>
        public async Task<bool> HandleAsync(IHttpContext context)
        {
            var targeturl = RewriteUrl(context);
            if (targeturl == null)
                return false;

            var wr = System.Net.WebRequest.CreateHttp(targeturl);
            wr.Headers.Clear();
            foreach (var item in context.Request.Headers)
                wr.Headers.Add(item.Key, item.Value);

            wr.Method = context.Request.Method;
            if (context.Request.ContentLength > 0)
                using(var rs = await wr.GetRequestStreamAsync())
                    await context.Request.Body.CopyToAsync(rs);

            using(var res = await GetResponseWithoutExceptionAsync(wr))
            {
                foreach (var key in res.Headers.AllKeys)
                    context.Response.Headers[key] = res.Headers[key];
                context.Response.StatusCode = (Ceen.HttpStatusCode)(int)res.StatusCode;
                context.Response.StatusMessage = res.StatusDescription;

                // The .Net http client has already removed the chunked markers
                // so the stream contains only data
                if (string.Equals(context.Response.Headers["Transfer-Encoding"], "chunked", StringComparison.OrdinalIgnoreCase))
                    context.Response.Headers.Remove("Transfer-Encoding");

                await context.Response.FlushHeadersAsync();
                using (var r = context.Response.GetResponseStream())
                using(var rr = res.GetResponseStream())

#if NET5_0_OR_GREATER
                    await rr.CopyToAsync(r, context.Request.TimeoutCancellationToken);
#else
                    await rr.CopyToAsync(r);
#endif
            }

            return true;
        }

        /// <summary>
        /// Helper method to work around annoying exceptions thrown by System.Net.WebRequest
        /// when the server returns a non-success status code
        /// </summary>
        /// <param name="req">The request to get the response from</param>
        /// <returns>The web response</returns>
        private static async Task<System.Net.HttpWebResponse> GetResponseWithoutExceptionAsync(System.Net.HttpWebRequest req)
        {
            try
            {
                return (System.Net.HttpWebResponse)await req.GetResponseAsync();
            }
            catch (System.Net.WebException we)
            {
                var resp = we.Response as System.Net.HttpWebResponse;
                if (resp == null)
                    throw;
                return resp;
            }
        }
    }
}