// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Assets;
using MTConnect.Devices;
using MTConnect.Errors;
using MTConnect.Streams;

namespace MTConnect.Http
{
    public struct MTConnectHttpResponse
    {
        public string Content { get; set; }

        public string ContentType { get; set; }

        public int StatusCode { get; set; }

        public long ResponseDuration { get; set; }


        public MTConnectHttpResponse(DevicesDocument document, DocumentFormat documentFormat, long responseDuration, bool indent = false)
        {
            Content = null;
            ContentType = null;
            StatusCode = 500;
            ResponseDuration = responseDuration;

            if (document != null)
            {
                if (documentFormat == DocumentFormat.XML)
                {
                    Content = document.ToXml(indent);
                    ContentType = "application/xml";
                }
                else if (documentFormat == DocumentFormat.JSON)
                {
                    Content = document.ToJson(indent);
                    ContentType = "application/json";
                }

                StatusCode = 200;
            }
        }

        public MTConnectHttpResponse(StreamsDocument document, DocumentFormat documentFormat, long responseDuration, bool indent = false)
        {
            Content = null;
            ContentType = null;
            StatusCode = 500;
            ResponseDuration = responseDuration;

            if (document != null)
            {
                if (documentFormat == DocumentFormat.XML)
                {
                    Content = document.ToXml(indent);
                    ContentType = "application/xml";
                }
                else if (documentFormat == DocumentFormat.JSON)
                {
                    Content = document.ToJson(indent);
                    ContentType = "application/json";
                }

                StatusCode = 200;
            }
        }

        public MTConnectHttpResponse(AssetsDocument document, DocumentFormat documentFormat, long responseDuration, bool indent = false)
        {
            Content = null;
            ContentType = null;
            StatusCode = 500;
            ResponseDuration = responseDuration;

            if (document != null)
            {
                if (documentFormat == DocumentFormat.XML)
                {
                    Content = document.ToXml(indent);
                    ContentType = "application/xml";
                }
                else if (documentFormat == DocumentFormat.JSON)
                {
                    Content = document.ToJson(indent);
                    ContentType = "application/json";
                }

                StatusCode = 200;
            }
        }

        public MTConnectHttpResponse(ErrorDocument document, int statusCode, DocumentFormat documentFormat, long responseDuration, bool indent = false)
        {
            Content = null;
            ContentType = null;
            StatusCode = statusCode;
            ResponseDuration = responseDuration;

            if (document != null)
            {
                if (documentFormat == DocumentFormat.XML)
                {
                    Content = document.ToXml(indent);
                    ContentType = "application/xml";
                }
                else if (documentFormat == DocumentFormat.JSON)
                {
                    Content = document.ToJson(indent);
                    ContentType = "application/json";
                }
            }
        }
    }
}
