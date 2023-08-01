// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
	/// <summary>
	/// Component that provides information related to an individual feature.
	/// </summary>
	public class FeatureOccurrenceComponent : Component 
    {
        public const string TypeId = "FeatureOccurrence";
        public const string NameId = "featureocc";
        public new const string DescriptionText = "Component that provides information related to an individual feature.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version22;


        public FeatureOccurrenceComponent()  { Type = TypeId; }
    }
}