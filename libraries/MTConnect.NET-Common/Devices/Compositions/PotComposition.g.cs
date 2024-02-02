// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Compositions
{
    /// <summary>
    /// Composition composed of a tool storage location associated with a ToolMagazine or AutomaticToolChanger.
    /// </summary>
    public class PotComposition : Composition 
    {
        public const string TypeId = "POT";
        public const string NameId = "potComposition";
        public new const string DescriptionText = "Composition composed of a tool storage location associated with a ToolMagazine or AutomaticToolChanger.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public PotComposition()  { Type = TypeId; }
    }
}