// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_68e0225_1760961028686_546213_65

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// System composed of a tool that performs the work for a Friction Stir Welding process
    /// </summary>
    public class PinToolComponent : Component
    {
        public const string TypeId = "PinTool";
        public const string NameId = "pinTool";
        public new const string DescriptionText = "System composed of a tool that performs the work for a Friction Stir Welding process";

        public override string TypeDescription => DescriptionText;
        
         


        public PinToolComponent() 
        { 
            Type = TypeId;
            Name = NameId;
        }
    }
}