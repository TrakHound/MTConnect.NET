// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1579572381997_851399_42255

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Axis that provides prismatic motion along a fixed axis.
    /// </summary>
    public class LinearComponent : Component
    {
        public const string TypeId = "Linear";
        public const string NameId = "linear";
        public new const string DescriptionText = "Axis that provides prismatic motion along a fixed axis.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version10; 


        public LinearComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}