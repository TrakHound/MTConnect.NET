// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Ceen;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Http;
using MTConnect.Servers.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MTConnect.Servers
{
    class MTConnectStaticResponseHandler : MTConnectHttpResponseHandler
    {
        public Func<MTConnectStaticFileRequest, byte[]> ProcessFunction { get; set; }


        public MTConnectStaticResponseHandler(IHttpAgentConfiguration agentConfiguration, IMTConnectAgentBroker mtconnectAgent, IHttpServerConfiguration serverConfiguration)
            : base(agentConfiguration, mtconnectAgent, serverConfiguration) { }


        protected async override Task<MTConnectHttpResponse> OnRequestReceived(IHttpContext context)
        {
            var response = new MTConnectHttpResponse();

            var httpRequest = context.Request;
            var httpResponse = context.Response;

            if (httpRequest != null && httpResponse != null)
            {
                try
                {
                    var statusCode = 404;
                    var contentType = "text/plain";
                    byte[] fileContents = null;
                    var requestedPath = httpRequest.Path.Trim('/');
                    var localPath = httpRequest.Path.TrimEnd('/');

                    // Get File extension to prevent certain files (.exe, .dll, etc) from being accessed
                    // that could cause security concerns
                    var pathExtension = Path.GetExtension(requestedPath);
                    var valid = pathExtension != ".exe" && pathExtension != ".dll";

                    if (valid)
                    {
                        valid = false;

                        // Check to see if the path matches one that is configured
                        if (!_agentConfiguration.Files.IsNullOrEmpty())
                        {
                            var resource = Path.GetFileName(requestedPath);
                            contentType = MimeTypes.GetMimeType(resource);

                            var relativePath = Path.GetDirectoryName(requestedPath);
                            if (!string.IsNullOrEmpty(relativePath)) relativePath = relativePath.Replace('\\', '/');
                            else relativePath = resource;

                            if (!string.IsNullOrEmpty(relativePath))
                            {
                                // Find a FileConfiguration whose Location matches the requested Resource
                                var fileConfiguration = _agentConfiguration.Files.FirstOrDefault(o => o.Location == resource);
                                if (fileConfiguration != null)
                                {
                                    // Rewrite the localPath to the one that is configured that matches the 'Location' property
                                    localPath = fileConfiguration.Path;
                                    valid = true;
                                }
                                else
                                {
                                    // Find a FileConfiguration whose Location matches the requested Resource
                                    fileConfiguration = _agentConfiguration.Files.FirstOrDefault(o => o.Location == relativePath);
                                    if (fileConfiguration != null)
                                    {
                                        // Rewrite the localPath to the one that is configured that matches the 'Location' property
                                        localPath = Path.Combine(fileConfiguration.Path, resource);
                                        localPath = localPath.Replace('/', '\\');
                                        valid = true;
                                    }
                                }
                            }
                        }
                    }

                    if (valid)
                    {
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
                            if (fileContents == null)
                            {
                                fileContents = File.ReadAllBytes(filePath);
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
    }
}
