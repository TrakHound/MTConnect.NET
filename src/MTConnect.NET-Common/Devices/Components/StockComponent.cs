// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Stock is a Resource that represents the information for the material that is used in a manufacturing process
    /// and to which work is applied in a machine or piece of equipment to produce parts.
    /// Stock may be either a continuous piece of material from which multiple parts may be produced or it may be a 
    /// discrete piece of material that will be made into a part or a set of parts.
    /// </summary>
    public class StockComponent : Component 
    {
        public const string TypeId = "Stock";
        public const string NameId = "stock";
        public new const string DescriptionText = "Stock is a Resource that represents the information for the material that is used in a manufacturing process and to which work is applied in a machine or piece of equipment to produce parts. Stock may be either a continuous piece of material from which multiple parts may be produced or it may be a discrete piece of material that will be made into a part or a set of parts.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version13;


        public StockComponent()  { Type = TypeId; }
    }
}
