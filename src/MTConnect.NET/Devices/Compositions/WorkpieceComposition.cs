// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// An object or material on which a form of work is performed.
    /// </summary>
    public class WorkpieceComposition : Composition 
    {
        public const string TypeId = "WORKPIECE";
        public const string NameId = "wrkpiece";

        public WorkpieceComposition()  { Type = TypeId; }
    }
}
