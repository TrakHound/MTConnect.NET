// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312106468_554120_44417

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component through which liquids or gases are passed to remove suspended impurities or to recover solids.
    /// </summary>
    public class FilterComponent : Component
    {
        public const string TypeId = "Filter";
        public const string NameId = "filter";
        public new const string DescriptionText = "Leaf Component through which liquids or gases are passed to remove suspended impurities or to recover solids.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public FilterComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}