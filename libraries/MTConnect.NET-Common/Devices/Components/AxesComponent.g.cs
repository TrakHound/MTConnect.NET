// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1607344218033_657673_1055

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Axis types.
    /// </summary>
    public class AxesComponent : Component, IOrganizerComponent
    {
        public const string TypeId = "Axes";
        public const string NameId = "axes";
        public new const string DescriptionText = "Axis types.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version10; 


        public AxesComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}