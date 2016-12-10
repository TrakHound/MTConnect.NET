// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using RestSharp;

namespace MTConnect.Client
{
    public class Current
    {
        public Current(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        public string BaseUrl { get; set; }

        public MTConnectStreams.Document Execute()
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest("current", Method.GET);
            IRestResponse response = client.Execute(request);
            if (response != null && !string.IsNullOrEmpty(response.Content))
            {
                return new MTConnectStreams.Document(response.Content);
            }

            return null;
        }

    }
}
