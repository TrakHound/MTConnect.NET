// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312106479_944360_44477

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component composed of a container that holds liquid or powdered materials.
    /// </summary>
    public class VatComponent : Component
    {
        public const string TypeId = "Vat";
        public const string NameId = "vat";
        public new const string DescriptionText = "Leaf Component composed of a container that holds liquid or powdered materials.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15; 


        public VatComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}