// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Adapters organizes Adapter component types.
    /// </summary>
    public class AdaptersComponent : Component 
    {
        public const string TypeId = "Adapters";
        public const string NameId = "adapters";
        public new const string DescriptionText = "Adapters organizes Adapter component types.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version17;


        public AdaptersComponent()  { Type = TypeId; }
    }
}