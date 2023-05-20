// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using Ceen;
using MTConnect.Agents;
using MTConnect.Configurations;
using MTConnect.Http;
using MTConnect.Servers.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MTConnect.Servers
{
    class MTConnectStaticResponseHandler : MTConnectHttpResponseHandler
    {
        public Func<MTConnectStaticFileRequest, byte[]> ProcessFunction { get; set; }


        public MTConnectStaticResponseHandler(IHttpAgentConfiguration configuration, IMTConnectAgentBroker mtconnectAgent) : base(configuration, mtconnectAgent) { }


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
                    var localPath = requestedPath;

                    // Get File extension to prevent certain files (.exe, .dll, etc) from being accessed
                    // that could cause security concerns
                    var pathExtension = Path.GetExtension(requestedPath);
                    var valid = pathExtension != ".exe" && pathExtension != ".dll";

                    if (valid)
                    {
                        valid = false;

                        // Check to see if the path matches one that is configured
                        if (!_configuration.Files.IsNullOrEmpty())
                        {
                            var resource = Path.GetFileName(requestedPath);
                            contentType = MimeTypes.GetMimeType(resource);

                            var relativePath = Path.GetDirectoryName(requestedPath);
                            if (!string.IsNullOrEmpty(relativePath)) relativePath = relativePath.Replace('\\', '/');
                            else relativePath = resource;

                            if (!string.IsNullOrEmpty(relativePath))
                            {
                                // Find a FileConfiguration whose Location matches the requested Resource
                                var fileConfiguration = _configuration.Files.FirstOrDefault(o => o.Location == resource);
                                if (fileConfiguration != null)
                                {
                                    // Rewrite the localPath to the one that is configured that matches the 'Location' property
                                    localPath = fileConfiguration.Path;
                                    valid = true;
                                }
                                else
                                {
                                    // Find a FileConfiguration whose Location matches the requested Resource
                                    fileConfiguration = _configuration.Files.FirstOrDefault(o => o.Location == relativePath);
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
                            if (ProcessFunction != null)
                            {
                                var staticFileRequest = new MTConnectStaticFileRequest();
                                staticFileRequest.HttpRequest = httpRequest;
                                staticFileRequest.FilePath = filePath;
                                staticFileRequest.LocalPath = localPath;
                                staticFileRequest.Version = version;

                                fileContents = ProcessFunction(staticFileRequest);
                            }

                            // Check Overridden method
                            //fileContents = OnProcessStatic(httpRequest, filePath, localPath, version);

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







            //var httpRequest = context.Request;
            //var httpResponse = context.Response;

            //if (httpRequest != null && httpRequest.Path != null && httpResponse != null)
            //{
            //    var absolutePath = "";
            //    var relativePath = "";

            //    // Get File extension to prevent certain files (.exe, .dll, etc) from being accessed
            //    // that could cause security concerns
            //    var pathExtension = Path.GetExtension(relativePath);
            //    var valid = pathExtension != ".exe" && pathExtension != ".dll";

            //    if (valid)
            //    {
            //        if (_agentConfiguration.DevicesStyle != null && (relativePath == _agentConfiguration.DevicesStyle.Location || relativePath.Replace('\\', '/') == _agentConfiguration.StreamsStyle.Location))
            //        {
            //            return ReadDevicesStylesheet(absolutePath, version);
            //        }
            //        else if (_agentConfiguration.StreamsStyle != null && (relativePath == _agentConfiguration.StreamsStyle.Location || relativePath.Replace('\\', '/') == _agentConfiguration.StreamsStyle.Location))
            //        {
            //            return ReadStreamsStylesheet(absolutePath, version);
            //        }
            //    }






            //    //var urlSegments = GetUriSegments(httpRequest.OriginalPath);

            //    //// Get Accept-Encoding Header (ex. gzip, br)
            //    //var acceptEncodings = GetRequestHeaderValues(httpRequest, HttpHeaders.AcceptEncoding);
            //    //acceptEncodings = ProcessAcceptEncodings(acceptEncodings);

            //    //// Read AssetIds from URL Path
            //    //var assetIdsString = httpRequest.Path?.Trim('/');
            //    //if (urlSegments.Length > 1)
            //    //{
            //    //    assetIdsString = urlSegments[urlSegments.Length - 1];
            //    //}

            //    //// Create list of AssetIds
            //    //IEnumerable<string> assetIds = null;
            //    //if (!string.IsNullOrEmpty(assetIdsString))
            //    //{
            //    //    assetIds = assetIdsString.Split(';');
            //    //    if (assetIds.IsNullOrEmpty()) assetIds = new List<string>() { assetIdsString };
            //    //}

            //    //// Read MTConnectVersion from Query string
            //    //var versionString = httpRequest.QueryString["version"];
            //    //Version.TryParse(versionString, out var version);
            //    //if (version == null) version = _mtconnectAgent.MTConnectVersion;

            //    //// Read DocumentFormat from Query string
            //    //var documentFormatString = httpRequest.QueryString["documentFormat"];
            //    //var documentFormat = DocumentFormat.XML;
            //    //if (!string.IsNullOrEmpty(documentFormatString) && documentFormatString.ToUpper() == DocumentFormat.JSON.ToString())
            //    //{
            //    //    documentFormat = DocumentFormat.JSON;
            //    //}

            //    //// Read ValidationLevel from Query string
            //    //int validationLevel = (int)_configuration.OutputValidationLevel;
            //    //var validationLevelString = httpRequest.QueryString["validationLevel"];
            //    //if (!string.IsNullOrEmpty(validationLevelString)) validationLevel = validationLevelString.ToInt();


            //    //// Set Format Options
            //    //var formatOptions = CreateFormatOptions(MTConnectRequestType.Asset, documentFormat, version, validationLevel);
            //    //formatOptions.Add(new KeyValuePair<string, string>("validationLevel", validationLevel.ToString()));

            //    //// Read IndentOutput from Query string
            //    //var indentOutputString = httpRequest.QueryString["indentOutput"];
            //    //if (!string.IsNullOrEmpty(indentOutputString)) formatOptions.Add(new KeyValuePair<string, string>("indentOutput", indentOutputString));
            //    //else formatOptions.Add(new KeyValuePair<string, string>("indentOutput", _configuration.IndentOutput.ToString()));

            //    //// Read OutputComments from Query string
            //    //var outputCommentsString = httpRequest.QueryString["outputComments"];
            //    //if (!string.IsNullOrEmpty(outputCommentsString)) formatOptions.Add(new KeyValuePair<string, string>("outputComments", outputCommentsString));
            //    //else formatOptions.Add(new KeyValuePair<string, string>("outputComments", _configuration.OutputComments.ToString()));


            //    //// Get MTConnectAssets document from the MTConnectAgent
            //    //return MTConnectHttpRequests.GetAssetRequest(_mtconnectAgent, assetIds, version, documentFormat, formatOptions);
            //}

            return response;
        }


        //protected override OnCreate
        

        //protected override List<KeyValuePair<string, string>> OnCreateFormatOptions(string requestType, string documentFormat, Version mtconnectVersion, int validationLevel = 0)
        //{
        //    if (CreateFormatOptionsFunction != null)
        //    {
        //        var formatOptions = new MTConnectFormatOptionsArgs();
        //        formatOptions.RequestType = requestType;
        //        formatOptions.DocumentFormat = documentFormat;
        //        formatOptions.MTConnectVersion = mtconnectVersion;
        //        formatOptions.ValidationLevel = validationLevel;

        //        return CreateFormatOptionsFunction(formatOptions);
        //    }

        //    return null;
        //}


        //#region "Stylesheets"

        //private static byte[] ReadDevicesStylesheet(string filePath, Version mtconnectVersion)
        //{
        //    if (filePath != null)
        //    {
        //        try
        //        {
        //            var fileContents = File.ReadAllText(filePath);
        //            if (!string.IsNullOrEmpty(fileContents))
        //            {
        //                string s = fileContents;
        //                string pattern = null;
        //                string replace = null;

        //                try
        //                {
        //                    // Replace Devices Namespace
        //                    pattern = @"urn:mtconnect\.org:MTConnectDevices:(\d\.\d)";
        //                    replace = $@"urn:mtconnect.org:MTConnectDevices:{mtconnectVersion}";
        //                    s = Regex.Replace(fileContents, pattern, replace);
        //                }
        //                catch { }

        //                try
        //                {
        //                    // Replace Streams Namespace
        //                    pattern = @"urn:mtconnect\.org:MTConnectStreams:(\d\.\d)";
        //                    replace = $@"urn:mtconnect.org:MTConnectStreams:{mtconnectVersion}";
        //                    s = Regex.Replace(s, pattern, replace);
        //                }
        //                catch { }

        //                return Encoding.UTF8.GetBytes(s);
        //            }
        //        }
        //        catch { }
        //    }

        //    return null;
        //}

        //private static byte[] ReadStreamsStylesheet(string filePath, Version mtconnectVersion)
        //{
        //    if (filePath != null)
        //    {
        //        try
        //        {
        //            var fileContents = File.ReadAllText(filePath);
        //            if (!string.IsNullOrEmpty(fileContents))
        //            {
        //                if (mtconnectVersion != null)
        //                {
        //                    // Replace Streams Namespace
        //                    var pattern = @"urn:mtconnect\.org:MTConnectStreams:(\d\.\d)";
        //                    var replace = $@"urn:mtconnect.org:MTConnectStreams:{mtconnectVersion.Major}.{mtconnectVersion.Minor}";
        //                    fileContents = Regex.Replace(fileContents, pattern, replace);
        //                }

        //                return Encoding.UTF8.GetBytes(fileContents);
        //            }
        //        }
        //        catch { }
        //    }

        //    return null;
        //}

        //#endregion

        //#region "Schemas"

        //private static string ReadDevicesSchema(Version mtconnectVersion)
        //{
        //    if (mtconnectVersion != null)
        //    {
        //        var versionKey = mtconnectVersion.ToString();
        //        string schema = null;
        //        lock (_lock) if (_devicesSchemas.TryGetValue(versionKey, out var x)) schema = x;

        //        if (string.IsNullOrEmpty(schema))
        //        {
        //            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "schemas");
        //            var filename = $"MTConnectDevices_{mtconnectVersion.Major}.{mtconnectVersion.Minor}.xsd";
        //            var path = Path.Combine(dir, filename);

        //            if (File.Exists(path))
        //            {
        //                try
        //                {
        //                    schema = File.ReadAllText(path);
        //                    if (!string.IsNullOrEmpty(schema))
        //                    {
        //                        lock (_lock) _devicesSchemas.Add(versionKey, schema);
        //                    }
        //                }
        //                catch { }
        //            }
        //        }

        //        return schema;
        //    }

        //    return null;
        //}

        //private static string ReadStreamsSchema(Version mtconnectVersion)
        //{
        //    if (mtconnectVersion != null)
        //    {
        //        var versionKey = mtconnectVersion.ToString();
        //        string schema = null;
        //        lock (_lock) if (_streamsSchemas.TryGetValue(versionKey, out var x)) schema = x;

        //        if (string.IsNullOrEmpty(schema))
        //        {
        //            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "schemas");
        //            var filename = $"MTConnectStreams_{mtconnectVersion.Major}.{mtconnectVersion.Minor}.xsd";
        //            var path = Path.Combine(dir, filename);

        //            if (File.Exists(path))
        //            {
        //                try
        //                {
        //                    schema = File.ReadAllText(path);
        //                    if (!string.IsNullOrEmpty(schema))
        //                    {
        //                        lock (_lock) _streamsSchemas.Add(versionKey, schema);
        //                    }
        //                }
        //                catch { }
        //            }
        //        }

        //        return schema;
        //    }

        //    return null;
        //}

        //private static string ReadAssetsSchema(Version mtconnectVersion)
        //{
        //    if (mtconnectVersion != null)
        //    {
        //        var versionKey = mtconnectVersion.ToString();
        //        string schema = null;
        //        lock (_lock) if (_assetsSchemas.TryGetValue(versionKey, out var x)) schema = x;

        //        if (string.IsNullOrEmpty(schema))
        //        {
        //            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "schemas");
        //            var filename = $"MTConnectAssets_{mtconnectVersion.Major}.{mtconnectVersion.Minor}.xsd";
        //            var path = Path.Combine(dir, filename);

        //            if (File.Exists(path))
        //            {
        //                try
        //                {
        //                    schema = File.ReadAllText(path);
        //                    if (!string.IsNullOrEmpty(schema))
        //                    {
        //                        lock (_lock) _assetsSchemas.Add(versionKey, schema);
        //                    }
        //                }
        //                catch { }
        //            }
        //        }

        //        return schema;
        //    }

        //    return null;
        //}

        //private static string ReadCommonSchema(Version mtconnectVersion)
        //{
        //    if (mtconnectVersion != null)
        //    {
        //        var key = "xlink";
        //        string schema = null;
        //        lock (_lock) if (_commonSchemas.TryGetValue(key, out var x)) schema = x;

        //        if (string.IsNullOrEmpty(schema))
        //        {
        //            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "schemas");
        //            var filename = "xlink.xsd";
        //            var path = Path.Combine(dir, filename);

        //            if (File.Exists(path))
        //            {
        //                try
        //                {
        //                    schema = File.ReadAllText(path);
        //                    if (!string.IsNullOrEmpty(schema))
        //                    {
        //                        lock (_lock) _commonSchemas.Add(key, schema);
        //                    }
        //                }
        //                catch { }
        //            }
        //        }

        //        return schema;
        //    }

        //    return null;
        //}

        //#endregion

    }
}
