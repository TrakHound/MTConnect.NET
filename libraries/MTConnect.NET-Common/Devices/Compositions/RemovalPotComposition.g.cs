// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Pot for a tool to be removed from a ToolMagazine or Turret to a location outside of the piece of equipment.
    /// </summary>
    public class RemovalPotCompositionComposition : Composition 
    {
        public const string TypeId = "REMOVAL_POT";
        public const string NameId = "removalPotComposition";
        public new const string DescriptionText = "Pot for a tool to be removed from a ToolMagazine or Turret to a location outside of the piece of equipment.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public RemovalPotCompositionComposition()  { Type = TypeId; }
    }
}