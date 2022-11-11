// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Agents;
using MTConnect.Errors;
using MTConnect.Streams.Output;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MTConnect.Servers.Http
{
    /// <summary>
    /// Methods to handle requesting Response Documents from an IMTConnectAgent. Reponses are returned as an MTConnectHttpResponse.
    /// </summary>
    public static class MTConnectHttpRequests
    {
        /// <summary>
        /// An Agent responds to a Probe Request with an MTConnectDevices Response Document that contains the 
        /// Equipment Metadata for pieces of equipment that are requested and currently represented in the Agent.
        /// </summary>
        /// <param name="mtconnectAgent">The IMTConnectAgent to request data from</param>
        /// <param name="mtconnectVersion">The MTConnect Version of the response document</param>
        /// <param name="documentFormat">The format of the response document</param>
        /// <param name="formatOptions">Flags used for Formatting the Response</param>
        /// <returns>An MTConnectHttpResponse</returns>
        public static MTConnectHttpResponse GetProbeRequest(
            IMTConnectAgentBroker mtconnectAgent, 
            string deviceType = null,
            Version mtconnectVersion = null,
            string documentFormat = DocumentFormat.XML, 
            IEnumerable<KeyValuePair<string, string>> formatOptions = null
            )
        {
            var stpw = Stopwatch.StartNew();

            // Get MTConnectDevices document from the MTConnectAgent
            var document = mtconnectAgent.GetDevicesResponseDocument(mtconnectVersion, deviceType);

            stpw.Stop();
            double duration = (double)stpw.ElapsedTicks / 10000;

            if (document != null)
            {
                // Return MTConnectDevices Response Document
                var response = new MTConnectHttpResponse(document, documentFormat.ToString(), duration, formatOptions);
                if (!response.Success)
                {
                    var errors = new List<Error>();
                    if (!response.FormatErrors.IsNullOrEmpty())
                    {
                        foreach (var error in response.FormatErrors) errors.Add(new Error(ErrorCode.INTERNAL_ERROR, error));
                    }

                    // Return MTConnectError Response Document
                    var error500Document = mtconnectAgent.GetErrorResponseDocument(errors, mtconnectVersion);
                    var errorResponse = new MTConnectHttpResponse(error500Document, 500, documentFormat, duration, formatOptions);
                    errorResponse.FormatErrors = response.FormatErrors;
                    return errorResponse;
                }

                document = null;
                return response;
            }
            else
            {
                // Return MTConnectError Response Document
                var error404Document = mtconnectAgent.GetErrorResponseDocument(ErrorCode.NO_DEVICE, "The Request could not be interpreted. No Devices were Found", mtconnectVersion);
                return new MTConnectHttpResponse(error404Document, 404, documentFormat, duration, formatOptions);
            }
        }

        /// <summary>
        /// An Agent responds to a Probe Request with an MTConnectDevices Response Document that contains the 
        /// Equipment Metadata for pieces of equipment that are requested and currently represented in the Agent.
        /// </summary>
        /// <param name="mtconnectAgent">The IMTConnectAgent to request data from</param>
        /// <param name="deviceKey">A specific Path portion (name or uuid)</param>
        /// <param name="mtconnectVersion">The MTConnect Version of the response document</param>
        /// <param name="formatOptions">Flags used for Formatting the Response</param>
        /// <returns>An MTConnectHttpResponse</returns>
        public static MTConnectHttpResponse GetDeviceProbeRequest(
            IMTConnectAgentBroker mtconnectAgent, 
            string deviceKey,
            Version mtconnectVersion = null,
            string documentFormat = DocumentFormat.XML,
            IEnumerable<KeyValuePair<string, string>> formatOptions = null
            )
        {
            var stpw = Stopwatch.StartNew();

            if (!string.IsNullOrEmpty(deviceKey))
            {
                // Get MTConnectDevices document from the MTConnectAgent
                var document = mtconnectAgent.GetDevicesResponseDocument(deviceKey, mtconnectVersion);

                stpw.Stop();
                double duration = (double)stpw.ElapsedTicks / 10000;

                if (document != null)
                {
                    // Return MTConnectDevices Response Document
                    var response = new MTConnectHttpResponse(document, documentFormat, duration, formatOptions);
                    if (!response.Success)
                    {
                        var errors = new List<Error>();
                        if (!response.FormatErrors.IsNullOrEmpty())
                        {
                            foreach (var error in response.FormatErrors) errors.Add(new Error(ErrorCode.INTERNAL_ERROR, error));
                        }

                        // Return MTConnectError Response Document
                        var error500Document = mtconnectAgent.GetErrorResponseDocument(errors, mtconnectVersion);
                        var errorResponse = new MTConnectHttpResponse(error500Document, 500, documentFormat, duration, formatOptions);
                        errorResponse.FormatErrors = response.FormatErrors;
                        return errorResponse;
                    }

                    document = null;
                    return response;
                }
                else
                {
                    // Return MTConnectError Response Document
                    var error404Document = mtconnectAgent.GetErrorResponseDocument(ErrorCode.NO_DEVICE, $"The Request could not be interpreted. The Device \"{deviceKey}\" was Not Found", mtconnectVersion);
                    return new MTConnectHttpResponse(error404Document, 404, documentFormat, duration, formatOptions);
                }
            }

            // Return MTConnectError Response Document
            var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.INVALID_REQUEST, "An empty \"device\" was received and the request could not be processed", mtconnectVersion);
            return new MTConnectHttpResponse(errorDocument, 400, documentFormat, stpw.ElapsedMilliseconds, formatOptions);
        }


        /// <summary>
        /// An Agent responds to a Current Request with an MTConnectStreams Response Document that contains
        /// the current value of Data Entities associated with each piece of Streaming Data available from the Agent, subject to any filtering defined in the Request.
        /// </summary>
        /// <param name="mtconnectAgent">The IMTConnectAgent to request data from</param>
        /// <param name="path">An XPath that defines specific information or a set of information to be included in an MTConnectStreams Response Document.</param>
        /// <param name="at">Requests that the MTConnect Response Documents MUST include the current value for all Data Entities relative to the time that a specific sequence number was recorded.</param>
        /// <param name="interval">The Agent MUST continuously publish Response Documents when the query parameters include interval using the value as the minimum period between adjacent publications.</param>
        /// <param name="mtconnectVersion">The MTConnect Version of the response document</param>
        /// <param name="documentFormat">The format of the response document</param>
        /// <param name="formatOptions">Flags used for Formatting the Response</param>
        /// <returns>An MTConnectHttpResponse</returns>
        public static MTConnectHttpResponse GetCurrentRequest(
            IMTConnectAgentBroker mtconnectAgent,
            string deviceType = null,
            string path = null,
            long at = 0,
            int interval = 0,
            Version mtconnectVersion = null,
            string documentFormat = DocumentFormat.XML,
            IEnumerable<KeyValuePair<string, string>> formatOptions = null
            )
        {
            var stpw = Stopwatch.StartNew();

            // Get list of DataItem ID's based on Path (XPath) parameter
            var dataItemIds = PathProcessor.GetDataItemIds(mtconnectAgent, path, documentFormat);

            // Return MTConnectError Response Document : Invalid Request if "at" is less than zero
            if (at < 0)
            {
                var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.INVALID_REQUEST, ErrorText.InvalidRequestNegativeAt, mtconnectVersion);
                return new MTConnectHttpResponse(errorDocument, 400, documentFormat, stpw.ElapsedMilliseconds, formatOptions);
            }

            // Return MTConnectError Response Document : Invalid Request if "at" and "interval" are specified
            if (at > 0 && interval > 0)
            {
                var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.INVALID_REQUEST, ErrorText.InvalidRequestAtIntervalConjunction, mtconnectVersion);
                return new MTConnectHttpResponse(errorDocument, 400, documentFormat, stpw.ElapsedMilliseconds, formatOptions);
            }

            // Return MTConnectError Response Document : Out Of Range if "at" less than Agent FirstSequence
            if (at > 0 && at < mtconnectAgent.FirstSequence)
            {
                var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.OUT_OF_RANGE, ErrorText.OutOfRangeAt, mtconnectVersion);
                return new MTConnectHttpResponse(errorDocument, 404, documentFormat, stpw.ElapsedMilliseconds, formatOptions);
            }

            // Return MTConnectError Response Document : Out Of Range if "at" greater than Agent LastSequence
            if (at > 0 && at > mtconnectAgent.LastSequence)
            {
                var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.OUT_OF_RANGE, ErrorText.OutOfRangeAt, mtconnectVersion);
                return new MTConnectHttpResponse(errorDocument, 404, documentFormat, stpw.ElapsedMilliseconds, formatOptions);
            }

            IStreamsResponseOutputDocument document;

            // Get MTConnectStreams document from the MTConnectAgent
            if (dataItemIds != null) document = mtconnectAgent.GetDeviceStreamsResponseDocument(dataItemIds, at, mtconnectVersion: mtconnectVersion, deviceType: deviceType);
            else document = mtconnectAgent.GetDeviceStreamsResponseDocument(at, mtconnectVersion: mtconnectVersion, deviceType: deviceType);

            stpw.Stop();
            double duration = (double)stpw.ElapsedTicks / 10000;

            if (document != null)
            {
                // Return MTConnectStreams Response Document
                var response = new MTConnectHttpResponse(ref document, documentFormat, duration, formatOptions);
                if (!response.Success)
                {
                    var errors = new List<Error>();
                    if (!response.FormatErrors.IsNullOrEmpty())
                    {
                        foreach (var error in response.FormatErrors) errors.Add(new Error(ErrorCode.INTERNAL_ERROR, error));
                    }

                    // Return MTConnectError Response Document
                    var error500Document = mtconnectAgent.GetErrorResponseDocument(errors, mtconnectVersion);
                    var errorResponse = new MTConnectHttpResponse(error500Document, 500, documentFormat, duration, formatOptions);
                    errorResponse.FormatErrors = response.FormatErrors;
                    return errorResponse;
                }

                document = null;
                return response;
            }
            else
            {
                // Return MTConnectError Response Document
                var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.NO_DEVICE, "The Request could not be interpreted. No Devices were Found", mtconnectVersion);
                return new MTConnectHttpResponse(errorDocument, 404, documentFormat, duration, formatOptions);
            }
        }

        /// <summary>
        /// An Agent responds to a Current Request with an MTConnectStreams Response Document that contains
        /// the current value of Data Entities associated with each piece of Streaming Data available from the Agent, subject to any filtering defined in the Request.
        /// </summary>
        /// <param name="mtconnectAgent">The IMTConnectAgent to request data from</param>
        /// <param name="deviceKey">The (name or uuid) of the requested Device</param>
        /// <param name="path">An XPath that defines specific information or a set of information to be included in an MTConnectStreams Response Document.</param>
        /// <param name="at">Requests that the MTConnect Response Documents MUST include the current value for all Data Entities relative to the time that a specific sequence number was recorded.</param>
        /// <param name="interval">The Agent MUST continuously publish Response Documents when the query parameters include interval using the value as the minimum period between adjacent publications.</param>
        /// <param name="mtconnectVersion">The MTConnect Version of the response document</param>
        /// <param name="documentFormat">The format of the response document</param>
        /// <param name="formatOptions">Flags used for Formatting the Response</param>
        /// <returns>An MTConnectHttpResponse</returns>
        public static MTConnectHttpResponse GetDeviceCurrentRequest(
            IMTConnectAgentBroker mtconnectAgent,
            string deviceKey,
            string path = null,
            long at = 0,
            int interval = -1,
            Version mtconnectVersion = null,
            string documentFormat = DocumentFormat.XML,
            IEnumerable<KeyValuePair<string, string>> formatOptions = null
            )
        {
            var stpw = Stopwatch.StartNew();

            // Get list of DataItem ID's based on Path (XPath) parameter
            var dataItemIds = PathProcessor.GetDataItemIds(mtconnectAgent, path, documentFormat);

            // Return MTConnectError Response Document : Invalid Request if "at" is less than zero
            if (at < 0)
            {
                var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.INVALID_REQUEST, ErrorText.InvalidRequestNegativeAt, mtconnectVersion);
                return new MTConnectHttpResponse(errorDocument, 400, documentFormat, stpw.ElapsedMilliseconds, formatOptions);
            }

            // Return MTConnectError Response Document : Invalid Request if "at" and "interval" are specified
            if (at > 0 && interval > -1)
            {
                var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.INVALID_REQUEST, ErrorText.InvalidRequestAtIntervalConjunction, mtconnectVersion);
                return new MTConnectHttpResponse(errorDocument, 400, documentFormat, stpw.ElapsedMilliseconds, formatOptions);
            }

            // Return MTConnectError Response Document : Out Of Range if "at" less than Agent FirstSequence
            if (at > 0 && at < mtconnectAgent.FirstSequence)
            {
                var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.OUT_OF_RANGE, ErrorText.OutOfRangeAt, mtconnectVersion);
                return new MTConnectHttpResponse(errorDocument, 404, documentFormat, stpw.ElapsedMilliseconds, formatOptions);
            }

            // Return MTConnectError Response Document : Out Of Range if "at" greater than Agent LastSequence
            if (at > 0 && at > mtconnectAgent.LastSequence)
            {
                var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.OUT_OF_RANGE, ErrorText.OutOfRangeAt, mtconnectVersion);
                return new MTConnectHttpResponse(errorDocument, 404, documentFormat, stpw.ElapsedMilliseconds, formatOptions);
            }

            if (!string.IsNullOrEmpty(deviceKey))
            {
                IStreamsResponseOutputDocument document;

                // Get MTConnectStreams document from the MTConnectAgent
                if (dataItemIds != null) document = mtconnectAgent.GetDeviceStreamsResponseDocument(deviceKey, dataItemIds, at, mtconnectVersion: mtconnectVersion);
                else document = mtconnectAgent.GetDeviceStreamsResponseDocument(deviceKey, at, mtconnectVersion: mtconnectVersion);

                stpw.Stop();
                double duration = (double)stpw.ElapsedTicks / 10000;

                if (document != null)
                {
                    // Return MTConnectStreams Response Document
                    var response = new MTConnectHttpResponse(ref document, documentFormat, duration, formatOptions);
                    if (!response.Success)
                    {
                        var errors = new List<Error>();
                        if (!response.FormatErrors.IsNullOrEmpty())
                        {
                            foreach (var error in response.FormatErrors) errors.Add(new Error(ErrorCode.INTERNAL_ERROR, error));
                        }

                        // Return MTConnectError Response Document
                        var error500Document = mtconnectAgent.GetErrorResponseDocument(errors, mtconnectVersion);
                        var errorResponse = new MTConnectHttpResponse(error500Document, 500, documentFormat, duration, formatOptions);
                        errorResponse.FormatErrors = response.FormatErrors;
                        return errorResponse;
                    }

                    document = null;
                    return response;
                }
                else
                {
                    // Return MTConnectError Response Document
                    var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.NO_DEVICE, $"The Request could not be interpreted. The Device \"{deviceKey}\" was Not Found", mtconnectVersion);
                    return new MTConnectHttpResponse(errorDocument, 404, documentFormat, duration, formatOptions);
                }
            }
            else
            {
                // Return MTConnectError Response Document
                var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.INVALID_REQUEST, "An empty \"device\" was received and the request could not be processed", mtconnectVersion);
                return new MTConnectHttpResponse(errorDocument, 400, documentFormat, stpw.ElapsedMilliseconds, formatOptions);
            }
        }


        /// <summary>
        /// An Agent responds to a Sample Request with an MTConnectStreams Response Document that contains a set of values for Data Entities
        /// currently available for Streaming Data from the Agent, subject to any filtering defined in the Request.
        /// </summary>
        /// <param name="mtconnectAgent">The IMTConnectAgent to request data from</param>
        /// <param name="path">An XPath that defines specific information or a set of information to be included in an MTConnectStreams Response Document.</param>
        /// <param name="from">The from parameter designates the sequence number of the first observation in the buffer the Agent MUST consider publishing in the Response Document.</param>
        /// <param name="to">The to parameter specifies the sequence number of the observation in the buffer that will be the upper bound of the observations in the Response Document.</param>
        /// <param name="count">The count parameter designates the maximum number of observations the Agent MUST publish in the Response Document.</param>
        /// <param name="mtconnectVersion">The MTConnect Version of the response document</param>
        /// <param name="documentFormat">The format of the response document</param>
        /// <param name="formatOptions">Flags used for Formatting the Response</param>
        /// <returns>An MTConnectHttpResponse</returns>
        public static MTConnectHttpResponse GetSampleRequest(
            IMTConnectAgentBroker mtconnectAgent,
            string deviceType = null,
            string path = null,
            long from = 0,
            long to = 0,
            int count = 100,
            Version mtconnectVersion = null,
            string documentFormat = DocumentFormat.XML,
            IEnumerable<KeyValuePair<string, string>> formatOptions = null
            )
        {
            var stpw = Stopwatch.StartNew();

            // Get list of DataItem ID's based on Path (XPath) parameter
            var dataItemIds = PathProcessor.GetDataItemIds(mtconnectAgent, path, documentFormat);

            // Return MTConnectError Response Document : Invalid Request if "from" is less than zero
            if (from < 0)
            {
                var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.INVALID_REQUEST, ErrorText.InvalidRequestNegativeFrom, mtconnectVersion);
                return new MTConnectHttpResponse(errorDocument, 400, documentFormat, stpw.ElapsedMilliseconds, formatOptions);
            }

            // Return MTConnectError Response Document : Invalid Request if "to" is less than zero
            if (to < 0)
            {
                var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.INVALID_REQUEST, ErrorText.InvalidRequestNegativeTo, mtconnectVersion);
                return new MTConnectHttpResponse(errorDocument, 400, documentFormat, stpw.ElapsedMilliseconds, formatOptions);
            }

            // Return MTConnectError Response Document : Invalid Request if "to" is less than "from"
            if (to > 0 && to < from)
            {
                var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.INVALID_REQUEST, ErrorText.InvalidRequestToLessThanFrom, mtconnectVersion);
                return new MTConnectHttpResponse(errorDocument, 400, documentFormat, stpw.ElapsedMilliseconds, formatOptions);
            }

            // Return MTConnectError Response Document : Invalid Request if "to" is given and "count" is less than zero
            if (to > 0 && count < 0)
            {
                var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.INVALID_REQUEST, ErrorText.InvalidRequestNegativeCount, mtconnectVersion);
                return new MTConnectHttpResponse(errorDocument, 400, documentFormat, stpw.ElapsedMilliseconds, formatOptions);
            }

            // Return MTConnectError Response Document : Out Of Range if "from" less than Agent FirstSequence or greater than Agent LastSequence
            if (from > 0 && (from < mtconnectAgent.FirstSequence || from > mtconnectAgent.LastSequence))
            {
                var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.OUT_OF_RANGE, ErrorText.OutOfRangeFrom, mtconnectVersion);
                return new MTConnectHttpResponse(errorDocument, 404, documentFormat, stpw.ElapsedMilliseconds, formatOptions);
            }

            // Return MTConnectError Response Document : Out Of Range if "to" less than Agent FirstSequence or greater than Agent LastSequence
            if (to > 0 && (to < mtconnectAgent.FirstSequence || to > mtconnectAgent.LastSequence))
            {
                var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.OUT_OF_RANGE, ErrorText.OutOfRangeTo, mtconnectVersion);
                return new MTConnectHttpResponse(errorDocument, 404, documentFormat, stpw.ElapsedMilliseconds, formatOptions);
            }

            // Return MTConnectError Response Document : Out Of Range if "count" is greater than the size of the buffer or equal to zero
            if (count == 0 && count > mtconnectAgent.BufferSize)
            {
                var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.OUT_OF_RANGE, ErrorText.OutOfRangeCount, mtconnectVersion);
                return new MTConnectHttpResponse(errorDocument, 404, documentFormat, stpw.ElapsedMilliseconds, formatOptions);
            }

            IStreamsResponseOutputDocument document;

            // Get MTConnectStreams document from the MTConnectAgent
            if (dataItemIds != null) document = mtconnectAgent.GetDeviceStreamsResponseDocument(dataItemIds, from, to, count, mtconnectVersion, deviceType);
            else document = mtconnectAgent.GetDeviceStreamsResponseDocument(from, to, count, mtconnectVersion, deviceType);

            stpw.Stop();
            double duration = (double)stpw.ElapsedTicks / 10000;

            if (document != null)
            {
                // Return MTConnectStreams Response Document
                var response = new MTConnectHttpResponse(ref document, documentFormat, duration, formatOptions);
                if (!response.Success)
                {
                    var errors = new List<Error>();
                    if (!response.FormatErrors.IsNullOrEmpty())
                    {
                        foreach (var error in response.FormatErrors) errors.Add(new Error(ErrorCode.INTERNAL_ERROR, error));
                    }

                    // Return MTConnectError Response Document
                    var error500Document = mtconnectAgent.GetErrorResponseDocument(errors, mtconnectVersion);
                    var errorResponse = new MTConnectHttpResponse(error500Document, 500, documentFormat, duration, formatOptions);
                    errorResponse.FormatErrors = response.FormatErrors;
                    return errorResponse;
                }

                return response;
            }
            else
            {
                // Return MTConnectError Response Document
                var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.NO_DEVICE, "The Request could not be interpreted. No Devices were Not Found", mtconnectVersion);
                return new MTConnectHttpResponse(errorDocument, 404, documentFormat, duration, formatOptions);
            }
        }

        /// <summary>
        /// An Agent responds to a Sample Request with an MTConnectStreams Response Document that contains a set of values for Data Entities
        /// currently available for Streaming Data from the Agent, subject to any filtering defined in the Request.
        /// </summary>
        /// <param name="mtconnectAgent">The IMTConnectAgent to request data from</param>
        /// <param name="deviceKey">The (name or uuid) of the requested Device</param>
        /// <param name="path">An XPath that defines specific information or a set of information to be included in an MTConnectStreams Response Document.</param>
        /// <param name="from">The from parameter designates the sequence number of the first observation in the buffer the Agent MUST consider publishing in the Response Document.</param>
        /// <param name="to">The to parameter specifies the sequence number of the observation in the buffer that will be the upper bound of the observations in the Response Document.</param>  
        /// <param name="count">The count parameter designates the maximum number of observations the Agent MUST publish in the Response Document.</param>
        /// <param name="mtconnectVersion">The MTConnect Version of the response document</param>
        /// <param name="documentFormat">The format of the response document</param>
        /// <param name="formatOptions">Flags used for Formatting the Response</param>
        /// <returns>An MTConnectHttpResponse</returns>
        public static MTConnectHttpResponse GetDeviceSampleRequest(
            IMTConnectAgentBroker mtconnectAgent,
            string deviceKey,
            string path = null,
            long from = 0,
            long to = 0,
            int count = 100,
            Version mtconnectVersion = null,
            string documentFormat = DocumentFormat.XML,
            IEnumerable<KeyValuePair<string, string>> formatOptions = null
            )
        {
            var stpw = Stopwatch.StartNew();    
            
            // Get list of DataItem ID's based on Path (XPath) parameter
            var dataItemIds = PathProcessor.GetDataItemIds(mtconnectAgent, path, documentFormat);

            // Return MTConnectError Response Document : Invalid Request if "from" is less than zero
            if (from < 0)
            {
                var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.INVALID_REQUEST, ErrorText.InvalidRequestNegativeFrom, mtconnectVersion);
                return new MTConnectHttpResponse(errorDocument, 400, documentFormat, stpw.ElapsedMilliseconds, formatOptions);
            }

            // Return MTConnectError Response Document : Invalid Request if "to" is less than zero
            if (to < 0)
            {
                var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.INVALID_REQUEST, ErrorText.InvalidRequestNegativeTo, mtconnectVersion);
                return new MTConnectHttpResponse(errorDocument, 400, documentFormat, stpw.ElapsedMilliseconds, formatOptions);
            }

            // Return MTConnectError Response Document : Invalid Request if "to" is less than "from"
            if (to > 0 && to < from)
            {
                var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.INVALID_REQUEST, ErrorText.InvalidRequestToLessThanFrom, mtconnectVersion);
                return new MTConnectHttpResponse(errorDocument, 400, documentFormat, stpw.ElapsedMilliseconds, formatOptions);
            }

            // Return MTConnectError Response Document : Invalid Request if "to" is given and "count" is less than zero
            if (to > 0 && count < 0)
            {
                var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.INVALID_REQUEST, ErrorText.InvalidRequestNegativeCount, mtconnectVersion);
                return new MTConnectHttpResponse(errorDocument, 400, documentFormat, stpw.ElapsedMilliseconds, formatOptions);
            }

            // Return MTConnectError Response Document : Out Of Range if "from" less than Agent FirstSequence or greater than Agent LastSequence
            if (from > 0 && (from < mtconnectAgent.FirstSequence || from > mtconnectAgent.LastSequence))
            {
                var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.OUT_OF_RANGE, ErrorText.OutOfRangeFrom, mtconnectVersion);
                return new MTConnectHttpResponse(errorDocument, 404, documentFormat, stpw.ElapsedMilliseconds, formatOptions);
            }

            // Return MTConnectError Response Document : Out Of Range if "to" less than Agent FirstSequence or greater than Agent LastSequence
            if (to > 0 && (to < mtconnectAgent.FirstSequence || to > mtconnectAgent.LastSequence))
            {
                var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.OUT_OF_RANGE, ErrorText.OutOfRangeTo, mtconnectVersion);
                return new MTConnectHttpResponse(errorDocument, 404, documentFormat, stpw.ElapsedMilliseconds, formatOptions);
            }

            // Return MTConnectError Response Document : Out Of Range if "count" is greater than the size of the buffer or equal to zero
            if (count == 0 && count > mtconnectAgent.BufferSize)
            {
                var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.OUT_OF_RANGE, ErrorText.OutOfRangeCount, mtconnectVersion);
                return new MTConnectHttpResponse(errorDocument, 404, documentFormat, stpw.ElapsedMilliseconds, formatOptions);
            }

            if (!string.IsNullOrEmpty(deviceKey))
            {
                IStreamsResponseOutputDocument document;

                // Get MTConnectStreams document from the MTConnectAgent
                if (dataItemIds != null) document = mtconnectAgent.GetDeviceStreamsResponseDocument(deviceKey, dataItemIds, from, to, count, mtconnectVersion);
                else document = mtconnectAgent.GetDeviceStreamsResponseDocument(deviceKey, from, to, count, mtconnectVersion);

                stpw.Stop();
                double duration = (double)stpw.ElapsedTicks / 10000;

                if (document != null)
                {
                    // Return MTConnectStreams Response Document
                    var response = new MTConnectHttpResponse(ref document, documentFormat, duration, formatOptions);
                    if (!response.Success)
                    {
                        var errors = new List<Error>();
                        if (!response.FormatErrors.IsNullOrEmpty())
                        {
                            foreach (var error in response.FormatErrors) errors.Add(new Error(ErrorCode.INTERNAL_ERROR, error));
                        }

                        // Return MTConnectError Response Document
                        var error500Document = mtconnectAgent.GetErrorResponseDocument(errors, mtconnectVersion);
                        var errorResponse = new MTConnectHttpResponse(error500Document, 500, documentFormat, duration, formatOptions);
                        errorResponse.FormatErrors = response.FormatErrors;
                        return errorResponse;
                    }

                    document = null;
                    return response;
                }
                else
                {
                    // Return MTConnectError Response Document
                    var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.NO_DEVICE, $"The Request could not be interpreted. The Device \"{deviceKey}\" was Not Found", mtconnectVersion);
                    return new MTConnectHttpResponse(errorDocument, 404, documentFormat, duration, formatOptions);
                }
            }
            else
            {
                // Return MTConnectError Response Document
                var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.INVALID_REQUEST, "An empty \"device\" was received and the request could not be processed", mtconnectVersion);
                return new MTConnectHttpResponse(errorDocument, 400, documentFormat, stpw.ElapsedMilliseconds, formatOptions);
            }
        }


        /// <summary>
        /// An Agent responds to an Asset Request with an MTConnectAssets Response Document that contains
        /// information for MTConnect Assets from the Agent, subject to any filtering defined in the Request.
        /// </summary>
        /// <param name="mtconnectAgent">The IMTConnectAgent to request data from</param>
        /// <param name="deviceKey">Optional Device name or uuid. If not given, all devices are returned.</param>
        /// <param name="type">Defines the type of MTConnect Asset to be returned in the MTConnectAssets Response Document.</param>
        /// <param name="removed">
        /// An attribute that indicates whether the Asset has been removed from a piece of equipment.
        /// If the value of the removed parameter in the query is true, then Asset Documents for Assets that have been marked as removed from a piece of equipment will be included in the Response Document.
        /// If the value of the removed parameter in the query is false, then Asset Documents for Assets that have been marked as removed from a piece of equipment will not be included in the Response Document.
        /// </param>
        /// <param name="count">Defines the maximum number of Asset Documents to return in an MTConnectAssets Response Document.</param>
        /// <param name="mtconnectVersion">The MTConnect Version of the response document</param>
        /// <param name="documentFormat">The format of the response document</param>
        /// <param name="formatOptions">Flags used for Formatting the Response</param>
        /// <returns>An MTConnectHttpResponse</returns>
        public static MTConnectHttpResponse GetAssetsRequest(
            IMTConnectAgentBroker mtconnectAgent,
            string deviceKey = null,
            string type = null,
            bool removed = false,
            int count = 100,
            Version mtconnectVersion = null,
            string documentFormat = DocumentFormat.XML,
            IEnumerable<KeyValuePair<string, string>> formatOptions = null
            )
        {
            var stpw = Stopwatch.StartNew();

            // Get MTConnectAssets document from the MTConnectAgent
            var document = mtconnectAgent.GetAssetsResponseDocument(deviceKey, type, removed, count, mtconnectVersion);

            stpw.Stop();
            double duration = (double)stpw.ElapsedTicks / 10000;

            if (document != null)
            {
                // Return MTConnectAssets Response Document
                var response = new MTConnectHttpResponse(document, documentFormat, duration, formatOptions);
                if (!response.Success)
                {
                    var errors = new List<Error>();
                    if (!response.FormatErrors.IsNullOrEmpty())
                    {
                        foreach (var error in response.FormatErrors) errors.Add(new Error(ErrorCode.INTERNAL_ERROR, error));
                    }

                    // Return MTConnectError Response Document
                    var error500Document = mtconnectAgent.GetErrorResponseDocument(errors, mtconnectVersion);
                    var errorResponse = new MTConnectHttpResponse(error500Document, 500, documentFormat, duration, formatOptions);
                    errorResponse.FormatErrors = response.FormatErrors;
                    return errorResponse;
                }

                return response;
            }
            else
            {
                // Return MTConnectError Response Document
                var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.ASSET_NOT_FOUND, "The Request could not be interpreted.", mtconnectVersion);
                return new MTConnectHttpResponse(errorDocument, 404, documentFormat, duration, formatOptions);
            }
        }

        /// <summary>
        /// An Agent responds to an Asset Request with an MTConnectAssets Response Document that contains
        /// information for MTConnect Assets from the Agent, subject to any filtering defined in the Request.
        /// </summary>
        /// <param name="mtconnectAgent">The IMTConnectAgent to request data from</param>
        /// <param name="assetIds">Identifies the id attribute of the MTConnect Asset(s) to be provided by an Agent.</param>
        /// <param name="mtconnectVersion">The MTConnect Version of the response document</param>
        /// <param name="documentFormat">The format of the response document</param>
        /// <param name="formatOptions">Flags used for Formatting the Response</param>
        /// <returns>An MTConnectHttpResponse</returns>
        public static MTConnectHttpResponse GetAssetRequest(
            IMTConnectAgentBroker mtconnectAgent,
            IEnumerable<string> assetIds,
            Version mtconnectVersion = null,
            string documentFormat = DocumentFormat.XML,
            IEnumerable<KeyValuePair<string, string>> formatOptions = null
            )
        {
            var stpw = Stopwatch.StartNew();

            if (!assetIds.IsNullOrEmpty())
            {
                // Get MTConnectAssets document from the MTConnectAgent
                var document = mtconnectAgent.GetAssetsResponseDocument(assetIds, mtconnectVersion);

                stpw.Stop();
                double duration = (double)stpw.ElapsedTicks / 10000;

                if (document != null)
                {
                    // Return MTConnectAssets Response Document
                    var response = new MTConnectHttpResponse(document, documentFormat, duration, formatOptions);
                    if (!response.Success)
                    {
                        var errors = new List<Error>();
                        if (!response.FormatErrors.IsNullOrEmpty())
                        {
                            foreach (var error in response.FormatErrors) errors.Add(new Error(ErrorCode.INTERNAL_ERROR, error));
                        }

                        // Return MTConnectError Response Document
                        var error500Document = mtconnectAgent.GetErrorResponseDocument(errors, mtconnectVersion);
                        var errorResponse = new MTConnectHttpResponse(error500Document, 500, documentFormat, duration, formatOptions);
                        errorResponse.FormatErrors = response.FormatErrors;
                        return errorResponse;
                    }

                    return response;
                }
                else
                {
                    // Return MTConnectError Response Document
                    var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.ASSET_NOT_FOUND, "The Request could not be interpreted.", mtconnectVersion);
                    return new MTConnectHttpResponse(errorDocument, 404, documentFormat, duration, formatOptions);
                }
            }
            else
            {
                // Return MTConnectError Response Document
                var errorDocument = mtconnectAgent.GetErrorResponseDocument(ErrorCode.INVALID_REQUEST, "An empty \"assetId\" was received and the request could not be processed", mtconnectVersion);
                return new MTConnectHttpResponse(errorDocument, 400, documentFormat, stpw.ElapsedMilliseconds, formatOptions);
            }
        }
    }
}
