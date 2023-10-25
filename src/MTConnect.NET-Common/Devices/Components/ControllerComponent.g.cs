// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.2 : UML ID = _19_0_3_45f01b9_1579572381977_283525_42216

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// System that provides regulation or management of a system or component. ISO 16484-5:2017
    /// </summary>
    public class ControllerComponent : Component
    {
        public const string TypeId = "Controller";
        public const string NameId = "controllerComponent";
        public new const string DescriptionText = "System that provides regulation or management of a system or component. ISO 16484-5:2017";

        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version10; 


        public ControllerComponent() { Type = TypeId; }
    }
}