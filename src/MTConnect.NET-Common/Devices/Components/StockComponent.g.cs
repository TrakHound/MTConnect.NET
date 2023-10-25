// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1579572382018_505205_42294

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Material that is used in a manufacturing process and to which work is applied in a machine or piece of equipment to produce parts.
    /// </summary>
    public class StockComponent : Component
    {
        public const string TypeId = "Stock";
        public const string NameId = "stockComponent";
        public new const string DescriptionText = "Material that is used in a manufacturing process and to which work is applied in a machine or piece of equipment to produce parts.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version13; 


        public StockComponent() { Type = TypeId; }
    }
}