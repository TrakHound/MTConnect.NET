// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Interfaces
{
    /// <summary>
    /// Interface that coordinates the operations between a piece of equipment and another associated piece of equipment used to automatically handle various types of materials or services associated with the original piece of equipment. 
    /// </summary>
    public class MaterialHandlerInterface : Interface 
    {
        public const string TypeId = "MaterialHandlerInterface";
        public const string NameId = "materialHandler";
        public new const string DescriptionText = "Interface that coordinates the operations between a piece of equipment and another associated piece of equipment used to automatically handle various types of materials or services associated with the original piece of equipment.";

        public override string TypeDescription => DescriptionText;

        public override System.Version MinimumVersion => MTConnectVersions.Version13;


        public MaterialHandlerInterface()
        {
            Type = TypeId;
        }
    }
}