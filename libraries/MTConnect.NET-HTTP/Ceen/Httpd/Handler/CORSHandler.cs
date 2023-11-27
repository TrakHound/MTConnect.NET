using System;
using System.Linq;
using System.Threading.Tasks;

using Ceen;

namespace Ceen.Httpd.Handler
{
    public class CORSHandler : IHttpModuleWithSetup
    {
        /// <summary>
        /// The allowed origins, use a wildcard (*) to allow any source
        /// </summary>
        public string[] AllowedOrigins { get; set; } = new string[] { "*" };
        /// <summary>
        /// The list of methods reported in the Access-Control-Allow-Methods response header
        /// </summary>
        public string[] AllowedMethods { get; set; } = new string[] { "GET" };
        /// <summary>
        /// The list of methods reported in the Access-Control-Allow-Methods response header
        /// </summary>
        public string[] AllowedHeaders { get; set; } = new string[] { "Content-Type" };
        /// <summary>
        /// Any extra headers to include in the response, should be in the form "Access-Control-Name: Value"
        /// </summary>
        public string[] ExtraHeaders { get; set; } = new string[] { };

        /// <summary>
        /// The regex for matching an origin
        /// </summary>
        private System.Text.RegularExpressions.Regex m_originMatcher;

        /// <summary>
        /// Cleans up the input parameters and builds the regular expression for matching the origin
        /// </summary>
        public void AfterConfigure()
        {
            if (AllowedOrigins == null)
                AllowedOrigins = new string[0];
            if (AllowedMethods == null)
                AllowedMethods = new string[0];
            if (AllowedHeaders == null)
                AllowedHeaders = new string[0];
            if (ExtraHeaders == null)
                ExtraHeaders = new string[0];

            if (Array.IndexOf(AllowedOrigins, "*") >= 0)
                AllowedOrigins = new string[] { "*" };

            m_originMatcher = 
                new System.Text.RegularExpressions.Regex(
                    string.Join("|",
                    AllowedOrigins
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => "(" + x.Replace("*", ".*").Replace("?", ".") + ")")
                    )
                );
        }

        /// <summary>
        /// Handles OPTIONS requests
        /// </summary>
        /// <param name="context">The request context</param>
        /// <returns>A value indicating if the request was handled</returns>
        public Task<bool> HandleAsync(IHttpContext context)
        {
            var preflight = context.Request.Method == "OPTIONS";
            var origin = context.Request.Headers["Origin"];
            if (string.IsNullOrWhiteSpace(origin) || m_originMatcher.Match(origin).Length != origin.Length)
            {
                context.Response.StatusCode = Ceen.HttpStatusCode.NotAcceptable;
                return Task.FromResult(preflight);   
            }

            context.Response.Headers.Add("Access-Control-Allow-Origin", origin);
            context.Response.Headers.Add("Access-Control-Allow-Headers", string.Join(",", AllowedHeaders));
            context.Response.Headers.Add("Access-Control-Allow-Methods", string.Join(",", AllowedMethods));
            foreach (var h in ExtraHeaders)
            {
                var c = h?.Split(new char[] { ':' }, 2);
                if (c == null || c.Length != 2 || string.IsNullOrWhiteSpace(c[0]))
                    continue;

                context.Response.Headers[c[0]] = c[1];
            }
            context.Response.StatusCode = Ceen.HttpStatusCode.OK;

            return Task.FromResult(preflight);
        }
    }
}
