// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1579572381997_851399_42255

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Component Types::Axis that provides prismatic motion along a fixed axis.
    /// </summary>
    public class LinearComponent : Component
    {
        public const string TypeId = "Linear";
        public const string NameId = "linearComponent";
        public new const string DescriptionText = "Component Types::Axis that provides prismatic motion along a fixed axis.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version10; 


        public LinearComponent() { Type = TypeId; }
    }
}