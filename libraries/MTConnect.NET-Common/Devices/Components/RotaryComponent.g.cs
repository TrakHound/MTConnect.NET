// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1579572382015_53595_42288

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Axis that provides rotation about a fixed axis.
    /// </summary>
    public class RotaryComponent : Component
    {
        public const string TypeId = "Rotary";
        public const string NameId = "rotary";
        public new const string DescriptionText = "Axis that provides rotation about a fixed axis.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version10; 


        public RotaryComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}