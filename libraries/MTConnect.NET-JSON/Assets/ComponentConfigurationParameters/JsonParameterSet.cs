// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.ComponentConfigurationParameters;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.ComponentConfigurationParameters
{
    public class JsonParameterSet
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("parameters")]
        public IEnumerable<JsonParameter> Parameters { get; set; }


        public JsonParameterSet() { }

        public JsonParameterSet(IParameterSet parameterSet)
        {
            if (parameterSet != null)
            {
                Name = parameterSet.Name;

                if (!parameterSet.Parameters.IsNullOrEmpty())
                {
                    var jsonParameters = new List<JsonParameter>();
                    foreach (var parameter in parameterSet.Parameters)
                    {
                        jsonParameters.Add(new JsonParameter(parameter));
                    }             
                    Parameters = jsonParameters;
                }
            }
        }


        public IParameterSet ToParameterSet()
        {
            var parameterSet = new ParameterSet();
            parameterSet.Name = Name;

            if (!Parameters.IsNullOrEmpty())
            {
                var parameters = new List<IParameter>();
                foreach (var parameter in Parameters)
                {
                    parameters.Add(parameter.ToParameter());
                }
                parameterSet.Parameters = parameters;
            }

            return parameterSet;
        }
    }
}