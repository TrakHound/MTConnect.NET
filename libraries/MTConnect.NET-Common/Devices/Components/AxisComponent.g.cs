// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1579572381972_553005_42207

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Abstract Component composed of a motion system that provides linear or rotational motion for a piece of equipment.
    /// </summary>
    public abstract class AxisComponent : Component
    {
        public const string TypeId = "Axis";
        public const string NameId = "axis";
        public new const string DescriptionText = "Abstract Component composed of a motion system that provides linear or rotational motion for a piece of equipment.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version10; 


        public AxisComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}