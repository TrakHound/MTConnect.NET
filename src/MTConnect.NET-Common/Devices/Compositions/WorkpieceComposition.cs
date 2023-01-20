// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// An object or material on which a form of work is performed.
    /// </summary>
    public class WorkpieceComposition : Composition 
    {
        public const string TypeId = "WORKPIECE";
        public const string NameId = "wrkpiece";
        public new const string DescriptionText = "An object or material on which a form of work is performed.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version16;


        public WorkpieceComposition()  { Type = TypeId; }
    }
}