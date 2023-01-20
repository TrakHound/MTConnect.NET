// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// 
    /// </summary>
    public class PartComponent : Component 
    {
        public const string TypeId = "Part";
        public const string NameId = "part";
        public new const string DescriptionText = null;

        public override string TypeDescription => DescriptionText;


        public PartComponent()  { Type = TypeId; }
    }
}