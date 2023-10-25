// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1678197254209_96040_17915

namespace MTConnect.Assets.ComponentConfigurationParameters
{
    /// <summary>
    /// Set of parameters defining the configuration of a Component.
    /// </summary>
    public class ParameterSet : IParameterSet
    {
        public const string DescriptionText = "Set of parameters defining the configuration of a Component.";


        /// <summary>
        /// Name of the parameter set if more than one exists.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Property defining a configuration of a Component.
        /// </summary>
        public System.Collections.Generic.IEnumerable<MTConnect.Assets.ComponentConfigurationParameters.IParameter> Parameters { get; set; }
    }
}