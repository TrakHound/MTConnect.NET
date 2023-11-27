// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1579572382008_658658_42273

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// System that uses compressed gasses to actuate components or do work within the piece of equipment.
    /// </summary>
    public class PneumaticComponent : Component
    {
        public const string TypeId = "Pneumatic";
        public const string NameId = "pneumaticComponent";
        public new const string DescriptionText = "System that uses compressed gasses to actuate components or do work within the piece of equipment.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version11; 


        public PneumaticComponent() { Type = TypeId; }
    }
}