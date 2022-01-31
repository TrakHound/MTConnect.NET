// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// WasteDisposal is an Auxiliary that represents the information for a unit comprised of all the parts involved in removing manufacturing byproducts from a piece of equipment.
    /// </summary>
    public class WasteDisposalComponent : Component 
    {
        public const string TypeId = "WasteDisposal";
        public const string NameId = "waste";

        public WasteDisposalComponent()  { Type = TypeId; }
    }
}
