// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1580312106470_968785_44426

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component composed of a chamber or bin in which materials are stored temporarily, typically being filled through the top and dispensed through the bottom.
    /// </summary>
    public class HopperComponent : Component
    {
        public const string TypeId = "Hopper";
        public const string NameId = "hopperComponent";
        public new const string DescriptionText = "Leaf Component composed of a chamber or bin in which materials are stored temporarily, typically being filled through the top and dispensed through the bottom.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public HopperComponent() { Type = TypeId; }
    }
}