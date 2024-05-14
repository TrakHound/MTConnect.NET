// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1579572381970_785259_42204

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Abstract Component composed of removable part(s) of a piece of equipment that provides supplementary or extended functionality.
    /// </summary>
    public abstract class AuxiliaryComponent : Component
    {
        public const string TypeId = "Auxiliary";
        public const string NameId = "auxiliary";
        public new const string DescriptionText = "Abstract Component composed of removable part(s) of a piece of equipment that provides supplementary or extended functionality.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version14; 


        public AuxiliaryComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}