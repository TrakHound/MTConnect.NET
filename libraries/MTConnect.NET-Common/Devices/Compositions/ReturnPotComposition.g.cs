// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Pot for a tool removed from spindle or Turret and awaiting for return to a ToolMagazine.
    /// </summary>
    public class ReturnPotComposition : Composition 
    {
        public const string TypeId = "RETURN_POT";
        public const string NameId = "returnPotComposition";
        public new const string DescriptionText = "Pot for a tool removed from spindle or Turret and awaiting for return to a ToolMagazine.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public ReturnPotComposition()  { Type = TypeId; }
    }
}