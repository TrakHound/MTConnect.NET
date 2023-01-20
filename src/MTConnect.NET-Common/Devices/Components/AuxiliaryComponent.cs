// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Auxiliary is an abstract Component that represents removable part(s) of a piece of equipment providing supplementary or extended functionality.
    /// </summary>
    public class AuxiliaryComponent : Component 
    {
        public const string TypeId = "Auxiliary";
        public const string NameId = "aux";
        public new const string DescriptionText = "Auxiliary is an abstract Component that represents removable part(s) of a piece of equipment providing supplementary or extended functionality.";

        public override string TypeDescription => DescriptionText;


        public AuxiliaryComponent()  { Type = TypeId; }
    }
}