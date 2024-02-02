// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1580312106469_237105_44420

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component composed of an electromechanical actuator that produces deflection of a beam of light or energy in response to electric current through its coil in a magnetic field.
    /// </summary>
    public class GalvanomotorComponent : Component
    {
        public const string TypeId = "Galvanomotor";
        public const string NameId = "galvanomotor";
        public new const string DescriptionText = "Leaf Component composed of an electromechanical actuator that produces deflection of a beam of light or energy in response to electric current through its coil in a magnetic field.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15; 


        public GalvanomotorComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}