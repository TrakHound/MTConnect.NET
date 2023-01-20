// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Enclosure is a System that represents the information for a structure used to contain or isolate a piece of equipment or area. 
    /// The Enclosure system may provide information regarding access to the internal components of a piece of equipment or the conditions within the enclosure.
    /// </summary>
    public class EnclosureComponent : Component 
    {
        public const string TypeId = "Enclosure";
        public const string NameId = "encl";
        public new const string DescriptionText = "Enclosure is a System that represents the information for a structure used to contain or isolate a piece of equipment or area. The Enclosure system may provide information regarding access to the internal components of a piece of equipment or the conditions within the enclosure.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version14;


        public EnclosureComponent()  { Type = TypeId; }
    }
}