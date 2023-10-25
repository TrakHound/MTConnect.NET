// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1579572382015_53595_42288

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Component Types::Axis that provides rotation about a fixed axis.
    /// </summary>
    public class RotaryComponent : Component
    {
        public const string TypeId = "Rotary";
        public const string NameId = "rotaryComponent";
        public new const string DescriptionText = "Component Types::Axis that provides rotation about a fixed axis.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version10; 


        public RotaryComponent() { Type = TypeId; }
    }
}