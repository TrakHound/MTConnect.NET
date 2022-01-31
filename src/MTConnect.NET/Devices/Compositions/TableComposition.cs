// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// A surface for holding an object or material
    /// </summary>
    public class TableComposition : Composition 
    {
        public const string TypeId = "TABLE";
        public const string NameId = "tab";

        public TableComposition()  { Type = TypeId; }
    }
}
