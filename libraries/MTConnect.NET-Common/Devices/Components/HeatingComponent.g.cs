// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_68e0225_1605117125123_371301_1943

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// System that delivers controlled amounts of heat to achieve a target temperature at a specified heating rate.
    /// </summary>
    public class HeatingComponent : Component
    {
        public const string TypeId = "Heating";
        public const string NameId = "heating";
        public new const string DescriptionText = "System that delivers controlled amounts of heat to achieve a target temperature at a specified heating rate.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public HeatingComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}