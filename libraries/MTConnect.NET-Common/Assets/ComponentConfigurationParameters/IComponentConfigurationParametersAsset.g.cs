// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.ComponentConfigurationParameters
{
    /// <summary>
    /// Set of ParameterSets that govern the functionality of the related Component.
    /// </summary>
    public partial interface IComponentConfigurationParametersAsset : IAsset
    {
        /// <summary>
        /// Set of Parameters.
        /// </summary>
        System.Collections.Generic.IEnumerable<MTConnect.Assets.ComponentConfigurationParameters.IParameterSet> ParameterSets { get; }
    }
}