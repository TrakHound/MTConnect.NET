// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1580312106466_875747_44408

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component that emits a type of radiation.
    /// </summary>
    public class ExposureUnitComponent : Component
    {
        public const string TypeId = "ExposureUnit";
        public const string NameId = "exposureUnitComponent";
        public new const string DescriptionText = "Leaf Component that emits a type of radiation.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15; 


        public ExposureUnitComponent() { Type = TypeId; }
    }
}