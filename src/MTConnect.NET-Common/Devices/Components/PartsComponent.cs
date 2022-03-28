// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Parts organizes information for Parts being processed by a piece of equipment.
    /// </summary>
    public class PartsComponent : Component 
    {
        public const string TypeId = "Parts";
        public const string NameId = "parts";
        public new const string DescriptionText = "Parts organizes information for Parts being processed by a piece of equipment.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version18;


        public PartsComponent()  { Type = TypeId; }
    }
}
