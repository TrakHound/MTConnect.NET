// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_68e0225_1760961027481_97077_59

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// System that securely interfaces a Component with a Device
    /// </summary>
    public class ToolHolderComponent : Component
    {
        public const string TypeId = "ToolHolder";
        public const string NameId = "toolHolder";
        public new const string DescriptionText = "System that securely interfaces a Component with a Device";

        public override string TypeDescription => DescriptionText;
        
         


        public ToolHolderComponent()
        {
            Type = TypeId;
        }
    }
}
