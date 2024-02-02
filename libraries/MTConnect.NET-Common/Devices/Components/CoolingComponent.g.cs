// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1605117088875_728711_1893

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// System that extracts controlled amounts of heat to achieve a target temperature at a specified cooling rate.
    /// </summary>
    public class CoolingComponent : Component
    {
        public const string TypeId = "Cooling";
        public const string NameId = "cooling";
        public new const string DescriptionText = "System that extracts controlled amounts of heat to achieve a target temperature at a specified cooling rate.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public CoolingComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}