// Copyright (c) 2026 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Ceen;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Http;
using MTConnect.Servers.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTConnect.Servers
{
    /// <summary>
    /// Ceen request handler that serves static files configured via the
    /// agent's HTTP server FileConfiguration entries (each entry maps a
    /// URL path prefix to a directory or file on disk). The handler
    /// resolves the requested path, applies the configured caching
    /// policy, and streams the file bytes back with the appropriate
    /// Content-Type. The endpoint surface is whatever path prefixes the
    /// host configuration registers; there is no fixed route template.
    /// </summary>
    class MTConnectStaticResponseHandler : MTConnectHttpResponseHandler
    {
        public Func<MTConnectStaticFileRequest, Stream> ProcessFunction { get; set; }


        public MTConnectStaticResponseHandler(IHttpServerConfiguration serverConfiguration, IMTConnectAgentBroker mtconnectAgent) : base(serverConfiguration, mtconnectAgent) { }


        protected async override Task<MTConnectHttpResponse> OnRequestReceived(IHttpContext context, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var response = new MTConnectHttpResponse();

            var httpRequest = context.Request;
            var httpResponse = context.Response;

            if (httpRequest != null && httpResponse != null)
            {
                try
                {
                    var statusCode = 404;
                    var contentType = "text/plain";
                    Stream fileContents = null;

                    var localPath = ResolveLocalPath(httpRequest.Path);
                    if (localPath != null)
                    {
                        contentType = MimeTypes.GetMimeType(Path.GetFileName(httpRequest.Path.Trim('/')));

                        // Read MTConnectVersion from Query string
                        var versionString = httpRequest.QueryString["version"];
                        Version.TryParse(versionString, out var version);
                        if (version == null) version = _mtconnectAgent.Configuration.DefaultVersion;

                        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, localPath);

                        if (File.Exists(filePath))
                        {
                            // Check ProcessFunction method
                            if (ProcessFunction != null)
                            {
                                var staticFileRequest = new MTConnectStaticFileRequest();
                                staticFileRequest.HttpRequest = httpRequest;
                                staticFileRequest.FilePath = filePath;
                                staticFileRequest.LocalPath = localPath;
                                staticFileRequest.Version = version;

                                fileContents = ProcessFunction(staticFileRequest);
                            }

                            // If nothing found in the overridden method, then read directly from filePath
                            // (disposed by the base handler once the response has been written)
                            if (fileContents == null)
                            {
                                fileContents = File.OpenRead(filePath);
                            }

                            statusCode = fileContents != null ? 200 : 500;
                            contentType = MimeTypes.GetMimeType(Path.GetExtension(filePath));
                        }

                        // Set HTTP Response Status Code
                        response.StatusCode = statusCode;
                        response.ContentType = contentType;
                        response.Content = fileContents;
                    }
                }
                catch { }
            }

            return response;
        }

        internal bool IsValidFile(string path)
        {
            try
            {
                var localPath = ResolveLocalPath(path);
                if (localPath != null)
                {
                    var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, localPath);
                    return File.Exists(filePath);
                }
            }
            catch { }

            return false;
        }

        // Maps a request path to the configured local file path, or null when no FileConfiguration matches.
        // A configuration matches either the file name itself or the request's directory exactly, and only the
        // file name (never a directory part) is appended to a configured directory, so ".." cannot escape it.
        private string ResolveLocalPath(string path)
        {
            if (string.IsNullOrEmpty(path) || _serverConfiguration.Files.IsNullOrEmpty()) return null;

            var requestedPath = path.Trim('/');

            // Get File extension to prevent certain files (.exe, .dll, etc) from being accessed
            // that could cause security concerns
            var pathExtension = Path.GetExtension(requestedPath);
            if (pathExtension == ".exe" || pathExtension == ".dll") return null;

            var resource = Path.GetFileName(requestedPath);

            var relativePath = Path.GetDirectoryName(requestedPath);
            if (!string.IsNullOrEmpty(relativePath)) relativePath = relativePath.Replace('\\', '/');
            else relativePath = "/"; // Assume Root

            // Find a FileConfiguration whose Location matches the requested Resource
            var fileConfiguration = _serverConfiguration.Files.FirstOrDefault(o => o.Location == resource);
            if (fileConfiguration != null)
            {
                // Rewrite the localPath to the one that is configured that matches the 'Location' property
                return NormalizeSeparators(fileConfiguration.Path);
            }

            // Find a FileConfiguration whose Location matches the requested Directory
            fileConfiguration = _serverConfiguration.Files.FirstOrDefault(o => o.Location == relativePath);
            if (fileConfiguration != null)
            {
                return NormalizeSeparators(Path.Combine(fileConfiguration.Path, resource));
            }

            return null;
        }

        // Configured paths may use either separator; File APIs on Linux / macOS only understand '/'
        private static string NormalizeSeparators(string path)
        {
            if (path == null) return null;
            return path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
        }
    }
}
