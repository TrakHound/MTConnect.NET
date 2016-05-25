// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.IO;
using System.Net;

namespace MTConnect
{
    public static class HTTP
    {
        private const int CONNECTION_ATTEMPTS = 3;
        private const int TIMEOUT = 10000;

        #region "Public"

        public class ProxySettings
        {
            public ProxySettings() { }
            public ProxySettings(string address, int port)
            {
                Address = address;
                Port = port;
            }

            public string Address { get; set; }
            public int Port { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public static string GetUrl(string address, int port, string deviceName)
        {
            string url = null;

            if (address != null)
            {
                url = "http://";

                // Add Ip Address
                string ip = address;

                // Add Port
                string lport = null;
                // If port is in ip address
                if (ip.Contains(":"))
                {
                    int colonindex = ip.LastIndexOf(':');
                    int slashindex = -1;

                    // Get index of last forward slash
                    if (ip.Contains("/")) slashindex = ip.IndexOf('/', colonindex);

                    // Get port based on indexes
                    if (slashindex > colonindex) lport = ":" + ip.Substring(colonindex + 1, slashindex - colonindex - 1) + "/";
                    else lport = ":" + ip.Substring(colonindex + 1) + "/";

                    ip = ip.Substring(0, colonindex);
                }
                else
                {
                    if (port > 0) lport = ":" + port.ToString() + "/";
                }

                url += ip;
                url += lport;

                // Add Device Name
                string ldeviceName = null;
                if (deviceName != String.Empty)
                {
                    if (lport != null) ldeviceName = deviceName;
                    else ldeviceName = "/" + deviceName;
                    //ldeviceName += "/";
                }
                url += ldeviceName;

                if (url[url.Length - 1] != '/') url += "/";
            }

            return url;
        }

        #endregion

        internal class HTTPInfo
        {
            public HTTPInfo()
            {
                Init();
            }

            public HTTPInfo(string url)
            {
                Init();
                Url = url;
            }

            private void Init()
            {
                Url = "";
                Data = null;
                UserAgent = null;
                Timeout = 5000;
                MaxAttempts = 3;
            }

            public string Url { get; set; }
            public byte[] Data { get; set; }
            public string UserAgent { get; set; }
            public ProxySettings ProxySettings { get; set; }
            public int Timeout { get; set; }
            public int MaxAttempts { get; set; }
        }

        internal static string GET(HTTPInfo info)
        {
            return SendData("GET", info.Url, info.Data, info.UserAgent, info.ProxySettings, info.Timeout, info.MaxAttempts);
        }

        private static string SendData(string method, string url,
            byte[] sendBytes = null,
            string userAgent = null,
            ProxySettings proxySettings = null,
            int timeout = TIMEOUT, int maxAttempts = CONNECTION_ATTEMPTS
            )
        {
            string result = null;

            int attempts = 0;
            bool success = false;
            string message = null;

            // Try to send data for number of connectionAttempts
            while (attempts < maxAttempts && !success)
            {
                attempts += 1;

                try
                {
                    // Create HTTP request and define Header info
                    var request = (HttpWebRequest)WebRequest.Create(url);
                    request.Timeout = timeout;
                    request.ReadWriteTimeout = timeout;
                    request.ContentType = "application/x-www-form-urlencoded";
                    //request.ContentType = "application/json";

                    // Set the Method
                    request.Method = method;

                    // Set the UserAgent
                    if (userAgent != null) request.UserAgent = userAgent;
                    else request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

                    // Add POST data to request stream
                    if (sendBytes != null)
                    {
                        request.ContentLength = sendBytes.Length;

                        Stream postStream = request.GetRequestStream();
                        postStream.WriteTimeout = timeout;
                        postStream.Write(sendBytes, 0, sendBytes.Length);
                        postStream.Flush();
                        postStream.Close();
                    }
                    else request.ContentLength = 0;

                    // Get Default System Proxy (Windows Internet Settings -> Proxy Settings)
                    var proxy = WebRequest.GetSystemWebProxy();

                    // Get Custom Proxy Settings from Argument (overwrite default proxy settings)
                    if (proxySettings != null)
                    {
                        if (proxySettings.Address != null && proxySettings.Port > 0)
                        {
                            var customProxy = new WebProxy(proxySettings.Address, proxySettings.Port);
                            customProxy.BypassProxyOnLocal = false;
                            proxy = customProxy;
                        }
                    }

                    request.Proxy = proxy;

                    // Get HTTP resonse and return as string
                    using (var response = (HttpWebResponse)request.GetResponse())
                    using (var s = response.GetResponseStream())
                    using (var reader = new StreamReader(s))
                    {
                        result = reader.ReadToEnd();
                        success = true;
                    }
                }
                catch (WebException wex) { message = wex.Message; }
                catch (Exception ex) { message = ex.Message; }

                if (!success) System.Threading.Thread.Sleep(500);
            }

            if (!success) Log.Write("Send :: " + attempts.ToString() + " Attempts :: URL = " + url + " :: " + message);

            return result;
        }
    }
}
