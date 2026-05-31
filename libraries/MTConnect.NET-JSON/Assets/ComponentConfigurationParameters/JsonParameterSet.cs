// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Assets.ComponentConfigurationParameters;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MTConnect.Assets.Json.ComponentConfigurationParameters
{
    /// <summary>
    /// JSON serialization surrogate for a <c>ParameterSet</c>, a named group
    /// of parameters within a ComponentConfigurationParameters asset.
    /// Converts to and from the strongly-typed <see cref="ParameterSet"/>
    /// model.
    /// </summary>
    public class JsonParameterSet
    {
        /// <summary>
        /// The name of the parameter set.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The parameters in the set.
        /// </summary>
        [JsonPropertyName("parameters")]
        public IEnumerable<JsonParameter> Parameters { get; set; }


        /// <summary>
        /// Initializes an empty instance for JSON deserialization.
        /// </summary>
        public JsonParameterSet() { }

        /// <summary>
        /// Initializes the surrogate from a strongly-typed
        /// <see cref="IParameterSet"/>, converting each parameter.
        /// </summary>
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


        /// <summary>
        /// Converts this surrogate to a strongly-typed
        /// <see cref="IParameterSet"/>, converting each parameter.
        /// </summary>
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