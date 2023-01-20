// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// Turret is a ToolingDelivery that represents a tool mounting mechanism that holds any number of tools.
    /// Tools are located in STATIONs. Tools are positioned for use in the manufacturing process by rotating the Turret.
    /// </summary>
    public class TurretComponent : Component 
    {
        public const string TypeId = "Turret";
        public const string NameId = "turret";
        public new const string DescriptionText = "Turret is a ToolingDelivery that represents a tool mounting mechanism that holds any number of tools. Tools are located in STATIONs. Tools are positioned for use in the manufacturing process by rotating the Turret.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version17;


        public TurretComponent()  { Type = TypeId; }
    }
}