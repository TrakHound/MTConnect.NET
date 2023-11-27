// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1587597430378_591532_1084

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Structure that provides a connection between Component entities.
    /// </summary>
    public class LinkComponent : Component
    {
        public const string TypeId = "Link";
        public const string NameId = "linkComponent";
        public new const string DescriptionText = "Structure that provides a connection between Component entities.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version17; 


        public LinkComponent() { Type = TypeId; }
    }
}