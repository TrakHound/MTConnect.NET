// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_45f01b9_1580312106478_840214_44471

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Leaf Component that transforms electric energy from a source to a secondary circuit.
    /// </summary>
    public class TransformerComponent : Component
    {
        public const string TypeId = "Transformer";
        public const string NameId = "transformer";
        public new const string DescriptionText = "Leaf Component that transforms electric energy from a source to a secondary circuit.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version15; 


        public TransformerComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}