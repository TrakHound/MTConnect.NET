// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1622456766067_72580_282

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Abstract Component composed of a part being processed by a piece of equipment.
    /// </summary>
    public abstract class PartComponent : Component
    {
        public const string TypeId = "Part";
        public const string NameId = "partComponent";
        public new const string DescriptionText = "Abstract Component composed of a part being processed by a piece of equipment.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public PartComponent() { Type = TypeId; }
    }
}