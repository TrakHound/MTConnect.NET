// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1579572382014_307743_42285

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Abstract Component composed of material or personnel involved in a manufacturing process.
    /// </summary>
    public abstract class ResourceComponent : Component
    {
        public const string TypeId = "Resource";
        public const string NameId = "resource";
        public new const string DescriptionText = "Abstract Component composed of material or personnel involved in a manufacturing process.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public ResourceComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}