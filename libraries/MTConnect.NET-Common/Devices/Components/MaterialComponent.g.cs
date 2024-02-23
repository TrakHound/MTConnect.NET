// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1579572382002_513291_42264

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Resource composed of material that is consumed or used by the piece of equipment for production of parts, materials, or other types of goods.
    /// </summary>
    public class MaterialComponent : Component
    {
        public const string TypeId = "Material";
        public const string NameId = "material";
        public new const string DescriptionText = "Resource composed of material that is consumed or used by the piece of equipment for production of parts, materials, or other types of goods.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version18; 


        public MaterialComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}