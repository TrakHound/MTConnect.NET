// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Xml;

namespace MTConnect.Application.Streams
{
    public static class Requests
    {

        public static Response GetCurrent(string baseUrl)
        {
            var uri = new Uri(baseUrl);
            uri = new Uri(uri, "current");

            return Get(uri.ToString());
        }

        public static Response Get(string url)
        {
            return _Get(url);
        }

        public static Response Get(string url, HTTP.ProxySettings proxySettings)
        {
            return _Get(url, proxySettings);
        }

        public static Response Get(string url, int timeout)
        {
            return _Get(url, null, timeout);
        }

        public static Response Get(string url, int timeout, int maxattempts)
        {
            return _Get(url, null, timeout, maxattempts);
        }

        public static Response Get(string url, HTTP.ProxySettings proxySettings, int timeout, int maxattempts)
        {
            return _Get(url, proxySettings, timeout, maxattempts);
        }

        #region "Private"

        private static Response _Get(string url, HTTP.ProxySettings proxySettings = null, int timeout = 10000, int maxattempts = 3)
        {
            Response result = null;

            var httpInfo = new HTTP.HTTPInfo();
            httpInfo.Url = url;
            httpInfo.ProxySettings = proxySettings;
            httpInfo.Timeout = timeout;
            httpInfo.MaxAttempts = maxattempts;

            string response = HTTP.GET(httpInfo);
            if (response != null)
            {
                result = new Streams.Response(response);

                //try
                //{
                //    XmlDocument doc = new XmlDocument();
                //    doc.LoadXml(response);

                //    if (doc.DocumentElement != null)
                //    {
                //        // Get Root Element from Xml Document
                //        XmlElement root = doc.DocumentElement;

                //        // Get Header_Streams object from Root node
                //        Headers.Streams header = GetHeader(root);

                //        // Get DeviceStream object from Root node
                //        List<DeviceStream> deviceStreams = GetDeviceStream(root);

                //        if (deviceStreams != null)
                //        {
                //            result = new Response();
                //            result.Header = header;
                //            result.DeviceStreams = deviceStreams;
                //        }
                //    }
                //}
                //catch (XmlException ex)
                //{
                //    Log.Write("XmlException :: " + ex.Message);
                //}
                //catch (Exception ex)
                //{
                //    Log.Write("Exception :: " + ex.Message);
                //}
            }

            return result;
        }

        //static List<DeviceStream> GetDeviceStream(XmlElement root)
        //{
        //    List<DeviceStream> result = null;

        //    XmlNodeList deviceStreamNodes = root.GetElementsByTagName("DeviceStream");

        //    if (deviceStreamNodes != null)
        //    {
        //        if (deviceStreamNodes.Count > 0)
        //        {
        //            result = new List<DeviceStream>();

        //            foreach (XmlElement deviceNode in deviceStreamNodes)
        //            {
        //                var deviceStream = new DeviceStream(deviceNode);

        //                result.Add(deviceStream);
        //            }
        //        }
        //    }

        //    return result;
        //}

        //static Headers.Streams GetHeader(XmlElement root)
        //{
        //    Headers.Streams result = null;

        //    XmlNodeList headerNodes = root.GetElementsByTagName("Header");

        //    if (headerNodes != null)
        //    {
        //        if (headerNodes.Count > 0)
        //        {
        //            XmlNode headerNode = headerNodes[0];

        //            var header = new Headers.Streams(headerNode);

        //            result = header;
        //        }
        //    }

        //    return result;
        //}

        #endregion

    }
}
