// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Assets.ComponentConfigurationParameters
{
    /// <summary>
    /// Set of Parameters.
    /// </summary>
    public interface IParameterSet
    {
        /// <summary>
        /// Name of the parameter set if more than one exists.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Property defining a configuration of a Component.
        /// </summary>
        System.Collections.Generic.IEnumerable<MTConnect.Assets.ComponentConfigurationParameters.IParameter> Parameters { get; }
    }
}