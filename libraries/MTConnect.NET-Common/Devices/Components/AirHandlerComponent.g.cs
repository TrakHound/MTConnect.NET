// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _19_0_3_68e0225_1696069278270_26061_1220

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// System that circulates air or regulates airflow without altering temperature or humidity.
    /// </summary>
    public class AirHandlerComponent : Component
    {
        public const string TypeId = "AirHandler";
        public const string NameId = "airHandler";
        public new const string DescriptionText = "System that circulates air or regulates airflow without altering temperature or humidity.";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version23; 


        public AirHandlerComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}