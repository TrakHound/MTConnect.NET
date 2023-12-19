// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanism that holds a part, stock material, or any other item in place.
    /// </summary>
    public class GripperComposition : Composition 
    {
        public const string TypeId = "GRIPPER";
        public const string NameId = "gripperComposition";
        public new const string DescriptionText = "Composition composed of a mechanism that holds a part, stock material, or any other item in place.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public GripperComposition()  { Type = TypeId; }
    }
}