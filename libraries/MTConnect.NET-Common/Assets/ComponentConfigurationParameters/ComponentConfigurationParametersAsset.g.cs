// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1678197202508_829668_17803

namespace MTConnect.Assets.ComponentConfigurationParameters
{
    /// <summary>
    /// Set of parameters that govern the functionality of the related Component.
    /// </summary>
    public partial class ComponentConfigurationParametersAsset : Asset, IComponentConfigurationParametersAsset
    {
        public new const string DescriptionText = "Set of parameters that govern the functionality of the related Component.";


        /// <summary>
        /// Set of parameters defining the configuration of a Component.
        /// </summary>
        public System.Collections.Generic.IEnumerable<MTConnect.Assets.ComponentConfigurationParameters.IParameterSet> ParameterSets { get; set; }
    }
}