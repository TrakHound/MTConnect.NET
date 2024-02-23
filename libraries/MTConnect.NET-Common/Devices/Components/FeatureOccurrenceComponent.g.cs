// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1678029650656_503771_494

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Component that provides information related to an individual feature.
    /// </summary>
    public class FeatureOccurrenceComponent : Component
    {
        public const string TypeId = "FeatureOccurrence";
        public const string NameId = "featureOccurrence";
        public new const string DescriptionText = "Component that provides information related to an individual feature.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version22; 


        public FeatureOccurrenceComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}