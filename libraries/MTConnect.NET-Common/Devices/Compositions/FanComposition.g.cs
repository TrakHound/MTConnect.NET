// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a mechanism that produces a current of air.
    /// </summary>
    public class FanComposition : Composition 
    {
        public const string TypeId = "FAN";
        public const string NameId = "fanComposition";
        public new const string DescriptionText = "Composition composed of a mechanism that produces a current of air.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public FanComposition()  { Type = TypeId; }
    }
}