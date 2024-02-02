// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1580312106476_995417_44462

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component composed of a surface for holding an object or material.
    /// </summary>
    public class TableComponent : Component
    {
        public const string TypeId = "Table";
        public const string NameId = "table";
        public new const string DescriptionText = "Leaf Component composed of a surface for holding an object or material.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15; 


        public TableComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}