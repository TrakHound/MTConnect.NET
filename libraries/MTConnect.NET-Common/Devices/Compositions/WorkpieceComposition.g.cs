// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of an object or material on which a form of work is performed.
    /// </summary>
    public class WorkpieceComposition : Composition 
    {
        public const string TypeId = "WORKPIECE";
        public const string NameId = "workpieceComposition";
        public new const string DescriptionText = "Composition composed of an object or material on which a form of work is performed.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version16; 


        public WorkpieceComposition()  { Type = TypeId; }
    }
}