// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1579572381973_701090_42210

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Loader that delivers bar stock to a piece of equipment.
    /// </summary>
    public class BarFeederComponent : Component
    {
        public const string TypeId = "BarFeeder";
        public const string NameId = "barFeederComponent";
        public new const string DescriptionText = "Loader that delivers bar stock to a piece of equipment.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public BarFeederComponent() { Type = TypeId; }
    }
}