// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Pot for a tool awaiting transfer to a ToolMagazine or Turret from outside of the piece of equipment.
    /// </summary>
    public class StagingPotComposition : Composition 
    {
        public const string TypeId = "STAGING_POT";
        public const string NameId = "stagingPotComposition";
        public new const string DescriptionText = "Pot for a tool awaiting transfer to a ToolMagazine or Turret from outside of the piece of equipment.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public StagingPotComposition()  { Type = TypeId; }
    }
}