using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RestSharp;

namespace MTConnect.Client
{
    public class Probe
    {
        public Probe(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        public string BaseUrl { get; set; }

        public string Execute()
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest("probe", Method.GET);
            IRestResponse response = client.Execute(request);
            return response.Content;
        }

    }
}
